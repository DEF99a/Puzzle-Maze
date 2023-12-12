using Assets.Scripts.Common;
using Assets.Scripts.Controller;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState
{
    Stop,
    Playing,
    PrepareEnd,
    Win,
    Loose
}

public enum GameEndReason
{
    Stop,
    PlayerOutLife,
    OutBox,
    AllEnemyDie,
    WinPuzzle
}

public enum CoinFrom
{
    KillEnemy,
    EatBalloon,
    RemoveBox,
    BonusTime
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private float timeScale;
    public Transform arrowTf;

    private GameState gameState, prevGameState;
    public bool IsPlaying => gameState == GameState.Playing;
    public bool IsGameOver => gameState == GameState.Win || gameState == GameState.Loose;
    public bool IsGameWin => gameState == GameState.Win;
    public bool IsGameLoose => gameState == GameState.Loose;
    public bool IsStop => gameState == GameState.Stop;
    public void Stop() { gameState = GameState.Stop; }

    public int GameTime { get; set; }
    public int GameCoin { get; set; }
    public int GameTimeToGetStar { get; set; }

    [HideInInspector] public Dictionary<CoinFrom, ValueTuple<int, int>> coinFromPairs = new Dictionary<CoinFrom, ValueTuple<int, int>>();
    [HideInInspector] public int useLifeAmount;

    public int GameX2 { get; set; }

    private int eatGameX2Amount;
    private GameEndReason gameEndReason;
    private Coroutine genBalloonCoroutine;
    public List<GameObject> allItemOnMap { get; private set; }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartGame();
    }

    private void StartGame()
    {
        Time.timeScale = timeScale;

        StartCoroutine(StartGameIE());

        IEnumerator StartGameIE()
        {
            AudioManager.instance.StopMusic(SoundName.ground_music);

            allItemOnMap = new List<GameObject>();

            coinFromPairs[CoinFrom.BonusTime] = (0, 0);
            coinFromPairs[CoinFrom.EatBalloon] = (0, 0);
            coinFromPairs[CoinFrom.KillEnemy] = (0, 0);
            coinFromPairs[CoinFrom.RemoveBox] = (0, 0);
            GameTime = 0;
            GameX2 = 1;

            IsShowHint = false;
            ObjectPool.instance.DisableAll();
            MapManager.instance.LoadMap();
            MapManager.instance.GenItemsInBound();

            GameTime = Level.instance.totalTime;
            GameTimeToGetStar = Level.instance.timeToGetStar;

            GameUIManager.instance.UpdateTime(GameTime);
            GameUIManager.instance.UpdateX2(GameX2);
            GameUIManager.instance.UpdateCoin(GameCoin);
            GameUIManager.instance.ShowLevelNotice();

            yield return new WaitForSeconds(1.2f);

            PlayGame();

            FireBaseManager.instance.LogPlayLevel(MapManager.instance.CurLevelNumber);

            var isJoystick = ES3.Load(StringDefine.IsJoystick, 0);
            if (isJoystick == 1)
                FireBaseManager.instance.LogJoystickMode(MapManager.instance.CurLevelNumber);
            else
                FireBaseManager.instance.LogGamepadMode(MapManager.instance.CurLevelNumber);
        }
    }


    private void PlayGame()
    {
        gameState = GameState.Playing;

        MapManager.instance?.InitPlayer();
        MapManager.instance?.GenEnemies();

        StartCoroutine(RunTime());

        GameUIManager.instance.ShowHideLeftRect(MapManager.instance.curMapType == MapType.Puzzle);
        //GameManager.instance.GenBalloon();

        GameUIManager.instance.ShowController();
        TutorialController.Instance?.Setup();
    }


    public void GenBalloon()
    {
        if (MapManager.instance.curMapType == MapType.Enemy)
        {
            if (genBalloonCoroutine == null)
                genBalloonCoroutine = StartCoroutine(GenBalloonIE());
        }
        else
        {
            StopGenBalloon();
        }
    }

    public void StopGenBalloon()
    {
        if (genBalloonCoroutine != null)
        {
            StopCoroutine(genBalloonCoroutine);
            genBalloonCoroutine = null;
        }
    }

    IEnumerator GenBalloonIE()
    {
        //ItemLife = 20,//ItemAngry,//ItemInvisible,//ItemFreeze,//ItemTimer5,
        //ItemTimer10,//ItemTimer20,//ItemCoin100,//ItemCoin200,
        //ItemCoin500,//ItemCoin1000,//ItemCoin2000,//ItemCoinRewardEndGameX2,
        //ItemCoinRewardEndGameX3,//ItemSpeed,//ItemGun,//ItemKillAll
        Dictionary<EntityType, float> percent = new Dictionary<EntityType, float>()
        {
            { EntityType.ItemLife, 5},{ EntityType.ItemAngry, 5},{ EntityType.ItemInvisible, 5},
            { EntityType.ItemFreeze, 5},{ EntityType.ItemTimer5, 10},{ EntityType.ItemTimer10, 10},
            { EntityType.ItemTimer20, 5},{ EntityType.ItemCoin100, 10},{ EntityType.ItemCoin200, 10},
            { EntityType.ItemCoin500, 10},{ EntityType.ItemCoin1000, 5},{ EntityType.ItemCoin2000, 5},
            { EntityType.ItemCoinRewardEndGameX2, 5},{ EntityType.ItemCoinRewardEndGameX3, 5},{ EntityType.ItemSpeed, 5},
            { EntityType.ItemGun, 0},{ EntityType.ItemKillAll, 0}
        };
        //Dictionary<EntityType, float> percent = new Dictionary<EntityType, float>()
        //{
        //    { EntityType.ItemSpeed, 100},
        //    { EntityType.ItemGun, 0},{ EntityType.ItemKillAll, 0}
        //};
        List<EntityType> list = new List<EntityType>();
        foreach (var dt in percent)
        {
            for (int k = 0; k < dt.Value; k++)
                list.Add(dt.Key);
        }

        var countX2 = 0;
        while (IsPlaying)
        {
            yield return new WaitForSeconds(20);

            var minX = CameraController.instance.transform.position.x - Camera.main.orthographicSize;
            var maxX = CameraController.instance.transform.position.x + Camera.main.orthographicSize;
            var minY = CameraController.instance.min.y - 1;

            minX = minX < MapManager.instance.GetMinMaxStoneMap().min.x ? MapManager.instance.GetMinMaxStoneMap().min.x : minX;
            maxX = maxX < MapManager.instance.GetMinMaxStoneMap().max.x ? maxX : MapManager.instance.GetMinMaxStoneMap().max.x;

            var x = UnityEngine.Random.Range(minX + 1.5f, maxX - 1.5f);
            var pos = MyGraph.instance.GetCellFromPosition(new Vector3(x, minY), out _);
            var go = ObjectPool.instance.GetGameObject(PrefabCache.instance.balloonPf, pos, Quaternion.identity);

            EntityType itemType;
            bool genAgain = false;
            int cnt = 0;
            do
            {
                cnt++;

                itemType = list[UnityEngine.Random.Range(0, list.Count)];
                if (itemType == EntityType.ItemCoinRewardEndGameX2 || itemType == EntityType.ItemCoinRewardEndGameX3)
                {
                    countX2++;
                    if (countX2 > 2) genAgain = true;
                }
            } while (genAgain && cnt < Config.LimitWhile);
            if (cnt == Config.LimitWhile) itemType = EntityType.ItemCoin100;

            go.GetComponent<Balloon>().Init(pos, itemType);
        }

        genBalloonCoroutine = null;
    }

    public void GenItemOnMap(Vector3 pos)
    {
        Dictionary<EntityType, float> percent = new Dictionary<EntityType, float>()
        {
            { EntityType.ItemLife, 0},{ EntityType.ItemAngry, 5},{ EntityType.ItemInvisible, 5},
            { EntityType.ItemFreeze, 5},{ EntityType.ItemTimer5, 10},{ EntityType.ItemTimer10, 10},
            { EntityType.ItemTimer20, 5},{ EntityType.ItemCoin100, 10},{ EntityType.ItemCoin200, 10},
            { EntityType.ItemCoin500, 10},{ EntityType.ItemCoin1000, 5},{ EntityType.ItemCoin2000, 5},
            { EntityType.ItemCoinRewardEndGameX2, 0},{ EntityType.ItemCoinRewardEndGameX3, 0},{ EntityType.ItemSpeed, 5},
            { EntityType.ItemGun, 0},{ EntityType.ItemKillAll, 0}
        };


        List<EntityType> list = new List<EntityType>();
        foreach (var dt in percent)
        {
            for (int k = 0; k < dt.Value; k++)
                list.Add(dt.Key);
        }
        var itemType = list[UnityEngine.Random.Range(0, list.Count)];
        var go = ObjectPool.instance.GetGameObject(PrefabCache.instance.itemOnmapPf, pos, Quaternion.identity);
        go.GetComponent<ItemOnMap>().Init(itemType);
        allItemOnMap.Add(go);
    }

    IEnumerator RunTime()
    {
        while (gameState != GameState.Stop)
        {
            yield return new WaitForSeconds(1f);
            if (GameTime >= 0)
            {
                GameTime += 1;
                //if(GameTime <= 10)
                //    AudioManager.instance?.PlaySound(SoundName.clock_count_ingame);
                GameUIManager.instance.UpdateTime(GameTime);
            }
        }
    }

    public void Continue(int life)
    {
        useLifeAmount++;// so lan su dung mang de hoi sinh

        if (prevGameState == GameState.Win)
        {
            gameState = GameState.Win;
        }
        else gameState = GameState.Playing;

        MapManager.instance.Continue(life);

        GenBalloon();
        StartCoroutine(RunTime());
    }

    public void PlayerEatItem(EntityType entityType)
    {
        switch (entityType)
        {
            case EntityType.ItemLife:
                MapManager.instance.playerLife += 1;
                GameUIManager.instance.UpdateLife(MapManager.instance.playerLife);
                break;
            case EntityType.ItemCoin100:
                UpdateCoin(10, CoinFrom.EatBalloon);
                break;
            case EntityType.ItemCoin200:
                UpdateCoin(20, CoinFrom.EatBalloon);
                break;
            case EntityType.ItemCoin500:
                UpdateCoin(50, CoinFrom.EatBalloon);
                break;
            case EntityType.ItemCoin1000:
                UpdateCoin(100, CoinFrom.EatBalloon);
                break;
            case EntityType.ItemCoin2000:
                UpdateCoin(200, CoinFrom.EatBalloon);
                break;
            case EntityType.ItemTimer5:
                this.GameTime += 5;
                GameUIManager.instance.UpdateTime(GameTime);
                break;
            case EntityType.ItemTimer10:
                this.GameTime += 10;
                GameUIManager.instance.UpdateTime(GameTime);
                break;
            case EntityType.ItemTimer20:
                this.GameTime += 20;
                GameUIManager.instance.UpdateTime(GameTime);
                break;
            case EntityType.ItemCoinRewardEndGameX2:
                this.GameX2 *= 2;
                this.eatGameX2Amount++;
                GameUIManager.instance.UpdateX2(GameX2);
                break;
            case EntityType.ItemCoinRewardEndGameX3:
                this.GameX2 *= 3;
                this.eatGameX2Amount++;
                GameUIManager.instance.UpdateX2(GameX2);
                break;
        }
    }

    public void UpdateGameCoin(Vector3 pos, int coinAdd, CoinFrom coinFrom)
    {
        var go = ObjectPool.instance.GetGameObject(PrefabCache.instance.itemSpritePf, pos, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = EntitySprites.instance.GetSprite(EntityType.ItemCoin);
        go.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = coinAdd.ToString();
        GameUIManager.instance.MoveToCoin(go.transform, () =>
        {
            UpdateCoin(coinAdd, coinFrom);
        });
        var go2 = ObjectPool.instance.GetGameObject(PrefabCache.instance.coinFlyUp, pos, Quaternion.identity);
        go2.GetComponent<CoinFlyUp>().Setup(coinAdd);
    }

    private void UpdateCoin(int coinAdd, CoinFrom coinFrom)
    {
        this.GameCoin += coinAdd;
        if (coinFromPairs.ContainsKey(coinFrom))
        {
            var oldValue = coinFromPairs[coinFrom];
            coinFromPairs[coinFrom] = (oldValue.Item1 + 1, oldValue.Item2 + coinAdd);
        }
        else coinFromPairs.Add(coinFrom, (1, coinAdd));

        QuestHelper.Action(Assets.Scripts.Model.QuestType.CollectGold, coinAdd);

        GameUIManager.instance.UpdateCoin(GameCoin);
    }

    //private void Update()
    //{
    //Time.timeScale = timeScale;
    //if (Input.GetKeyUp(KeyCode.UpArrow))
    //{
    //    MapManager.instance.RestartCurrentMap();
    //}
    //}

    public void GameOver(bool isWin, GameEndReason _gameEndReason)
    {
        if (IsGameOver && isWin) return;

        gameState = isWin ? GameState.Win : GameState.Loose;
        gameEndReason = _gameEndReason;

        StopGenBalloon();

        if (isWin) prevGameState = GameState.Win;

        if (!isWin) StartCoroutine(GameOverIE(isWin, gameEndReason));
        else MapManager.instance.OpenGateInBound();
    }

    private IEnumerator GameOverIE(bool isWin, GameEndReason gameEndReason, float delay = 1.5f)
    {
        int cnt = 0;
        while (GeneralHelper.moveCount != 0 && cnt < Config.LimitWhile)
        {
            cnt++;
            yield return null;
        }
        yield return new WaitForSeconds(delay);

        if (GameTime > 0)
        {
            int coin = 1000 / GameTime;

            if (this.GameX2 > 0)
                coin = this.GameX2 * coin;
            coinFromPairs[CoinFrom.BonusTime] = (GameTime, coin);
        }

        PlayerType playerType = Config.SelectPlayerType;
        if (playerType == PlayerType.None) playerType = PlayerHelper.GetCurrentPlayer().playerType;
        if (PlayerHelper.GetPlayerLife(playerType) <= MapManager.instance.playerLife && isWin)
            QuestHelper.Action(Assets.Scripts.Model.QuestType.PassGameNoDie, 1);

        GameUIManager.instance.EndGame(isWin, gameEndReason);
    }

    public void GameWin(bool isWin, GameEndReason _gameEndReason)
    {
        if (IsGameOver) return;

        StopGenBalloon();

        gameState = isWin ? GameState.Win : GameState.Loose;
        gameEndReason = _gameEndReason;

        StartCoroutine(GameOverIE(true, gameEndReason, 0.5f));
    }

    public void ShowGameWinUI()
    {
        StartCoroutine(GameOverIE(true, gameEndReason, 1));
    }

    public void PlayNow()
    {
        gameState = GameState.Playing;
    }

    public bool IsShowHint;
    public void StartHint()
    {
        if (IsGameWin) return;
        IsShowHint = true;
        CurrentHint = 0;
        if (rollBackSteps.Count > 0)
        {
            MapManager.instance.ResetMovePoints();
            ObjectPool.instance.DisableAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            CreateRollBack();
            Invoke("ShowHint", 2);
        }
        else
        {
            ShowHintPoint(Level.instance.Hint.Hints[CurrentHint]);
        }
    }

    private void ShowHint()
    {
        ShowHintPoint(Level.instance.Hint.Hints[CurrentHint]);
    }

    private int CurrentHint = 0;
    public void EndHint(TypeHint type)
    {
        if (type != Level.instance.Hint.Hints[CurrentHint])
        {
            IsShowHint = false;
            GameUIManager.instance.HideAllTut();
        }
        else
        {
            CurrentHint++;
            if (CurrentHint < Level.instance.Hint.Hints.Count) ShowHintPoint(Level.instance.Hint.Hints[CurrentHint]);
            else
            {
                IsShowHint = false;
                GameUIManager.instance.HideAllTut();
            }
        }
    }

    public bool CheckNextHintIsAttack()
    {
        if (CurrentHint >= Level.instance.Hint.Hints.Count) return false;
        return Level.instance.Hint.Hints[CurrentHint] == TypeHint.Attack;
    }

    private void ShowHintPoint(TypeHint type)
    {
        GameUIManager.instance.ShowTut(type);
    }

    public void RollBack()
    {
        if (IsGameWin) return;
        if (rollBackSteps.Count > 0)
        {
            PlayerController.instance.RollBack(rollBackSteps[rollBackSteps.Count - 1]);
            rollBackSteps.RemoveAt(rollBackSteps.Count - 1);
        }
    }

    private List<RollBackData> rollBackSteps = new List<RollBackData>();
    public void CreateRollBack()
    {
        rollBackSteps.Clear();
    }
    public void AddRollBack(RollBackData rollBack)
    {
        if (rollBack.IsMovement)
        {
            DynamicEntityDirection direction = DynamicEntityDirection.Down;
            for (int i = rollBackSteps.Count - 1; i >= 0; i--)
            {
                if (rollBackSteps[i].IsMovement)
                {
                    direction = rollBackSteps[i].Movement.newDirection;
                    break;
                }
            }
            rollBack.Movement.oldDirection = direction;
        }
        rollBackSteps.Add(rollBack);
    }
}

[Serializable]
public class LevelHintData
{
    public List<TypeHint> Hints;
}

public enum TypeHint
{
    Right,
    Left,
    Up,
    Down,
    Attack
}

[Serializable]
public class RollBackData
{
    public bool IsMovement;
    public RollBackMovementData Movement;
    public RollBackAttackData Attack;
}

[Serializable]
public class RollBackMovementData
{
    public Vector3 oldPos;
    public DynamicEntityDirection oldDirection;
    public Vector3 newPos;
    public DynamicEntityDirection newDirection;
}

[Serializable]
public class RollBackAttackData
{
    public BoxKey1 box;
    public DynamicEntityDirection oldDirection;
}