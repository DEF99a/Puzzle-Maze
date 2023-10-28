using Assets.Scripts.Bullet;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBase : EntityBase
{
    public SkeletonAnimation skeletonAnimation;
    public SpriteRenderer spriteRenderer;

    protected int moveStepCount;
    protected ItemParam itemParam;
    public ItemParam GetItemParam => itemParam;

    [HideInInspector] public int diedEnemyAmount;

    public bool CanBorn { get; private set; }
    public bool CanBreak { get; private set; }
    public bool CanKickFly { get; set; }
    public bool EnemyCanKickFly { get; set; }
    public bool CanBurn { get; private set; }

    public bool CanShoot()
    {
        return isDie == false && state == DynamicEntityState.Idle;
    }
    protected int moveAmount = -1;
    protected BoxType boxType;

    public virtual void BoxInit(BoxType boxType, SkeletonDataAsset skeletonDataAsset, Sprite sprite, bool _canBorn, bool _canBreak, bool _canKickFly,
        bool _canBurn, bool _enemyCanKickFly, int _moveAmount)
    {
        base.Init();

        if (skeletonDataAsset != null && skeletonAnimation != null)
        {
            skeletonAnimation.ClearState();

            skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
            skeletonAnimation.Initialize(true);
            skeletonAnimation.loop = true;
            skeletonAnimation.AnimationName = "idle";
            skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
            skeletonAnimation.AnimationState.Complete += OnAnimationComplete;
            skeletonAnimation.gameObject.SetActive(true);

            if (spriteRenderer != null)
                spriteRenderer.gameObject.SetActive(false);
        }
        else if (spriteRenderer != null)
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.sprite = sprite;
            if (skeletonAnimation != null)
                skeletonAnimation.gameObject.SetActive(false);
        }

        isDie = false;
        state = DynamicEntityState.Idle;
        moveStepCount = 0;
        diedEnemyAmount = 0;

        CanBorn = _canBorn;
        CanBreak = _canBreak;
        CanKickFly = _canKickFly;
        CanBurn = _canBurn;
        this.EnemyCanKickFly = _enemyCanKickFly;
        this.moveAmount = _moveAmount;
        this.boxType = boxType;
    }

    protected virtual void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "animation")
            ObjectPool.instance.Disable(gameObject);
    }

    public virtual void Setup(ItemParam itemParam)
    {
        isDie = false;

        this.itemParam = itemParam;

        movePointTf.parent = null;
        movePointTf.position = transform.position;

        direction = itemParam.direct;
        state = DynamicEntityState.Walk;
        currentMoveSpeed = itemParam.speed;

        if (!CanKickFly && CanBreak) currentMoveSpeed = 0;

        if (HasEnemyAbove) currentMoveSpeed = 0;
        if (currentMoveSpeed == 0)
        {
            if (CanBreak)
                Die();
            else
            {
                moveStepCount = 0;
                state = DynamicEntityState.Idle;
            }
        }
        diedEnemyAmount = 0;

        //currentCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
    }

    protected override void Update()
    {
        if (isDie) return;

        Move();
        UpdateCellMap();
    }

    protected override void Move()
    {
        if (isDie) return;
        if (state != DynamicEntityState.Walk) return;

        if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        {
            if (moveAmount != -1)
            {
                if (moveStepCount == moveAmount)
                {
                    state = DynamicEntityState.Idle;
                    moveStepCount = 0;
                }
                else
                {
                    var tmpPos = transform.position + directPos[direction];
                    CheckLocation(tmpPos);
                }
            }
            else
            {
                var tmpPos = transform.position + directPos[direction];
                CheckLocation(tmpPos);
            }
        }
    }

    protected virtual void CheckLocation(Vector3 pos)
    {
    }

    protected override void Die()
    {
        if (isDie) return;

        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }

        base.Die();

        //Debug.Log("State:" + DynamicEntityState.Walk);
        if (state == DynamicEntityState.Walk)
        {
            var cell2 = MyGraph.instance.GetCellFromPosition(transform.position, out _);
            var enemies = MapManager.instance.enemies;
            for (int k = 0; k < enemies.Count; k++)
            {
                var enemy = enemies[k];
                var cell1 = MyGraph.instance.GetCellFromPosition(enemy.transform.position, out _);
                //Debug.Log("cell2:" + cell2 + " cell1:" + cell1);

                if (cell1 == cell2)
                {
                    enemy.Killed();
                }
            }
        }

        state = DynamicEntityState.Idle;

        //skeletonAnimation.loop = false;
        skeletonAnimation.AnimationName = "animation";

        if (CanBorn)
        {
            Debug.Log("Dieeeeeee 1:" + boxType);
            MapManager.instance.DecreaseBoxAmount();
        }
        if (CanKickFly)
        {
            //Debug.Log("Dieeeeeee 2:" + boxType);
            MapManager.instance.DecreaseBoxCanFlyAmount();
        }

        MyGraph.instance.UpdateLocation(currentCell, null);
    }

    public virtual void RemoveOnTileMap()
    {
        if (isDie) return;

        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }


        Die();

        if (HasEnemyAbove) HasEnemyAbove = false;
        Debug.Log("RemoveOnTileMap");
    }
}
