using Assets.Scripts.Controller;
using Assets.Scripts.Define;
using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using System;

public partial class PlayerController : CharacterBase
{
    public static PlayerController instance;
    private float timeGuard = 2f;

    private float guardTimer;
    private DynamicEntityDirection directionByButton, oldDirectionByButton;

    public bool InGuard => guardTimer > Time.time;
    public bool IsAngry => subStates.Contains(DynamicEntityState.Angry);

    protected string actionAnimName;

    private int countAutoMove;
    protected DynamicEntityState autoMoveState;
    private bool endPartOfMap;

    protected override void Awake()
    {
        base.Awake();
        angryEffects = new List<ParticleSystem>();
        this.RegisterListener(EventID.EndPartOfMap, EndPartOfMap);
    }

    private void EndPartOfMap(object obj)
    {
        endPartOfMap = true;
    }

    public void Init(float realMoveSpeed, bool firstInit)
    {
        instance = this;

        moveSpeed = realMoveSpeed;

        base.Init();
        skeletonAnimation.skeleton.A = 1;

        autoMoveState = DynamicEntityState.None;
        countAutoMove = 0;
        if (firstInit) endPartOfMap = false;

        entityType = EntityType.Player;
        guardTimer = Time.time + timeGuard;
        directionByButton = DynamicEntityDirection.None;

        allEffectSpeed.Clear();
        angryEffects.Clear();

        if (!firstInit) StartCoroutine(CheckingGuard());

        var localScale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 1);
        transform.DOScale(localScale, 0.5f);
    }

    protected override void HandleEvent(TrackEntry trackEntry, Spine.Event e)
    {
        base.HandleEvent(trackEntry, e);
        if (e.Data.Name.Equals("action"))
        {
            Action();
        }
    }

    IEnumerator CheckingGuard()
    {
        AudioManager.instance.PlaySound(SoundName.player_appear);
        Blink();
        while (InGuard)
        {
            yield return null;
        }
        StopBlink();
    }

    protected override void Update()
    {
        base.Update();
        if (isDie) return;
        //if (GameManager.instance.IsGameOver) return;

        if (angryEffects.Count > 0)
        {
            for (int k = 0; k < angryEffects.Count; k++)
            {
                var ang = angryEffects[k];
                if (ang.isStopped)
                {
                    angryEffects.RemoveAt(k);
                    k--;
                    ObjectPool.instance.Disable(ang.gameObject);
                }
                else ang.transform.position = transform.position;
            }
            if (angryEffects.Count() == 0)
                subStates.Remove(DynamicEntityState.Angry);
        }
    }

    protected override void Move()
    {
        if (state == DynamicEntityState.Shoot || state == DynamicEntityState.Ice || GameManager.instance.IsStop) return;
        if (state == DynamicEntityState.Action) return;

        var distance = Vector3.Distance(transform.position, movePointTf.position);

        //if (directionByButton != oldDirectionByButton &&
        //    directionByButton != DynamicEntityDirection.None
        //    && oldDirectionByButton != DynamicEntityDirection.None)
        //{
        //    if (directPos[oldDirectionByButton].x != directPos[directionByButton].x &&
        //        directPos[oldDirectionByButton].y != directPos[directionByButton].y)
        //    {
        //        Vector3 nextMovePoint;
        //        if (distance <= 0.5f)
        //        {
        //            //Debug.Log("ch1 directionByButton: " + directionByButton + " oldDirectionByButton:" + oldDirectionByButton);
        //            nextMovePoint = movePointTf.position + directPos[directionByButton];
        //        }
        //        else
        //        {
        //            //Debug.Log("ch2 directionByButton: " + directionByButton + " oldDirectionByButton:" + oldDirectionByButton);
        //            nextMovePoint = oldMovePointPos + directPos[directionByButton];
        //        }
        //        MyGraph.instance.GetCellPlayerCanMove(nextMovePoint, out bool canMove);
        //        if (canMove)
        //        {
        //            movePointTf.position = nextMovePoint;
        //            direction = directionByButton;
        //        }
        //    }
        //}

        if (distance <= 0.01f)
        {
            if (MapManager.instance.CurLevelNumber == 1)
            {
                if (TutorialController.Instance != null)
                {
                    var curCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
                    TutorialController.Instance.ShowTutMove(curCell);
                }
            }

            if ((GameManager.instance.IsGameWin || endPartOfMap) && autoMoveState != DynamicEntityState.AutoMove)
            {
                var curCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
                var exist = MyGraph.instance.autoMoveLocations.Exists(a => a.ToVector3Int() == curCell);
                if (exist)
                {

                    MapManager.instance.RemoveCurCheckpoints();
                    autoMoveState = DynamicEntityState.AutoMove;
                    AutoMove(curCell);
                }
            }
            if (autoMoveState == DynamicEntityState.AutoMove)
            {
                countAutoMove++;
                if (MapManager.instance.GateCheckpoints.Count <= 0)
                {
                    if (countAutoMove == 1)
                    {
                        StartCoroutine(Blur());
                    }
                    else if (countAutoMove == 2)
                    {
                        AudioManager.instance?.PlaySound(SoundName.disappear_in_gate);
                        EffectController.instance.GenEffectReborn(transform.position);
                        GameManager.instance.ShowGameWinUI();
                    }
                    if (countAutoMove >= 2)
                    {
                        //directionByButton = DynamicEntityDirection.None;
                        return;
                    }
                }
                else
                {
                    if (countAutoMove == 3)
                    {
                        // next part
                        NextPart(transform.position);
                    }
                    if (countAutoMove >= 3) return;
                }
            }

            MoveByButton();
            if (directionByButton == DynamicEntityDirection.None)
            {
                if (Input.GetKey(KeyCode.W))
                    MoveUp();
                else if (Input.GetKey(KeyCode.D))
                    MoveRight();
                else if (Input.GetKey(KeyCode.S))
                    MoveDown();
                else if (Input.GetKey(KeyCode.A))
                    MoveLeft();
                else MoveStop();
            }
        }
        else
        {
            if (state == DynamicEntityState.Walk)
                AudioManager.instance.PlaySound(SoundName.player_walk_common);
        }
        oldDirectionByButton = directionByButton;
    }

    private void MoveByButton()
    {
        if (directionByButton == DynamicEntityDirection.Up)
            MoveUp();
        else if (directionByButton == DynamicEntityDirection.Down)
            MoveDown();
        else if (directionByButton == DynamicEntityDirection.Left)
            MoveLeft();
        else if (directionByButton == DynamicEntityDirection.Right)
            MoveRight();
        else
            MoveStop();
    }

    public void SetDirectionButton(DynamicEntityDirection dynamicEntityDirection)
    {
        if (isDie || autoMoveState == DynamicEntityState.AutoMove)
        {
            if (dynamicEntityDirection != DynamicEntityDirection.None) return;
        }

        directionByButton = dynamicEntityDirection;
    }

    protected override void CanMove(Vector3 tmpPos)
    {
        MyGraph.instance.GetCellPlayerCanMove(tmpPos, out bool canMove);
        if (canMove)
        {
            oldMovePointPos = movePointTf.position;
            movePointTf.position = tmpPos;
            state = DynamicEntityState.Walk;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //if (subState == DynamicEntityState.Invisible) return;

        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + col.tag);
        if (col.CompareTag("Box") && !subStates.Contains(DynamicEntityState.Invisible))
        {
            var itemBase = col.gameObject.GetComponent<BoxBase>();
            if (itemBase != null && itemBase.IsMoving())
                if (!InGuard) Die();
            //EditorApplication.isPaused = true;
        }
        else if (!subStates.Contains(DynamicEntityState.Invisible) && (col.CompareTag("Enemy") || col.CompareTag("BombEffect")
            || col.CompareTag("FireBallExplode") || col.CompareTag("TrapItem") || col.CompareTag("Mario2BombExplode")))
        {
            //Debug.Log("guardTimer < Time.time:" + (guardTimer < Time.time) + "guardTimer:" + guardTimer);
            if (col.CompareTag("Enemy"))
            {
                if (subStates.Contains(DynamicEntityState.Angry)) return;

                var canTakeDamage = col.gameObject.GetComponent<EnemyController>().CanTakeDamage();
                //Debug.Log("canTakeDamage:" + canTakeDamage);
                if (!canTakeDamage) return;
            }
            if (!InGuard) Die();
            //EditorApplication.isPaused = true;
        }
        else if (col.CompareTag("BongBong") || col.CompareTag("ItemOnMap"))
        {
            ProcessEatItem(col.gameObject, col.CompareTag("BongBong") ? 1 : 2);
        }
        else if (col.CompareTag("SlowItem"))
        {
            Debug.Log("slow item");
            if (allEffectSpeed.ContainsKey(EffectItem.SlowItem)) return;
            allEffectSpeed.Add(EffectItem.SlowItem, new EffectSpeed() { percent = -50, time = Time.time + 5 });
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (isDie) return;

         if (!subStates.Contains(DynamicEntityState.Invisible) && col.CompareTag("Enemy"))
        {
            //Debug.Log("guardTimer < Time.time:" + (guardTimer < Time.time) + "guardTimer:" + guardTimer);
            if (col.CompareTag("Enemy"))
            {
                if (subStates.Contains(DynamicEntityState.Angry)) return;

                var canTakeDamage = col.gameObject.GetComponent<EnemyController>().CanTakeDamage();
                //Debug.Log("canTakeDamage:" + canTakeDamage);
                if (!canTakeDamage) return;
            }
            if (!InGuard) Die();
            //EditorApplication.isPaused = true;
        }
    }

    protected override void Die()
    {
        if (isDie) return;
        base.Die();

        AudioManager.instance.PlaySound(SoundName.player_die);

        Remove();

        StartCoroutine(DieIE());
    }

    public void Remove()
    {
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
        if (angryEffects.Count > 0)
        {
            for (int k = 0; k < angryEffects.Count; k++)
            {
                var ang = angryEffects[k];
                angryEffects.RemoveAt(k);
                ObjectPool.instance.Disable(ang.gameObject);
                k--;
            }
        }
        if (poisonEffectTf != null)
        {
            ObjectPool.instance.Disable(poisonEffectTf.gameObject);
            poisonEffectTf = null;
        }

        subStates.Clear();

        MyGraph.instance.UpdateLocation(currentCell, null);
    }

    public override bool IsLive()
    {
        return instance != null && gameObject.activeInHierarchy && isDie == false;
    }

    public DynamicEntityDirection MoveByJoyStick(float tan)
    {
        //Debug.Log("tan:" + tan);
        //if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        //{
        //Debug.Log("Direct");
        if (tan > 45 && tan < 135)
        {
            SetDirectionButton(DynamicEntityDirection.Up);
            //Debug.Log("Up");
        }
        else if (tan >= 135 && tan <= 225)
        {
            SetDirectionButton(DynamicEntityDirection.Left);
            //Debug.Log("Left");
        }
        else if (tan > 225 && tan <= 315)
        {
            SetDirectionButton(DynamicEntityDirection.Down);
            //Debug.Log("Down");
        }
        else if (tan > 315 || tan <= 45)
        {
            SetDirectionButton(DynamicEntityDirection.Right);
            //Debug.Log("Right");
        }
        return directionByButton;
        //}
    }

    protected override void ProcessAnim()
    {
        string directAnimName = GetDirectAnimName();

        string nextAnimName = "";
        if (state == DynamicEntityState.Idle)
        {
            nextAnimName = string.Format("{0}_idle", directAnimName);
            //if (subState == DynamicEntityState.Angry) nextAnimName = string.Format("{0}_idle_angry", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
        }
        else if (state == DynamicEntityState.Walk)
        {
            nextAnimName = string.Format("{0}_walk", directAnimName);
            //if (subState == DynamicEntityState.Angry) nextAnimName = string.Format("{0}_walk_angry", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
        }
        else if (state == DynamicEntityState.Shoot)
        {
            nextAnimName = string.Format("{0}_shoot", directAnimName);
            //if (subState == DynamicEntityState.Angry) nextAnimName = string.Format("{0}_shoot_angry", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
            this.shootAnimName = nextAnimName;
        }
        else if (state == DynamicEntityState.Action)
        {
            skeletonAnimation.loop = false;

            nextAnimName = string.Format("{0}_action", directAnimName);
            if (afterAnimName != "") nextAnimName += "_" + afterAnimName;
            this.actionAnimName = nextAnimName;
        }
        else if (state == DynamicEntityState.Die)
        {
            nextAnimName = "die_start";
        }

        //Debug.Log(nextAnimName + " directAnimName:" + directAnimName);
        var currentAnim = skeletonAnimation.AnimationName;
        if (currentAnim == nextAnimName || nextAnimName == "") return;
        //Debug.Log("nextAnimName:" + nextAnimName + " directAnimName:" + directAnimName);
        skeletonAnimation.AnimationName = nextAnimName;
    }

    protected virtual void Action()
    {

    }

    public virtual void StartAction()
    {
        //Debug.Log("isDie:" + isDie + " GameManager.instance.IsGameOver:" + GameManager.instance.IsGameOver + " InGuard:" + InGuard
        //    + " state:" + state);
        if (isDie || GameManager.instance.IsGameLoose || InGuard) return;
        if (state == DynamicEntityState.Shoot || state == DynamicEntityState.Ice) return;
        if (subStates.Contains(DynamicEntityState.Invisible)) return;

        if (state == DynamicEntityState.Action) return;

        state = DynamicEntityState.Action;
    }

    public void EndAction()
    {
        this.actionAnimName = "";
        Idle(0);
        skeletonAnimation.loop = true;
        ProcessAnim();
    }

    public void AutoMove(Vector3Int curCell)
    {
        var doors = MyGraph.instance.doors.Where(d => (d.X == curCell.x && Mathf.Abs(d.Y - curCell.y) <= 1)
            || (d.Y == curCell.y && Mathf.Abs(d.X - curCell.x) <= 1));

        var xs = doors.Select(d => d.X);
        var ys = doors.Select(d => d.Y);

        var all = ys.All(y => y < curCell.y);
        //Debug.Log("AutoMove 1:" + all);
        if (all) directionByButton = DynamicEntityDirection.Down;
        else
        {
            all = ys.All(y => y > curCell.y);
            //Debug.Log("AutoMove 2:" + all);

            if (all) directionByButton = DynamicEntityDirection.Up;
            else
            {
                all = xs.All(x => x < curCell.x);
                //Debug.Log("AutoMove 3:" + all);

                if (all) directionByButton = DynamicEntityDirection.Left;
                else directionByButton = DynamicEntityDirection.Right;
            }
        }
        Debug.Log("AutoMove:" + directionByButton);
    }

    private IEnumerator Blur()
    {
        GameManager.instance.arrowTf.gameObject.SetActive(false);
        while (skeletonAnimation.skeleton.A > 0)
        {
            yield return new WaitForSeconds(0.2f);
            skeletonAnimation.skeleton.A -= 0.2f;
        }
        skeletonAnimation.skeleton.A = 0f;
    }

    public void NextPart(Vector3 position)
    {
        countAutoMove = 0;
        endPartOfMap = false;
        autoMoveState = DynamicEntityState.None;
        directionByButton = DynamicEntityDirection.None;

        MapManager.instance.CalCurMapBound(position);
        MapManager.instance.GenItemsInBound();
        MapManager.instance.GenEnemies();
        MapManager.instance.CloseAllDoor(false);
        //GameManager.instance.GenBalloon();
    }

    public override bool Shoot()
    {
        var s = base.Shoot();
        AudioManager.instance.PlaySound(SoundName.player_kick);
        if (s)
        {
            if (TutorialController.Instance != null)
            {
                var curCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
                TutorialController.Instance.ShowTutKick(curCell);
            }
        }
        return s;
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        movePointTf.position = pos;
    }
}
