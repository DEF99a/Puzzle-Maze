using Assets.Scripts.Common;
using Assets.Scripts.Controller;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using EasyJoystick;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

[System.Serializable]
public class EnemyTypeAmount
{
    public EnemyType enemyType;
    public int amount;
}

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    static List<string> boxStrs = new List<string>()
    {
        "bombox", "ballbox", "giftbox", "box",
        "tile_stone"
    };

    [SerializeField] private int fakeLevel = -1;

    public List<GameObject> levelPfs;

    private List<Tilemap> frontTileMaps;
    private Tilemap backgroundTilemap;
    private Tilemap characterTilemap;

    [HideInInspector] public List<EnemyController> enemies;
    //private List<Box> normalBoxes;
    private List<EntityBase> allEntities;
    private Level curLevel;

    [HideInInspector] public int playerLife = 3, maxPlayerLife;

    public int CurLevelNumber { get; private set; }
    public int TotalKey { get; set; }
    public int KeyMatchAmount { get; set; }
    public List<GameObject> AllKeyBox { get; set; }
    public List<GameObject> AllGroundKey { get; set; }
    public List<Vector3Int> GateCheckpoints { get; private set; }
    public MapType curMapType { get; private set; }
    public SoundName GameMusicStr { get; private set; }

    private Vector3 playerPosition, firstPlayerPosition;

    private int diedEnemiesInPart;
    private int genedEnemiesInPart;
    private int currentEnemiesAmount;

    private int totalDiedEnemiesInAllPart;
    private int totalGenedEnemiesInAllPart;

    private int totalBoxCanBorn, totalBoxCanFly;
    private Coroutine genEnemyCoroutine;
    private Bound bound;
    private Dictionary<Vector3Int, Stone> allGateStone;
    private Dictionary<Vector3Int, BoxBase> allPositionCanBornBox;
    private Dictionary<Vector3Int, Stone> allStone;
    private List<Vector3Int> passedDoors;

    //cache
    private int playerLifeCache, coinCache, timeCache, x2Cache, enemyCache, enemyMaxCache,
       moreInvisibleCache, invisibleAmountCache, moreFreezeCache, freezeAmountCache, moreAngryCache, angryAmountCache;
    private Dictionary<CoinFrom, ValueTuple<int, int>> coinFromPairsCache;

    private void Awake()
    {
        instance = this;

        //normalBoxes = new List<Box>();
        allEntities = new List<EntityBase>();
        enemies = new List<EnemyController>();
        AllGroundKey = new List<GameObject>();
        AllKeyBox = new List<GameObject>();
        allGateStone = new Dictionary<Vector3Int, Stone>();
        GateCheckpoints = new List<Vector3Int>();
        allPositionCanBornBox = new Dictionary<Vector3Int, BoxBase>();
        allStone = new Dictionary<Vector3Int, Stone>();
        passedDoors = new List<Vector3Int>();
    }

    private void Start()
    {
        this.RegisterListener(EventID.EnemyDie, (enemy) => EnemyDieEvent(enemy));
        this.RegisterListener(EventID.PlayerDie, (dt) => PlayerDieEvent());
    }

    private void PlayerDieEvent()
    {
        playerLife--;
        GameUIManager.instance.UpdateLife(playerLife);
        Debug.Log("playerLife:" + playerLife);

        if (playerLife == 0)
        {
            GameManager.instance.GameOver(false, GameEndReason.PlayerOutLife);
        }
        else
        {
            playerPosition = PlayerController.instance.transform.position;
            Reborn();
        }
    }

    private void EnemyDieEvent(object en)
    {
        var enemy = (EnemyController)en;
        enemies.Remove(enemy);

        diedEnemiesInPart++;
        totalDiedEnemiesInAllPart++;

        Debug.Log("genedEnemiesInPart:" + genedEnemiesInPart);
        Debug.Log("diedEnemiesInPart:" + diedEnemiesInPart);

        if (Level.instance.mapType == MapType.All)
        {
            GameUIManager.instance.UpdateDiedMonster(totalDiedEnemiesInAllPart, totalGenedEnemiesInAllPart);
            if (diedEnemiesInPart == genedEnemiesInPart)
            {
                this.PostEvent(EventID.EndPartOfMap);
                Debug.Log("MapManager.instance.GateCheckpoints.Count:" + MapManager.instance.GateCheckpoints.Count);

                if (MapManager.instance.GateCheckpoints.Count <= 1)
                    GameManager.instance.GameOver(true, GameEndReason.WinPuzzle);
                else MapManager.instance.OpenGateInBound();

                GameManager.instance.StopGenBalloon();
                if (CurLevelNumber == 1)
                {
                    TutorialController.Instance?.GotoGate();
                }
            }
        }
        else
        {
            GameUIManager.instance.UpdateDiedMonster(totalDiedEnemiesInAllPart, curLevel.totalEnemy);
            currentEnemiesAmount--;
            Debug.Log("curLevel.totalEnemy:" + curLevel.totalEnemy + " totalDiedEnemiesInAllPart:" + totalDiedEnemiesInAllPart);

            if (curLevel.totalEnemy == totalDiedEnemiesInAllPart) GameManager.instance.GameOver(true, GameEndReason.AllEnemyDie);
            else
            {
                Debug.Log("totalBoxCanBorn:" + totalBoxCanBorn + " genedEnemiesInPart:" + totalDiedEnemiesInAllPart);
                if (totalBoxCanBorn <= 0 && genedEnemiesInPart == totalDiedEnemiesInAllPart)
                {
                    if (genedEnemiesInPart != curLevel.totalEnemy)//gen quai het roi
                    {
                        if (MapManager.instance.GateCheckpoints.Count <= 1)
                            GameManager.instance.GameOver(true, GameEndReason.OutBox);
                        else
                        {
                            this.PostEvent(EventID.EndPartOfMap);
                            MapManager.instance.OpenGateInBound();
                        }
                    }
                }    
                else GenEnemies();
            }
        }
    }

    int RealMap(int levelNumber)
    {
        //if(levelNumber >= 2 && levelNumber <= 10)
        //{
        //    levelNumber = 77 + levelNumber;
        //}
        //else if(levelNumber > 10)
        //{
        //    levelNumber = levelNumber - 9;
        //}

        return levelNumber;
    }

    public void LoadMap()
    {
#if !UNITY_EDITOR
fakeLevel = 0;
#endif
        GameObject levelPf;
        if (fakeLevel != 0)
        {
            CurLevelNumber = fakeLevel;
        }
        else
        {
            if (Config.SelectMap != 0)
                CurLevelNumber = Config.SelectMap;
            else
                CurLevelNumber = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
        }

        if (CurLevelNumber > levelPfs.Count)
            levelPf = levelPfs[UnityEngine.Random.Range(10, levelPfs.Count - 9)];
        else
        {
            levelPf = levelPfs[RealMap(CurLevelNumber) - 1];
        }

        var curLevelGameObject = Instantiate(levelPf, Vector3.zero, Quaternion.identity);

        curLevel = curLevelGameObject.GetComponent<Level>();

        var bgTf = curLevel.transform.Find("Background");
        frontTileMaps = new List<Tilemap>();
        for (int k = 0; k < curLevel.transform.childCount; k++)
        {
            var child = curLevel.transform.GetChild(k);
            if (child.name.Contains("Front"))
            {
                frontTileMaps.Add(child.GetComponent<Tilemap>());
            }
        }
        backgroundTilemap = bgTf.GetComponent<Tilemap>();
        characterTilemap = curLevel.transform.Find("Character")?.GetComponent<Tilemap>();
        characterTilemap?.gameObject.SetActive(false);

        PlayerType playerType = Config.SelectPlayerType;
        if (playerType == PlayerType.None) playerType = PlayerHelper.GetCurrentPlayer().playerType;
        playerLife = PlayerHelper.GetPlayerLife(playerType);
        maxPlayerLife = playerLife;

        //playerLife = 1000;

        GameUIManager.instance.UpdateLife(playerLife);

        GameUIManager.instance.UpdateDiedMonster(totalDiedEnemiesInAllPart, curLevel.totalEnemy);

        foreach (var frontTilemap in frontTileMaps)
        {
            foreach (var pos in frontTilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                if (frontTilemap.HasTile(localPlace))
                {
                    var tile = frontTilemap.GetTile(localPlace);
                    GameObject go = null;
                    //Debug.Log("tileeeee:" + tile.name);

                    if (tile.name.Equals("player"))
                    {
                        playerPosition = localPlace + new Vector3(0.5f, 0.5f);
                        Debug.Log("PlayerPos:" + playerPosition);
                        continue;
                    }
                    else if (tile.name.Contains(StringTypes.instance.pair[StringType.GateCheckPoint]))
                    {
                        GateCheckpoints.Add(localPlace);
                    }
                    else if (!boxStrs.Exists(b => tile.name.Contains(b)) && !tile.name.Contains(StringTypes.instance.pair[StringType.StoneLock])
                        && !tile.name.Contains(StringTypes.instance.pair[StringType.WoodLock])
                        && !tile.name.Contains(StringTypes.instance.pair[StringType.GroundKeyLock])
                        && !MyGraph.enemieStrs.Contains(tile.name))
                    {
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.stonePf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        var stone = go.GetComponent<Stone>();
                        stone.Init(frontTilemap.GetSprite(localPlace), frontTilemap.GetComponent<TilemapRenderer>().sortingOrder);

                        if (!allStone.ContainsKey(localPlace))
                            allStone.Add(localPlace, stone);

                        if (tile.name.Contains(StringTypes.instance.pair[StringType.DoorClose]))
                        {
                            //Debug.Log("tileeeee 2:" + tile.name);

                            if (!allGateStone.ContainsKey(localPlace))
                                allGateStone.Add(localPlace, stone);
                            MyGraph.instance.UpdateDoorCloseMap(localPlace, stone, tile.name);
                        }
                        continue;
                    }
                }
            }
            frontTilemap.gameObject.SetActive(false);
        }

        CalCameraBound();
        CalCurMapBound(playerPosition);
        EffectController.instance.GenEffectReborn(playerPosition);
    }

    private void CalCameraBound()
    {
        var minX = int.MaxValue;
        var maxX = int.MinValue;
        var minY = int.MaxValue;
        var maxY = int.MinValue;

        foreach (var pos in backgroundTilemap.cellBounds.allPositionsWithin)
        {
            if (backgroundTilemap.HasTile(pos))
            {
                if (minX > pos.x) minX = pos.x;
                if (maxX < pos.x) maxX = pos.x;
                if (minY > pos.y) minY = pos.y;
                if (maxY < pos.y) maxY = pos.y;
            }
        }
        CameraController.instance.Init(playerPosition, new Vector3(minX, minY), new Vector3(maxX + 1, maxY + 1));
    }

    public void CalCurMapBound(Vector3 position)
    {
        //var cell = MyGraph.instance.GetCellFromPosition(position, out _);

        //var cell1 = cell;
        //int i = 0;
        //do
        //{
        //    i++;
        //    cell1.y -= 1;
        //}
        //while (!allStone.ContainsKey(cell1) && i < Config.LimitWhile);
        //var minY = cell1.y;

        //i = 0;
        //var cell2 = cell;
        //do
        //{
        //    i++;
        //    cell2.x -= 1;
        //}
        //while (!allStone.ContainsKey(cell2) && i < Config.LimitWhile);
        //var minX = cell2.x;

        //i = 0;
        //var cell3 = cell;
        //do
        //{
        //    i++;
        //    cell3.y += 1;
        //}
        //while (!allStone.ContainsKey(cell3) && i < Config.LimitWhile);
        //var maxY = cell3.y;

        //i = 0;
        //var cell4 = cell;
        //do
        //{
        //    i++;
        //    cell4.x += 1;
        //}
        //while (!allStone.ContainsKey(cell4) && i < Config.LimitWhile);
        //var maxX = cell4.x;

        //bound = new Bound()
        //{
        //    min = new Vector3(minX, minY, 0),
        //    max = new Vector3(maxX + 1, maxY + 1, 0)
        //};
        //new GameObject("MIN").transform.position = new Vector3(minX, minY, 0);
        //new GameObject("MAX").transform.position = bound.max;
        bound = Level.instance.GetBound(position);
        firstPlayerPosition = position;

        ////if (Level.instance.mapType == MapType.All)
        ////{
        //    bound = Level.instance.GetBound(position);
        //    if(bound == null)
        //    {
        //        var minX = int.MaxValue;
        //        var maxX = int.MinValue;
        //        var minY = int.MaxValue;
        //        var maxY = int.MinValue;

        //        foreach (var pos in frontTileMaps[0].cellBounds.allPositionsWithin)
        //        {
        //            if (frontTileMaps[0].HasTile(pos))
        //            {
        //                if (minX > pos.x) minX = pos.x;
        //                if (maxX < pos.x) maxX = pos.x;
        //                if (minY > pos.y) minY = pos.y;
        //                if (maxY < pos.y) maxY = pos.y;
        //            }
        //        }
        //        bound = new Bound()
        //        {
        //            min = new Vector3(minX, minY),
        //            max = new Vector3(maxX + 1, maxY + 1)
        //        };
        //    }
        //    Debug.Log("bound : " + bound.min + " max:" + bound.max);
        //}
        //else
        //{
        //    var minX = int.MaxValue;
        //    var maxX = int.MinValue;
        //    var minY = int.MaxValue;
        //    var maxY = int.MinValue;

        //    foreach (var pos in frontTileMaps[0].cellBounds.allPositionsWithin)
        //    {
        //        if (frontTileMaps[0].HasTile(pos))
        //        {
        //            if (minX > pos.x) minX = pos.x;
        //            if (maxX < pos.x) maxX = pos.x;
        //            if (minY > pos.y) minY = pos.y;
        //            if (maxY < pos.y) maxY = pos.y;
        //        }
        //    }
        //    bound = new Bound()
        //    {
        //        min = new Vector3(minX, minY),
        //        max = new Vector3(maxX + 1, maxY + 1)
        //    };
        //}
    }

    public void GenEnemies()
    {
        if (Level.instance.mapType == MapType.All)
        {
            GenEnemiesInBound();
        }
        else
        {
            if (genEnemyCoroutine != null) return;

            genEnemyCoroutine = StartCoroutine(GenEnemiesIE());

            GameUIManager.instance.ShowHideLeftRect(false);
        }
    }

    private IEnumerator GenEnemiesIE()
    {
        //var box = normalBoxes.FirstOrDefault(b => b.transform.position.x == -5.5f && b.transform.position.y == -3.5f);
        //var go = ObjectPool.instance.GetGameObject(PrefabCache.instance.GetEnemyPrefab(EnemyType.Boo), box.transform.position, Quaternion.identity);
        //var enemy = go.GetComponent<EnemyController>();
        //enemies.Add(enemy);
        //box.HasEnemyAbove = true;
        //enemy.EnemyInit(EnemyType.Slime, box);
        //allEntities.Add(enemy);

        //box = normalBoxes.FirstOrDefault(b => b.transform.position.x == -4.5f && b.transform.position.y == -3.5f);
        //go = ObjectPool.instance.GetGameObject(PrefabCache.instance.GetEnemyPrefab(EnemyType.Boo), box.transform.position, Quaternion.identity);
        //enemy = go.GetComponent<EnemyController>();
        //enemies.Add(enemy);
        //box.HasEnemyAbove = true;
        //enemy.EnemyInit(EnemyType.Slime, box);
        //allEntities.Add(enemy);
        //yield break;

        var leaveEnemies = curLevel.totalEnemy - genedEnemiesInPart;
        var needBorn = curLevel.sameTimeEnemyAmount - currentEnemiesAmount;
        needBorn = needBorn > leaveEnemies ? leaveEnemies : needBorn;
        needBorn = needBorn > allPositionCanBornBox.Count ? allPositionCanBornBox.Count : needBorn;
        Debug.Log("needBorn 000:" + needBorn);
        if (needBorn == 0) yield break;

        var enemyTypes = new List<EnemyType>();
        for (int k = 0; k < curLevel.enemyTypeAmounts.Length; k++)
        {
            for (int l = 0; l < curLevel.enemyTypeAmounts[k].amount; l++)
            {
                enemyTypes.Add(curLevel.enemyTypeAmounts[k].enemyType);
            }
        }

        yield return new WaitForSeconds(2f);

        var tmpEnemyTypes = enemyTypes.ToList();
        while (true)
        {
            leaveEnemies = curLevel.totalEnemy - genedEnemiesInPart;
            needBorn = curLevel.sameTimeEnemyAmount - currentEnemiesAmount;
            needBorn = needBorn > leaveEnemies ? leaveEnemies : needBorn;
            needBorn = needBorn > allPositionCanBornBox.Count ? allPositionCanBornBox.Count : needBorn;

            Debug.Log("curLevel.totalEnemy:" + curLevel.totalEnemy + "totalGenedEnemies:" + genedEnemiesInPart
                + " curLevel.sameTimeEnemyAmount:" + curLevel.sameTimeEnemyAmount + " currentEnemy:" + currentEnemiesAmount);
            Debug.Log("needBorn 111:" + needBorn);
            Debug.Log("normalBoxes.Count:" + allPositionCanBornBox.Count);

            for (int k = 0; k < needBorn; k++)
            {
                var keys = allPositionCanBornBox.Keys;
                var key = keys.ElementAt(UnityEngine.Random.Range(0, keys.Count()));

                var box = allPositionCanBornBox[key];
                if (!box.IsMoving() && !box.HasEnemyAbove && box.CanBorn)
                {
                    EnemyType enemyType = EnemyType.Goomba;
                    if (tmpEnemyTypes.Count > 0)
                        enemyType = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)];
                    else
                    {
                        tmpEnemyTypes = enemyTypes.ToList();
                        if (tmpEnemyTypes.Count > 0)
                            enemyType = enemyTypes[UnityEngine.Random.Range(0, enemyTypes.Count)];
                    }
                    tmpEnemyTypes.Remove(enemyType);

                    var go = ObjectPool.instance.GetGameObject(PrefabCache.instance.GetEnemyPrefab(enemyType), box.transform.position, Quaternion.identity);
                    var enemy = go.GetComponent<EnemyController>();
                    enemies.Add(enemy);

                    box.HasEnemyAbove = true;

                    enemy.EnemyInit(enemyType, box);
                    allEntities.Add(enemy);

                    currentEnemiesAmount++;
                    genedEnemiesInPart++;
                }
            }
            if (needBorn == 0) break;
            yield return new WaitForSeconds(0.2f);
        }
        genEnemyCoroutine = null;
    }

    public void InitPlayer()
    {
        GenPlayer(true);
    }

    private void GenPlayer(bool firstInit)
    {
        PlayerType playerType = Config.SelectPlayerType;
        if (playerType == PlayerType.None) playerType = PlayerHelper.GetCurrentPlayer().playerType;
        var prefab = PrefabCache.instance.GetPlayerPrefab(playerType);
        var go = ObjectPool.instance.GetGameObject(prefab, playerPosition, Quaternion.identity);

        var player = go.GetComponent<PlayerController>();
        player.Init(PlayerHelper.GetPlayerSpeed(playerType), firstInit);
        allEntities.Add(player);
    }

    private void Reborn()
    {
        StartCoroutine(RebornIE());
        IEnumerator RebornIE()
        {
            yield return new WaitForSeconds(2f);
            GenPlayer(false);
        }
    }

    #region other logic
    public void RemoveNormalBox(Box box)
    {
        //normalBoxes.Remove(box);
        for (int k = 0; k < allPositionCanBornBox.Count; k++)
        {
            var posBox = allPositionCanBornBox.ElementAt(k);
            if (posBox.Value == box)
            {
                allPositionCanBornBox.Remove(posBox.Key);
                break;
            }
        }
        //Debug.Log("normalBoxes:" + normalBoxes.Count);
        //if (normalBoxes.Count == 0)
        //{
        //    GameManager.instance.GameOver(true);
        //}
    }

    public void DecreaseBoxAmount()
    {
        totalBoxCanBorn--;
        if (totalBoxCanBorn == 0)
        {
            if (Level.instance.mapType == MapType.All)
            {
                if (curMapType == MapType.Enemy)//gen quai het roi
                {
                    //if (MapManager.instance.GateCheckpoints.Count <= 1)
                    //    GameManager.instance.GameOver(true, GameEndReason.OutBox);
                    //else
                    //{
                    //    this.PostEvent(EventID.EndPartOfMap);
                    //    MapManager.instance.OpenGateInBound();
                    //}
                }
            }
            else
            {
                //if (genedEnemiesInPart != curLevel.totalEnemy)//gen quai het roi
                //{
                //    if (MapManager.instance.GateCheckpoints.Count <= 1)
                //        GameManager.instance.GameOver(true, GameEndReason.OutBox);
                //    else
                //    {
                //        this.PostEvent(EventID.EndPartOfMap);
                //        MapManager.instance.OpenGateInBound();
                //    }
                //}
            }
        }
    }

    public void DecreaseBoxCanFlyAmount()
    {
        totalBoxCanFly--;
        if (totalBoxCanFly == 0)
        {
            if ((Level.instance.mapType == MapType.All && curMapType == MapType.Enemy)
                || Level.instance.mapType == MapType.Enemy)
            {
                if (curMapType == MapType.Enemy)//gen quai het roi
                {
                    for(int k = 0; k < enemies.Count(); k++)
                    {
                        enemies[k].Killed();
                        enemies.RemoveAt(k);
                        k--;
                    }

                    //if (MapManager.instance.GateCheckpoints.Count <= 1)
                    //    GameManager.instance.GameOver(true, GameEndReason.OutBox);
                    //else
                    //{
                    //    this.PostEvent(EventID.EndPartOfMap);
                    //    MapManager.instance.OpenGateInBound();
                    //}
                }
            }
        }
    }

    public void ResetMovePoints()
    {
        foreach (var entity in allEntities)
        {
            if (entity != null && entity.IsLive())
                entity.ResetMovePoint();
        }
    }

    public void Continue(int life)
    {
        playerLife = life;
        maxPlayerLife = life;

        GameUIManager.instance.UpdateLife(playerLife);
        playerPosition = PlayerController.instance.transform.position;
        GenPlayer(false);
    }

    public Bound GetMinMaxStoneMap()
    {
        return bound;
    }

    public void ChangeKeyStage()
    {
        var allGroundKey = MapManager.instance.AllGroundKey;
        var allKeyBox = MapManager.instance.AllKeyBox;
        foreach (var keyBox in allKeyBox)
        {
            keyBox.GetComponent<BoxBase>().RemoveOnTileMap();
            keyBox.gameObject.SetActive(false);
        }
        foreach (var groundKey in allGroundKey)
        {
            groundKey.GetComponent<GroundKey>().spriteRenderer.sprite = EntitySprites.instance.GetSprite(Assets.Scripts.Define.EntityType.GroundKeyUnLock);
        }
    }
    #endregion

    private void GenEnemiesInBound()
    {
        StartCoroutine(GenIE());

        IEnumerator GenIE()
        {
            yield return new WaitForSeconds(1.5f);
            diedEnemiesInPart = 0;
            enemies.Clear();

            if (characterTilemap != null)
            {
                foreach (var pos in characterTilemap.cellBounds.allPositionsWithin)
                {
                    Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                    if (characterTilemap.HasTile(localPlace) && localPlace.x > bound.min.x && localPlace.x < bound.max.x
                        && localPlace.y > bound.min.y && localPlace.y < bound.max.y)
                    {
                        var tile = characterTilemap.GetTile(localPlace);
                        GameObject go = null;
                        EnemyType enemyType = EnemyType.None;
                        if (tile.name.Equals("boo")) enemyType = EnemyType.Boo;
                        else if (tile.name.Equals("bowser")) enemyType = EnemyType.Bowser;
                        else if (tile.name.Equals("foothecloud")) enemyType = EnemyType.FooTheCloud;
                        else if (tile.name.Equals("goomba")) enemyType = EnemyType.Goomba;
                        else if (tile.name.Equals("huanter-gastly")) enemyType = EnemyType.HuanterGastly;
                        else if (tile.name.Equals("koffing")) enemyType = EnemyType.Koffing;
                        else if (tile.name.Equals("koopa-troopa")) enemyType = EnemyType.KoopaTroopa;
                        else if (tile.name.Equals("meowth")) enemyType = EnemyType.Meowth;
                        else if (tile.name.Equals("rattata")) enemyType = EnemyType.Rattana;
                        else if (tile.name.Equals("weepinbell")) enemyType = EnemyType.Weepinbell;
                        else if (tile.name.Equals("zubat")) enemyType = EnemyType.Zubat;
                        else if (tile.name.Equals("slime")) enemyType = EnemyType.Slime;

                        if (enemyType != EnemyType.None)
                        {
                            go = ObjectPool.instance.GetGameObject(PrefabCache.instance.GetEnemyPrefab(enemyType),
                                localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                            var enemy = go.GetComponent<EnemyController>();
                            enemies.Add(enemy);

                            BoxBase boxBase = null;
                            if (allPositionCanBornBox.ContainsKey(localPlace))
                            {
                                boxBase = allPositionCanBornBox[localPlace];
                                if (boxBase.CanBorn)
                                    boxBase.HasEnemyAbove = true;
                                else boxBase = null;
                            }
                            Debug.Log("GenEnemiesInBound:" + localPlace + " boxBase:" + (boxBase != null));

                            enemy.EnemyInit(enemyType, boxBase);
                            allEntities.Add(enemy);
                        }
                    }
                }
            }

            genedEnemiesInPart = enemies.Count;
            totalGenedEnemiesInAllPart += genedEnemiesInPart;
            Debug.Log("totalGenedEnemiesInAllPart:" + totalGenedEnemiesInAllPart);

            GameUIManager.instance.UpdateDiedMonster(totalDiedEnemiesInAllPart, totalGenedEnemiesInAllPart);

            enemyCache = totalDiedEnemiesInAllPart;
            enemyMaxCache = totalGenedEnemiesInAllPart;
        }
    }

    public void GenItemsInBound()
    {
        allPositionCanBornBox.Clear();
        AllKeyBox.Clear();
        AllGroundKey.Clear();
        TotalKey = 0;
        KeyMatchAmount = 0;

        curMapType = MapType.Enemy;
        totalBoxCanBorn = 0;

        foreach (var frontTilemap in frontTileMaps)
        {
            foreach (var pos in frontTilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(pos.x, pos.y, pos.z);
                //Debug.Log("GenItemsInBound:" + localPlace);

                if (frontTilemap.HasTile(localPlace) && bound.InBound(localPlace))
                {
                    bool canKickFly = true;
                    bool canBorn = false;
                    bool canBreak = true;
                    bool canBurn = false;
                    bool enemyCanKickFly = true;
                    int moveAmount = -1;

                    var tile = frontTilemap.GetTile(localPlace);
                    GameObject go = null;
                    SkeletonDataAsset skeletonDataAsset;
                    if (tile.name.Contains("bombox"))
                    {
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.bombBoxPf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                    }
                    else if (tile.name.Contains("giftbox"))
                    {
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.giftBoxPf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                    }
                    else if (tile.name.Contains("ballbox"))
                    {
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.ballBoxPf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                    }
                    else if (tile.name.Contains("box") || tile.name.Contains("tile_stone"))
                    {
                        canBorn = true;
                        if (tile.name.Equals("box04_grass_01") || tile.name.Equals("box04_grass_02")
                            || tile.name.Equals("box04_grass_05") || tile.name.Equals("box04_grass_06"))
                        {
                            canKickFly = enemyCanKickFly = false;
                            canBurn = true;
                            if (tile.name.Equals("box04_grass_05") || tile.name.Equals("box04_grass_06")) canBorn = false;
                            go = ObjectPool.instance.GetGameObject(PrefabCache.instance.boxTreePf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        }
                        else if (tile.name.Equals("box03_fruit_01") || tile.name.Equals("box03_fruit_02"))
                        {
                            canBurn = canBreak = true;
                            canBorn = false;
                            go = ObjectPool.instance.GetGameObject(PrefabCache.instance.boxWaterMelon, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        }
                        else
                        {
                            if (tile.name.Equals("box03_fruit_03"))
                            {
                                canBreak = canKickFly = enemyCanKickFly = false;
                                canBurn = true;
                            }
                            else if (tile.name.Equals("box01_ground_01")) canBorn = false;
                            else if (tile.name.Equals("box01_ground_02"))
                            {
                                canBorn = true;
                                canBreak = false;
                            }
                            else if (tile.name.Equals("box04_grass_07") || tile.name.Equals("box04_grass_08"))
                            {
                                canBorn = false;
                                canBreak = false;
                            }
                            else if (tile.name.Equals("tile_stone_01") || tile.name.Equals("tile_stone_03"))
                            {
                                canKickFly = enemyCanKickFly = canBreak = canBorn = false;
                            }
                            else if (tile.name.Equals("tile_stone_02"))
                            {
                                canKickFly = enemyCanKickFly = canBreak = false;
                            }
                            go = ObjectPool.instance.GetGameObject(PrefabCache.instance.boxPf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        }
                        if (canBorn)
                            allPositionCanBornBox.Add(localPlace, go.GetComponent<BoxBase>());
                    }
                    else if (tile.name.Contains(StringTypes.instance.pair[StringType.StoneLock]))
                    {
                        canBreak = canBurn = enemyCanKickFly = false;
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.boxKey1Pf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        AllKeyBox.Add(go);
                    }
                    else if (tile.name.Contains(StringTypes.instance.pair[StringType.WoodLock]))
                    {
                        canBreak = canBurn = enemyCanKickFly = false;
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.boxKeyPf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        AllKeyBox.Add(go);
                    }
                    else if (tile.name.Contains(StringTypes.instance.pair[StringType.GroundKeyLock]))
                    {
                        go = ObjectPool.instance.GetGameObject(PrefabCache.instance.groundKeyPf, localPlace + new Vector3(0.5f, 0.5f), Quaternion.identity);
                        AllGroundKey.Add(go);
                        TotalKey++;

                        curMapType = MapType.Puzzle;
                    }
                    else if (tile.name.Contains(StringTypes.instance.pair[StringType.GateCheckPoint]))//gate_checkpoint
                    {
                        MyGraph.instance.CalculateAutoMoveLocation(localPlace);
                    }
                    if (go != null)
                    {
                        Enum.TryParse(tile.name, out BoxType boxType);
                        skeletonDataAsset = EntityAnimData.instance.GetBoxAnimData(boxType);

                        var boxBase = go.GetComponent<BoxBase>();
                        boxBase.BoxInit(boxType, skeletonDataAsset, frontTilemap.GetSprite(localPlace), canBorn, canBreak, canKickFly, canBurn, enemyCanKickFly, moveAmount);

                        if (tile.name.Equals("ballbox_01")) go.GetComponent<BallBox>().BallBoxSetup(false, 2);
                        else if (tile.name.Equals("ballbox_03")) go.GetComponent<BallBox>().BallBoxSetup(true, 2);
                        else if (tile.name.Equals("box04_grass_01") || tile.name.Equals("box04_grass_02"))
                        {
                            go.GetComponent<BoxTree>().TreeSetup(false);
                        }
                        else if (tile.name.Equals("box04_grass_05") || tile.name.Equals("box04_grass_06"))
                        {
                            go.GetComponent<BoxTree>().TreeSetup(true);
                        }
                        else if (tile.name.Equals("box01_ground_03") || tile.name.Equals("box01_ground_04"))
                        {
                            go.GetComponent<Box>().BoxSpecialInit(skeletonDataAsset, frontTilemap.GetSprite(localPlace));
                        }

                        allEntities.Add(boxBase);
                        if (canBorn)
                        {
                            //Debug.Log("boxType:" + boxType);
                            totalBoxCanBorn++;
                        }
                        if (canKickFly) totalBoxCanFly++;
                    }
                }
            }
        }
        //Debug.Log("AllGroundKey:" + AllGroundKey.Count + " AllKeyBox:" + AllKeyBox.Count + " TotalKey:" + TotalKey);

        playerLifeCache = playerLife;
        coinCache = GameManager.instance.GameCoin;
        timeCache = GameManager.instance.GameTime;
        x2Cache = GameManager.instance.GameX2;
        coinFromPairsCache = GameManager.instance.coinFromPairs;

        freezeAmountCache = GameUIManager.instance.freezeAmount;
        angryAmountCache = GameUIManager.instance.angryAmount;
        invisibleAmountCache = GameUIManager.instance.invisibleAmount;

        moreFreezeCache = GameUIManager.instance.moreFreeze;
        moreAngryCache = GameUIManager.instance.moreAngry;
        moreInvisibleCache = GameUIManager.instance.moreInvisible;

        if (curMapType == MapType.Puzzle)
        {
            GameMusicStr = SoundName.puzzle_music;
        }
        else
        {
            GameMusicStr = SoundName.combat_music;
            GameManager.instance.GenBalloon();
        }
        AudioManager.instance.PlayMusic(GameMusicStr);

    }

    public void OpenGateInBound()
    {
        var doors = MyGraph.instance.doors;
        var cacheDoorsVector3Int = MyGraph.instance.cacheDoors.Select(a => a.ToVector3Int());
        //Debug.Log("OpenGateInBound ");

        //foreach (var cd in cacheDoorsVector3Int)
        //{
        //    Debug.Log("OpenGateInBound cacheDoorsVector3Int:" + cd);
        //}


        for (int k = 0; k < doors.Count; k++)
        {
            var location = doors.ElementAt(k);
            var localPlace = location.ToVector3Int();
            var doorEntity = location.entityBase.gameObject;
            //Debug.Log("OpenGateInBound:" + bound.min + " max:" + bound.max + " localPlace:" + localPlace);

            if (bound.InBound(localPlace) && !cacheDoorsVector3Int.Contains(localPlace))
            {
                MyGraph.instance.cacheDoors.Add(location.Clone());

                doorEntity.SetActive(false);

                MyGraph.instance.UpdateLocation(localPlace, null);

                var autoMove = MyGraph.instance.autoMoveLocations;
                var checkpoint = autoMove.FirstOrDefault(a => a.X == localPlace.x || a.Y == localPlace.y);
                if (checkpoint != null)
                {
                    GameManager.instance.arrowTf.gameObject.SetActive(true);
                    StartCoroutine(ArrowPath(localPlace));
                }
                //var checkpoint = autoMove.FirstOrDefault(a => a.X == localPlace.x);
                //var normalizedPoint = Vector3.zero;
                //if (checkpoint != null)
                //{
                //    Debug.Log("normalizedPoint:" + normalizedPoint + " checkpoint:" + checkpoint);

                //    if (checkpoint != null && checkpoint.Y > localPlace.y)
                //    {
                //        EffectController.instance.GenEffectPortal(localPlace + new Vector3(0.5f, 0.8f));
                //        normalizedPoint = new Vector3(0, -1);
                //    }
                //    else
                //    {
                //        EffectController.instance.GenEffectPortal(localPlace + new Vector3(0.5f, 0.5f));
                //        normalizedPoint = new Vector3(0, 1);
                //    }
                //}
                //else
                //{
                //    checkpoint = autoMove.FirstOrDefault(a => a.Y == localPlace.y);
                //    var particle = EffectController.instance.GenEffectPortalSide(localPlace + new Vector3(0.5f, 0.5f));
                //    if (checkpoint != null && checkpoint.X > localPlace.x)
                //    {
                //        Debug.Log("normalizedPoint:" + normalizedPoint + " checkpoint 2:" + checkpoint);

                //        var tmp = particle.transform.localScale;
                //        tmp.x = -tmp.x;
                //        particle.transform.localScale = tmp;

                //        normalizedPoint = new Vector3(-1, 0);
                //    }
                //    else normalizedPoint = new Vector3(1, 0);
                //}
                //if (checkpoint != null)
                //{
                //    var angle = GeneralHelper.GetAngleFromVectorFloat(normalizedPoint);
                //    Debug.Log("normalizedPoint:" + normalizedPoint + " angle:" + angle);

                //    GameManager.instance.arrowTf.position = checkpoint.ToVector3();
                //    GameManager.instance.arrowTf.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                //    GameManager.instance.arrowTf.gameObject.SetActive(true);
                //}
            }
        }
        AudioManager.instance.PlaySound(SoundName.open_gate);
    }

    IEnumerator ArrowPath(Vector3Int localPlace)
    {
        bool gened = false;
        while (GameManager.instance.arrowTf.gameObject.activeSelf)
        {
            var autoMove = MyGraph.instance.autoMoveLocations;
            var checkpoint = autoMove.FirstOrDefault(a => a.X == localPlace.x);
            var normalizedPoint = Vector3.zero;
            Vector3 pos = Vector3.zero;

            if (checkpoint != null)
            {
                //Debug.Log("normalizedPoint:" + normalizedPoint + " checkpoint:" + checkpoint);

                if (checkpoint != null && checkpoint.Y > localPlace.y)
                {
                    normalizedPoint = new Vector3(0, -1);
                    pos = new Vector3(Screen.width / 2, 100f);
                    if (!gened)
                    {
                        EffectController.instance.GenEffectPortal(localPlace + new Vector3(0.5f, 0.8f));
                        gened = true;
                    }
                }
                else
                {
                    if (!gened)
                    {
                        EffectController.instance.GenEffectPortal(localPlace + new Vector3(0.5f, 0.5f));
                        gened = true;
                    }
                    normalizedPoint = new Vector3(0, 1);
                    pos = new Vector3(Screen.width / 2, Screen.height - 150f);
                }
            }
            else
            {
                if (!gened)
                {
                    EffectController.instance.GenEffectPortal(localPlace + new Vector3(0.5f, 0.5f));
                    gened = true;
                }

                checkpoint = autoMove.FirstOrDefault(a => a.Y == localPlace.y);
                if (checkpoint != null && checkpoint.X > localPlace.x)
                {
                    normalizedPoint = new Vector3(-1, 0);
                    pos = new Vector3(100f, Screen.height / 2);
                }
                else
                {
                    normalizedPoint = new Vector3(1, 0);
                    pos = new Vector3(Screen.width - 150f, Screen.height / 2);
                }
            }
            if (checkpoint != null)
            {
                var angle = GeneralHelper.GetAngleFromVectorFloat(normalizedPoint);
                //Debug.Log("normalizedPoint:" + normalizedPoint + " angle:" + angle);

                if (CameraController.instance.IsInCamera(checkpoint.ToVector3()))
                {
                    GameManager.instance.arrowTf.position = checkpoint.ToVector3();
                }
                else
                {
                    var tmpPos = Camera.main.ScreenToWorldPoint(pos);
                    tmpPos.z = 0;
                    GameManager.instance.arrowTf.position = tmpPos;
                }
                GameManager.instance.arrowTf.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            }
            yield return null;
        }
    }

    public bool IsInOpenDoor(Vector3 pos)
    {
        var doors = MyGraph.instance.cacheDoors;
        var cell = MyGraph.instance.GetCellFromPosition(pos, out _);
        //Debug.Log("IsInOpenDoor cell:" + cell + " pos:" + pos);

        for (int k = 0; k < doors.Count; k++)
        {
            var location = doors.ElementAt(k);
            var localPlace = location.ToVector3Int();
            var doorEntity = location.entityBase.gameObject;

            if (!doorEntity.activeInHierarchy)
            {
                //Debug.Log("IsInOpenDoor cell2:" + cell + " localPlace:" + localPlace);

                if (cell == localPlace) return true;
            }
        }
        return false;
    }

    public void CloseAllDoor(bool hasRemove)
    {
        AudioManager.instance.PlaySound(SoundName.disappear_in_gate);

        EffectController.instance.DisableAllEffectDoor();

        var doors = MyGraph.instance.cacheDoors;
        for (int k = 0; k < doors.Count; k++)
        {
            var location = doors.ElementAt(k);
            var localPlace = location.ToVector3Int();
            var doorEntity = location.entityBase.gameObject;

            doorEntity.SetActive(true);

            MyGraph.instance.UpdateLocation(localPlace, location.entityBase);
            location.tileType = TileType.Obtacle;
            if (hasRemove)
            {
                doors.RemoveAt(k);
                k--;
            }
            else
            {
                MyGraph.instance.doors.RemoveAll(a => a.ToVector3Int() == localPlace);
            }
        }
        GameManager.instance.arrowTf.gameObject.SetActive(false);
    }

    public void RemoveCurCheckpoints()
    {
        var list = MyGraph.instance.autoMoveLocations.Select(a => a.ToVector3Int());
        GateCheckpoints.RemoveAll(a => list.Contains(a));
        MyGraph.instance.autoMoveLocations.Clear();
    }

    public void RestartCurrentMap()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        return;

        if (GameManager.instance.IsGameOver)
        {
            //MapManager.instance.ResetMovePoints();
            //ObjectPool.instance.DisableAll();
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }

        playerLife = playerLifeCache;
        GameManager.instance.GameCoin = coinCache;
        GameManager.instance.GameTime = timeCache;
        GameManager.instance.GameX2 = x2Cache;
        GameManager.instance.coinFromPairs = coinFromPairsCache;

        totalDiedEnemiesInAllPart = enemyCache;
        totalGenedEnemiesInAllPart = enemyMaxCache;

        GameUIManager.instance.freezeAmount = freezeAmountCache;
        GameUIManager.instance.angryAmount = angryAmountCache;
        GameUIManager.instance.invisibleAmount = invisibleAmountCache;

        ES3.Save(StringDefine.PlayerFreezeItemAmount, freezeAmountCache);
        ES3.Save(StringDefine.PlayerAngryItemAmount, angryAmountCache);
        ES3.Save(StringDefine.PlayerInvisibleItemAmount, invisibleAmountCache);

        GameUIManager.instance.UpdateAll(playerLife, timeCache, coinCache, x2Cache, totalDiedEnemiesInAllPart, totalGenedEnemiesInAllPart,
            angryAmountCache + moreAngryCache, freezeAmountCache + moreFreezeCache, invisibleAmountCache + moreInvisibleCache);

        var tmpBound = new Bound()
        {
            min = bound.min + new Vector3(1, 1),
            max = bound.max + new Vector3(-1, -1)
        };

        //Debug.Log("tmpBound:" + tmpBound.min + " max:" + tmpBound.max);

        // hide all object in curent map
        for (int k = 0; k < allEntities.Count; k++)
        {
            var entity = allEntities[k];
            if (entity.entityType == Assets.Scripts.Define.EntityType.Player)
            {
                entity.ResetMovePoint();
                PlayerController.instance.Remove();
                k--;
                allEntities.Remove(entity);
                ObjectPool.instance.Disable(entity.gameObject);
            }
            else if (tmpBound.InBound(entity.transform.position))
            {
                entity.ResetMovePoint();
                ObjectPool.instance.Disable(entity.gameObject);
                k--;
                allEntities.Remove(entity);
            }
        }
        for (int k = 0; k < GameManager.instance.allItemOnMap.Count; k++)
        {
            var gameObject = GameManager.instance.allItemOnMap[k];
            ObjectPool.instance.Disable(gameObject);
            GameManager.instance.allItemOnMap.Remove(gameObject);
            k--;
        }

        MyGraph.instance.ResetCurrentPartMap(tmpBound);

        GenItemsInBound();
        if(genEnemyCoroutine != null)
        {
            StopCoroutine(genEnemyCoroutine);
            genEnemyCoroutine = null;
        }
        GenEnemies();
        //GenEnemiesInBound();

        CloseAllDoor(true);

        playerPosition = firstPlayerPosition;
        GenPlayer(true);

        GameManager.instance.PlayNow();
    }

    public void SkipCurrentMap()
    {
        if (playerLife <= 0) return;

        if (MapManager.instance.GateCheckpoints.Count <= 1)
        {
            GameManager.instance.GameWin(true, GameEndReason.WinPuzzle);
            return;
        }

        var door = MyGraph.instance.doors.FirstOrDefault(d => bound.InBound(d.ToVector3Int()) && !passedDoors.Contains(d.ToVector3Int()));
        var autoMove = MyGraph.instance.autoMoveLocations.FirstOrDefault(d => bound.InBound(d.ToVector3Int()));
        passedDoors.Add(door.ToVector3Int());

        Vector3 pos = door.ToVector3Int();
        DynamicEntityDirection dir = DynamicEntityDirection.None;
        if (door.X == autoMove.X)
        {
            // portrait
            if (door.Y > autoMove.Y)
            {
                //up                 
                //door.Y + 1;
                pos.y += 1;
                dir = DynamicEntityDirection.Up;
            }
            else
            {
                //down
                pos.y -= 1;
                dir = DynamicEntityDirection.Down;
            }
        }
        else
        {
            // side
            if (door.X > autoMove.X)
            {
                //right
                pos.x += 1;
                dir = DynamicEntityDirection.Right;
            }
            else
            {
                //left
                pos.x -= 1;
                dir = DynamicEntityDirection.Left;
            }
        }
        Debug.Log("Pos:" + pos);
        RemoveCurCheckpoints();

        var tmpPos = pos + new Vector3(0.5f, 0.5f);
        PlayerController.instance.NextPart(tmpPos);
        PlayerController.instance.SetPosition(tmpPos);

        PlayerController.instance.direction = dir;
    }
}
