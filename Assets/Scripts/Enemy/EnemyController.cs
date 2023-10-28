using Assets.Scripts.Common;
using Assets.Scripts.Controller;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Assets.Scripts.Model;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class EnemyController : CharacterBase
{
    public static Dictionary<EnemyType, int> Coins = new Dictionary<EnemyType, int>()
    {
        {EnemyType.Slime, 10 },
        {EnemyType.Goomba, 10 },{EnemyType.KoopaTroopa, 20}, {EnemyType.Koffing, 30},
         {EnemyType.Bowser, 40 },{EnemyType.Rattana, 40}, {EnemyType.Meowth, 50},
          {EnemyType.Zubat, 50 },{EnemyType.Weepinbell, 60}, {EnemyType.FooTheCloud, 60},
           {EnemyType.Boo, 70 },{EnemyType.HuanterGastly, 70}
    };
    [SerializeField] private float aiPoint = 200;
    [SerializeField] private float shootRate = 20;
    [SerializeField] private float detectRange = 3;
    [SerializeField] protected float actionRate = 0;
    [SerializeField] protected int maxLife = 1;
    [SerializeField] protected bool moveThroughOb = false;

    protected bool shouldTurnDirect;
    private float timeIdle;
    private float idleTimer, newDirectTimer;
    protected float chasePlayerTimer;
    protected int life;
    [HideInInspector] public EnemyType enemyType;
    private string actionAnimName;
    protected Coroutine bornCoroutine;
    private bool onChase;
    private float detectRangeSave;

    protected bool tmpDie;

    protected override void Awake()
    {
        base.Awake();
        detectRangeSave = detectRange;
        //idleTimer = Time.time + 1000;
    }

    public virtual void EnemyInit(EnemyType enemyType, BoxBase boxBase)
    {
        base.Init();
        this.enemyType = enemyType;

        tmpDie = false;

        timeIdle = 0.5f;
        entityType = EntityType.Enemy;
        //Debug.Log("State:" + state);
        subStates.Clear();
        life = maxLife;

        if (shootRate > 0) shootRate = 10;
        if (detectRangeSave > 0)
        {
            detectRange = detectRangeSave * 0.4f;
            if (detectRange < 1) detectRange = 1;
        }

        onChase = false;
        idleTimer = Time.time + 1000;

        bornCoroutine = StartCoroutine(Born(boxBase));
    }

    IEnumerator Born(BoxBase boxBase)
    {
        skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 1;

        if (boxBase == null)
        {
            var cell = MyGraph.instance.GetCellFromPosition(transform.position, out bool canMove);
            currentCell = cell;
            MyGraph.instance.UpdateLocation(cell, this);
            timeIdle = 1f;
        }
        else timeIdle = 2f;

        Idle();
        Blink();
        //Debug.Log("idleTimer:" + idleTimer);

        yield return new WaitForSeconds(timeIdle);

        if (boxBase != null && boxBase.transform.gameObject.activeInHierarchy)
        {
            if (boxBase.CanBreak)
            {
                var cell2 = MyGraph.instance.GetCellFromPosition(transform.position, out _);
                var cell1 = MyGraph.instance.GetCellFromPosition(boxBase.transform.position, out _);
                if (cell1 == cell2) boxBase.RemoveOnTileMap();
            }
            else boxBase.HasEnemyAbove = false;
        }

        StopBlink();
        timeIdle = 0.5f;
        if (!moveThroughOb) skeletonAnimation.GetComponent<MeshRenderer>().sortingOrder = 0;
    }

    protected override void HandleCompleteAnim(TrackEntry trackEntry)
    {
        base.HandleCompleteAnim(trackEntry);
        if (trackEntry.Animation.Name.Equals(this.actionAnimName))
        {
            this.actionAnimName = "";
            if (!isDie)
            {
                Idle();
                skeletonAnimation.loop = true;
            }
        }
    }

    protected override void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        base.HandleEvent(trackEntry, e);
        if (e.Data.Name.Equals("action"))
        {
            Action();
        }
    }

    protected override void CanMove(Vector3 tmpPos)
    {
        var cell = MyGraph.instance.GetCellEnemyCanMove(tmpPos, out bool canMove);
        if (canMove)
        {
            var enemies = MapManager.instance.enemies;
            foreach (var enemy in enemies)
            {
                if (enemy.GetInstanceID() != this.GetInstanceID())
                {
                    var otherCell = MyGraph.instance.GetCellEnemyCanMove(enemy.movePointTf.position, out _);
                    if ((cell.x == otherCell.x && Mathf.Abs(cell.y - otherCell.y) <= 0)
                        || (cell.y == otherCell.y && Mathf.Abs(cell.x - otherCell.x) <= 0))
                    {
                        direction = NextDirect(reverseSide: false, out bool hasCellEmpty);
                        Idle(hasCellEmpty ? -1 : 0);
                        return;
                    }
                }
            }

            movePointTf.position = tmpPos;
            state = DynamicEntityState.Walk;
        }
    }

    protected override void Move()
    {
        if (state == DynamicEntityState.Shoot || state == DynamicEntityState.Ice || state == DynamicEntityState.Action) return;

        if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        {
            MoveAI();
        }
    }

    private void MoveAI()
    {
        switch (state)
        {
            case DynamicEntityState.Idle:
                //Debug.Log("idleTimer:" + idleTimer + " Time.time: " + Time.time);
                if (idleTimer <= Time.time)
                {
                    state = DynamicEntityState.Walk;
                    newDirectTimer = Time.time + UnityEngine.Random.Range(3, 5);
                }
                break;
            case DynamicEntityState.Walk:
                bool hasCellEmpty;
                if (SpecialAction())
                {
                    //this.shouldTurnDirect = shouldTurn;
                    //Idle();
                    return;
                }

                if (PlayerController.instance.IsLive() && !PlayerController.instance.InGuard)
                {
                    var distance = GeneralHelper.DistanceXY(transform.position, PlayerController.instance.transform.position);

                    if (distance <= detectRange && UseAI())
                    {
                        if (!onChase)
                        {
                            onChase = true;
                            chasePlayerTimer = Time.time + UnityEngine.Random.Range(0.5f, 1.5f);
                        }
                    }

                    if (chasePlayerTimer > Time.time)
                    {
                        var locations = MyGraph.instance.Search(transform.position, PlayerController.instance.transform.position, useCacheMap: false,
                                through: moveThroughOb);
                        //string locStr = "";
                        //for(int k = 0; k < locations.Count; k++)
                        //{
                        //    locStr += string.Format("({0},{1}) - ", locations[k].X, locations[k].Y);
                        //}
                        //Debug.Log("locations:" + locStr);
                        if (locations.Count > 1)
                        {
                            var firstLoc = locations[1];
                            //Debug.Log("FirstLoc:" + firstLoc.X + " y0:" + firstLoc.Y + " currentCell:" + currentCell.x + " y:" + currentCell.y);
                            if (firstLoc.X == currentCell.x)
                            {
                                if (firstLoc.Y > currentCell.y) direction = DynamicEntityDirection.Up;
                                else direction = DynamicEntityDirection.Down;
                            }
                            else if (firstLoc.Y == currentCell.y)
                            {
                                if (firstLoc.X > currentCell.x) direction = DynamicEntityDirection.Right;
                                else direction = DynamicEntityDirection.Left;
                            }
                        }
                        locations.Clear();
                    }

                    if (distance > detectRange)
                        onChase = false;
                }
                else onChase = false;

                var location = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction]);
                if (location == null)
                {
                    direction = NextDirect(reverseSide: false, out hasCellEmpty);
                    Idle(hasCellEmpty ? -1 : 0);
                    return;
                }

                TileType tileType = location.tileType;
                // check box coming
                if (tileType == TileType.CanMove)
                {
                    Location tmpLocation;
                    if (UseAI())
                    {
                        int k = 2;
                        int cnt = 0;
                        tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * k);
                        while (tmpLocation != null && tmpLocation.tileType != TileType.Obtacle && cnt < Config.LimitWhile)
                        {
                            cnt++;
                            if (cnt == Config.LimitWhile - 1)
                                Debug.LogError("huuuuuu");
                            if (EntityHelper.IsBox(tmpLocation) && tmpLocation.entityBase.IsMoving())
                            {
                                direction = NextDirect(reverseSide: true, out hasCellEmpty);
                                Idle(hasCellEmpty ? -1 : 0);
                                return;
                            }
                            k++;
                            tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * k);
                        }
                    }

                    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * 2);
                    if (EntityHelper.IsEnemy(tmpLocation))
                    {
                        direction = NextDirect(reverseSide: false, out hasCellEmpty);
                        Idle(hasCellEmpty ? -1 : 0);
                        return;
                    }
                }

                if (tileType == TileType.CanMove || EntityHelper.IsEnemy(location) || EntityHelper.IsPlayer(location))
                {
                    if (newDirectTimer < Time.time || (EntityHelper.IsPlayer(location) && PlayerController.instance.InGuard))
                    {
                        direction = NextDirect(reverseSide: false, out hasCellEmpty);
                        Idle(hasCellEmpty ? -1 : 0);
                        return;
                    }

                    MoveByDirect();
                }
                else if (tileType == TileType.Obtacle || EntityHelper.IsBoxKey(location))
                {
                    //Debug.Log("tile obtacle");
                    direction = NextDirect(reverseSide: false, out hasCellEmpty);
                    Idle(hasCellEmpty ? -1 : 0);
                }
                else
                {
                    if (EntityHelper.IsBox(location))
                    {
                        if (!location.entityBase.IsMoving())
                        {
                            if (UseAI())
                            {
                                int k = 2;
                                int cnt = 0;
                                var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * k);
                                while (tmpLocation != null && tmpLocation.tileType != TileType.Obtacle && cnt < Config.LimitWhile)
                                {
                                    cnt++;
                                    if (cnt == Config.LimitWhile - 1)
                                        Debug.LogError("huuuuuu");
                                    if (EntityHelper.IsEnemy(tmpLocation))
                                    {
                                        direction = NextDirect(reverseSide: true, out hasCellEmpty);
                                        Idle(hasCellEmpty ? -1 : 0);
                                        return;
                                    }
                                    k++;
                                    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * k);
                                }

                                if (!EntityHelper.IsEntityType(location, EntityType.BallBox))
                                {
                                    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * 2);
                                    if (tmpLocation.tileType == TileType.CanMove && ShootRate(location.entityBase))
                                    {
                                        Shoot();
                                        if (EntityHelper.IsEntityType(location, EntityType.BombBox))
                                            this.shouldTurnDirect = true;
                                    }
                                    else
                                    {
                                        MoveOrTurnDirect();
                                    }
                                }
                                else
                                {
                                    for (int i = 2; i <= 3; i++)
                                    {
                                        tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direction] * i);
                                        if (tmpLocation.tileType != TileType.CanMove)
                                        {
                                            direction = NextDirect(reverseSide: true, out hasCellEmpty);
                                            Idle(hasCellEmpty ? -1 : 0);
                                            return;
                                        }
                                    }
                                    if (ShootRate(location.entityBase))
                                    {
                                        Shoot();
                                    }
                                    else
                                    {
                                        MoveOrTurnDirect();
                                    }
                                }
                            }
                            else
                            {
                                if (ShootRate(location.entityBase))
                                {
                                    Shoot();
                                }
                                else
                                {
                                    MoveOrTurnDirect();
                                }
                            }
                        }
                        else
                        {
                            direction = NextDirect(reverseSide: false, out hasCellEmpty);
                            Idle(hasCellEmpty ? -1 : 0);
                        }
                    }
                    else if (location.entityBase.entityType == EntityType.Enemy)
                    {
                        direction = NextDirect(reverseSide: true, out hasCellEmpty);
                        Idle(hasCellEmpty ? -1 : 0);
                    }
                }
                break;
        }
    }

    private void MoveOrTurnDirect()
    {
        var rateToMove = 0;
        if (moveThroughOb) rateToMove = 50;
        var rd = UnityEngine.Random.Range(1, 101);
        if (rd <= rateToMove)
        {
            MoveByDirect();
        }
        else
        {
            direction = NextDirect(reverseSide: true, out bool hasCellEmpty);
            Idle(hasCellEmpty ? -1 : 0);
        }
    }

    private void MoveByDirect()
    {
        if (direction == DynamicEntityDirection.Up)
            MoveUp();
        else if (direction == DynamicEntityDirection.Right)
            MoveRight();
        else if (direction == DynamicEntityDirection.Down)
            MoveDown();
        else if (direction == DynamicEntityDirection.Left)
            MoveLeft();
    }

    protected override void UpdateCellMap()
    {
        if (isDie || GameManager.instance.IsGameOver) return;

        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out bool canMove);
        if (canMove)
        {
            //Debug.Log("currentCell 1:" + currentCell);
            if (currentCell != cell)
            {
                var location = MyGraph.instance.GetLocationFromPos(currentCell);
                if (location.entityBase == this || location.tileType == TileType.CanMove)
                {
                    MyGraph.instance.UpdateLocation(currentCell, null);
                    //Debug.Log("currentCell 2:" + currentCell);
                }
                currentCell = cell;
            }

            MyGraph.instance.UpdateLocation(cell, this);
        }
    }

    protected override void Idle(float time = 0)
    {
        base.Idle(time);
        idleTimer = Time.time + (time == 0 ? timeIdle : time);
        if (this.shouldTurnDirect)
        {
            direction = NextDirect(reverseSide: true, out bool hasCellEmpty);
            this.shouldTurnDirect = false;
        }
    }

    protected virtual DynamicEntityDirection NextDirect(bool reverseSide, out bool hasCellEmpty)
    {
        hasCellEmpty = false;
        //Up,
        //Right,
        //Down,
        //Left,
        var directs = new List<DynamicEntityDirection>() { DynamicEntityDirection.Up, DynamicEntityDirection.Right, DynamicEntityDirection.Down, DynamicEntityDirection.Left };
        var list = new Dictionary<DynamicEntityDirection, float>();
        var directHasCellEmpty = new Dictionary<DynamicEntityDirection, bool>();

        for (int k = 0; k < directs.Count; k++)
        {
            var direct = directs[k];
            var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[direct]);
            if (tmpLocation.tileType == TileType.CanMove) list.Add(direct, 0.5f);
            else if (tmpLocation.tileType == TileType.Dynamic)
            {
                list.Add(direct, 0f);
                if (tmpLocation.entityBase != null && tmpLocation.entityBase is Box box)
                {
                    if (box.EnemyCanKickFly || box.CanBreak)
                    {
                        list[direct] = 0.25f;
                    }
                }
                else if (EntityHelper.IsPlayer(tmpLocation))
                    list[direct] = 0.5f;
            }
            else list.Add(direct, 0);

            if (tmpLocation.tileType == TileType.CanMove) directHasCellEmpty[direct] = true;
            else directHasCellEmpty[direct] = false;
            //Debug.Log("tmpLocation:" + tmpLocation.ToVector3Int() + "direct:" + direct + " tile: " + tmpLocation.tileType);
        }
        //var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Left]);
        //if (tmpLocation.tileType != TileType.Obtacle) list.Add(DynamicEntityDirection.Left, 1);
        //else list.Add(DynamicEntityDirection.Left, 0);
        //if (tmpLocation.tileType == TileType.CanMove) directHasCellEmpty[DynamicEntityDirection.Left] = true;
        //Debug.Log("tmpLocation:" + tmpLocation.ToVector3Int() + " tile: " + tmpLocation.tileType);

        //tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Right]);
        //if (tmpLocation.tileType != TileType.Obtacle) list.Add(DynamicEntityDirection.Right, 1);
        //else list.Add(DynamicEntityDirection.Right, 0);
        //if (tmpLocation.tileType == TileType.CanMove) directHasCellEmpty[DynamicEntityDirection.Right] = true;
        //Debug.Log("tmpLocation 1:" + tmpLocation.ToVector3Int() + " tile: " + tmpLocation.tileType);

        //tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Up]);
        //if (tmpLocation.tileType != TileType.Obtacle) list.Add(DynamicEntityDirection.Up, 1);
        //else list.Add(DynamicEntityDirection.Up, 0);
        //if (tmpLocation.tileType == TileType.CanMove) directHasCellEmpty[DynamicEntityDirection.Up] = true;
        //Debug.Log("tmpLocation 2:" + tmpLocation.ToVector3Int() + " tile: " + tmpLocation.tileType);

        //tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Down]);
        //if (tmpLocation.tileType != TileType.Obtacle) list.Add(DynamicEntityDirection.Down, 1);
        //else list.Add(DynamicEntityDirection.Down, 0);
        //if (tmpLocation.tileType == TileType.CanMove) directHasCellEmpty[DynamicEntityDirection.Down] = true;
        //Debug.Log("tmpLocation 3:" + tmpLocation.ToVector3Int() + " tile: " + tmpLocation.tileType);

        //Debug.Log("list.count:" + list.Count);
        var hasValue = list.Where(a => a.Value != 0); ;

        if (hasValue.Count() == 0)
        {
            hasCellEmpty = false;
            return DynamicEntityDirection.None;
        }

        //var per = 1f / hasValue.Count();
        //var keys = list.Keys;
        //for (int k = 0; k < keys.Count; k++)
        //{
        //    var key = keys.ElementAt(k);
        //    if (list[key] != 0)
        //    {
        //        list[key] = per;
        //        if (key == direction) list[key] = per * 0.75f;
        //    }
        //}
        var newDirect = (DynamicEntityDirection)RandomElement(list.Values.ToArray());
        hasCellEmpty = directHasCellEmpty[newDirect];
        //Debug.Log("newDirect:" + newDirect);

        //Up,Right,Down,Left
        //float[] directChange = new float[] { 0.25f, 0.25f, 0.25f, 0.25f };
        //if (direction == DynamicEntityDirection.Up)
        //{
        //    directChange = new float[] { 0f, 0.4f, 0.2f, 0.4f };
        //    var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Left]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0f, 0.5f, 0.5f, 0 };
        //    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Right]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0f, 0, 0.5f, 0.5f };

        //    if (reverseSide) directChange = new float[] { 0f, 0.5f, 0f, 0.5f };
        //}
        //else if (direction == DynamicEntityDirection.Down)
        //{
        //    directChange = new float[] { 0.2f, 0.4f, 0, 0.4f };
        //    var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Left]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0.5f, 0.5f, 0, 0 };
        //    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Right]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0.5f, 0, 0, 0.5f };

        //    if (reverseSide || tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0f, 0.5f, 0f, 0.5f };
        //}
        //else if (direction == DynamicEntityDirection.Right)
        //{
        //    directChange = new float[] { 0.4f, 0, 0.4f, 0.2f };
        //    var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Up]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0f, 0, 0.5f, 0.5f };
        //    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Down]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0.5f, 0, 0, 0.5f };

        //    if (reverseSide || tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0.5f, 0, 0.5f, 0 };
        //}
        //else if (direction == DynamicEntityDirection.Left)
        //{
        //    directChange = new float[] { 0.4f, 0.2f, 0.4f, 0 };
        //    var tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Up]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0f, 0.5f, 0.5f, 0 };
        //    tmpLocation = MyGraph.instance.GetLocationFromPos(transform.position + directPos[DynamicEntityDirection.Down]);
        //    if (tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0.5f, 0.5f, 0, 0 };

        //    if (reverseSide || tmpLocation.tileType == TileType.Obtacle) directChange = new float[] { 0.5f, 0, 0.5f, 0 };
        //}
        //var newDirect = (DynamicEntityDirection)RandomElement(directChange);



        //Debug.Log("otherRange:" + reverseSide + "oldDirect:" + direction + " newDirect:" + newDirect);
        return newDirect;
    }

    protected virtual bool SpecialAction()
    {
        return false;
    }

    protected virtual void Action()
    {

    }

    protected override void ProcessAnim()
    {
        string directAnimName = GetDirectAnimName();

        string nextAnimName = "";
        if (state == DynamicEntityState.Idle)
        {
            nextAnimName = string.Format("{0}_idle", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
        }
        else if (state == DynamicEntityState.Walk)
        {
            nextAnimName = string.Format("{0}_walk", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
        }
        else if (state == DynamicEntityState.Shoot)
        {
            nextAnimName = string.Format("{0}_shoot", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
            this.shootAnimName = nextAnimName;
        }
        else if (state == DynamicEntityState.Action)
        {
            nextAnimName = string.Format("{0}_action", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
            this.actionAnimName = nextAnimName;
        }
        else if (state == DynamicEntityState.Die)
        {
            nextAnimName = string.Format("{0}_idle", directAnimName);
        }

        //Debug.Log(nextAnimName + " directAnimName:" + directAnimName);
        var currentAnim = skeletonAnimation.AnimationName;
        if (currentAnim == nextAnimName || nextAnimName == "") return;
        //Debug.Log("nextAnimName:" + nextAnimName + " directAnimName:" + directAnimName);
        skeletonAnimation.AnimationName = nextAnimName;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
        if (
            (col.CompareTag("Box") || col.CompareTag("BoxBound"))
            && !subStates.Contains(DynamicEntityState.PrepareReborn)
            && !subStates.Contains(DynamicEntityState.Invisible))
        {
            BoxBase boxBase;
            if (col.CompareTag("Box"))
                boxBase = col.gameObject.GetComponent<BoxBase>();
            else boxBase = col.transform.parent.GetComponent<BoxBase>();
            //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + boxBase.IsMoving());

            //var cell1 = MyGraph.instance.GetCellFromPosition(transform.position, out _);
            //var cell2 = MyGraph.instance.GetCellFromPosition(boxBase.transform.position, out _);
            //Debug.Log("cell1:" + cell1.ToString() + " cell2: " + cell2.ToString() + " : " + boxBase.IsMoving()
            //    );
            if (boxBase != null)
            {
                var characterBase = boxBase.GetItemParam.characterBase;
                //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + (characterBase == null));

                if (characterBase == null)
                {
                    if (boxBase is Mario2Bomb)
                    {
                        if (PlayerController.instance.gameObject.activeInHierarchy)
                        {
                            if (isDie || tmpDie) return;
                            Die();
                            if (isDie)
                            {
                                GameManager.instance.UpdateGameCoin(transform.position,
                                Coins[this.enemyType] * GeneralHelper.Pow(2, boxBase.diedEnemyAmount), CoinFrom.KillEnemy);
                                boxBase.diedEnemyAmount++;
                                if (boxBase.diedEnemyAmount == 2) AudioManager.instance.PlaySound(SoundName.kill_combo2);
                                else if (boxBase.diedEnemyAmount == 3) AudioManager.instance.PlaySound(SoundName.kill_combo3);

                                QuestHelper.Action(Assets.Scripts.Model.QuestType.KillEnemy, 1, enemyType);
                            }
                        }
                    }
                }
                else if (boxBase.IsMoving())
                {
                    if (isDie || tmpDie) return;
                    Die();
                    if (isDie)
                    {
                        var charBase = boxBase.GetItemParam.characterBase;
                        if (charBase != null && charBase.entityType == EntityType.Player)
                        {
                            GameManager.instance.UpdateGameCoin(transform.position,
                            Coins[this.enemyType] * GeneralHelper.Pow(2, boxBase.diedEnemyAmount), CoinFrom.KillEnemy);
                            boxBase.diedEnemyAmount++;

                            QuestHelper.Action(Assets.Scripts.Model.QuestType.KillEnemy, 1, enemyType);
                        }
                    }
                }
            }
        }
        else if ((col.CompareTag("BombEffect") || col.CompareTag("FireBallExplode") || col.CompareTag("MarioBullet")
            || col.CompareTag("Mario2BombExplode") || col.CompareTag("SongokuHook") || col.CompareTag("RedImpostorCollider")
            || (col.CompareTag("TrapItem") && col.GetComponent<TrapItem>().fromBallBox))
            && !subStates.Contains(DynamicEntityState.PrepareReborn)
            && !subStates.Contains(DynamicEntityState.Invisible))
        {
            if (isDie || tmpDie) return;
            Die();
            if (isDie)
            {
                if (col.CompareTag("BombEffect"))
                {
                    var bombEffect = col.gameObject.GetComponent<BombEffect>();
                    if (bombEffect.bombEffectParam.characterBase != null && bombEffect.bombEffectParam.characterBase.entityType == EntityType.Player)
                    {
                        GameManager.instance.UpdateGameCoin(transform.position,
                            Coins[this.enemyType] * GeneralHelper.Pow(2, bombEffect.bombEffectParam.diedEnemyAmount), CoinFrom.KillEnemy);
                        bombEffect.bombEffectParam.diedEnemyAmount++;
                        QuestHelper.Action(Assets.Scripts.Model.QuestType.KillEnemy, 1, enemyType);
                    }
                }
                else
                {
                    GameManager.instance.UpdateGameCoin(transform.position, Coins[this.enemyType], CoinFrom.KillEnemy);
                }
            }
        }
        else if (col.CompareTag("Player")
            && !subStates.Contains(DynamicEntityState.Invisible))
        {
            if (PlayerController.instance.IsLive())
            {
                if (PlayerController.instance.IsAngry)
                {
                    if (isDie || tmpDie) return;
                    Die();
                }
            }
        }
        //else if (col.CompareTag("BongBong") || col.CompareTag("ItemOnMap"))
        //{
        //    ProcessEatItem(col.gameObject, col.CompareTag("BongBong") ? 1 : 2);
        //}
        else if (col.CompareTag("Hook") && !subStates.Contains(DynamicEntityState.PrepareReborn)
            && !subStates.Contains(DynamicEntityState.Invisible))
        {
            Frozen(1f, false);
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isDie || tmpDie) return;

        if (
           (col.CompareTag("Box") || col.CompareTag("BoxBound"))
           && !subStates.Contains(DynamicEntityState.PrepareReborn)
           && !subStates.Contains(DynamicEntityState.Invisible))
        {
            BoxBase boxBase;
            if (col.CompareTag("Box")) boxBase = col.gameObject.GetComponent<BoxBase>();
            else boxBase = col.transform.parent.GetComponent<BoxBase>();

            //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + boxBase.IsMoving());

            //var cell1 = MyGraph.instance.GetCellFromPosition(transform.position, out _);
            //var cell2 = MyGraph.instance.GetCellFromPosition(boxBase.transform.position, out _);
            //Debug.Log("cell1:" + cell1.ToString() + " cell2: " + cell2.ToString() + " : " + boxBase.IsMoving()
            //    );
            if (boxBase != null)
            {
                var characterBase = boxBase.GetItemParam.characterBase;
                //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + (characterBase == null));

                if (characterBase == null)
                {
                    if (boxBase is Mario2Bomb)
                    {
                        if (PlayerController.instance.gameObject.activeInHierarchy)
                        {
                            if (isDie || tmpDie) return;
                            Die();
                            if (isDie)
                            {
                                GameManager.instance.UpdateGameCoin(transform.position,
                                Coins[this.enemyType] * GeneralHelper.Pow(2, boxBase.diedEnemyAmount), CoinFrom.KillEnemy);
                                boxBase.diedEnemyAmount++;
                                if (boxBase.diedEnemyAmount == 2) AudioManager.instance.PlaySound(SoundName.kill_combo2);
                                else if (boxBase.diedEnemyAmount == 3) AudioManager.instance.PlaySound(SoundName.kill_combo3);
                                QuestHelper.Action(Assets.Scripts.Model.QuestType.KillEnemy, 1, enemyType);
                            }
                        }
                    }
                }
                else if (boxBase.IsMoving())
                {
                    if (isDie || tmpDie) return;
                    Die();
                    if (isDie)
                    {
                        var charBase = boxBase.GetItemParam.characterBase;
                        if (charBase != null && charBase.entityType == EntityType.Player)
                        {
                            GameManager.instance.UpdateGameCoin(transform.position,
                            Coins[this.enemyType] * GeneralHelper.Pow(2, boxBase.diedEnemyAmount), CoinFrom.KillEnemy);
                            boxBase.diedEnemyAmount++;

                            QuestHelper.Action(Assets.Scripts.Model.QuestType.KillEnemy, 1, enemyType);
                        }
                    }
                }
            }
        }
    }
    protected int RandomElement(float[] elements)
    {
        var total = 0f;
        foreach (var element in elements)
        {
            total += element;
        }

        var randomPoint = UnityEngine.Random.value * total;

        for (int k = 0; k < elements.Length; k++)
        {
            if (randomPoint < elements[k])
            {
                return k;
            }
            else
            {
                randomPoint -= elements[k];
            }
        }
        return elements.Length - 1;
    }

    private bool UseAI()
    {
        var per = aiPoint / 200;
        var rd = UnityEngine.Random.Range(0, 1f);
        return rd <= per;
    }

    private bool ShootRate(EntityBase entityBase)
    {
        if (GameManager.instance.IsGameOver)
            return false;
        if (entityBase is BoxBase box)
        {
            if (box == null || (box != null && !box.EnemyCanKickFly && !box.CanBreak)) return false;
        }

        var per = shootRate / 100;
        var rd = UnityEngine.Random.Range(0, 1f);
        //Debug.Log("ShootRate:" + per + " rd:" + rd);
        return rd <= per;
    }

    protected override void Die()
    {
        if (tmpDie) return;
        tmpDie = true;

        if (invisibleEffects.Count > 0)
        {
            for (int k = 0; k < invisibleEffects.Count; k++)
            {
                var ang = invisibleEffects[k];
                invisibleEffects.RemoveAt(k);
                ObjectPool.instance.Disable(ang.gameObject);
                k--;
            }
        }
        subStates.Clear();

        this.life--;
        if (this.life <= 0)
        {
            base.Die();
            AudioManager.instance.PlaySound(SoundName.enemy_die);

            MyGraph.instance.UpdateLocation(currentCell, null);
            StartCoroutine(DieIE());
        }
        else
        {
            if (!subStates.Contains(DynamicEntityState.PrepareReborn))
                subStates.Add(DynamicEntityState.PrepareReborn);

            StartCoroutine(ReBorn());
        }
    }

    protected override IEnumerator DieIE()
    {
        yield return null;

        var cell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        var pos = cell + new Vector3(0.5f, 0.8f);
        pos.z = pos.y;
        EffectController.instance.GenDieEffect(pos);

        state = DynamicEntityState.Die;
        skeletonAnimation.loop = false;

        ObjectPool.instance.Disable(gameObject);
        this.PostEvent(entityType == EntityType.Enemy ? EventID.EnemyDie : EventID.PlayerDie, this);
    }

    protected IEnumerator ReBorn()
    {
        Blink();
        yield return new WaitForSeconds(2f);
        StopBlink();
        tmpDie = false;

        subStates.Remove(DynamicEntityState.PrepareReborn);
    }

    public bool CanTakeDamage()
    {
        //Debug.Log("CanTakeDamage isDie:" + (isDie = false) + " prepare:" + (subState != DynamicEntityState.PrepareReborn) + " wtf:" + (isDie = false && subState != DynamicEntityState.PrepareReborn));
        return (isDie == false);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }

    public override void Killed()
    {
        if (tmpDie) return;
        tmpDie = true;

        if (invisibleEffects.Count > 0)
        {
            for (int k = 0; k < invisibleEffects.Count; k++)
            {
                var ang = invisibleEffects[k];
                invisibleEffects.RemoveAt(k);
                ObjectPool.instance.Disable(ang.gameObject);
                k--;
            }
        }
        subStates.Clear();

        base.Die();
        AudioManager.instance.PlaySound(SoundName.enemy_die);
        MyGraph.instance.UpdateLocation(currentCell, null);
        StartCoroutine(DieIE());

        //base.Killed();
        //GameManager.instance.UpdateGameCoin(transform.position,
        //                       Coins[this.enemyType] * GeneralHelper.Pow(2, 0), CoinFrom.KillEnemy);
        //QuestHelper.Action(Assets.Scripts.Model.QuestType.KillEnemy, 1, enemyType);
    }
}
