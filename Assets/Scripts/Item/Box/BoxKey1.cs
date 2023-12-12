using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxKey1 : BoxBase
{
    [SerializeField] private Sprite unlock;
    [SerializeField] private Sprite lockSp;

    private bool overlapGroundKey;

    public override void BoxInit(BoxType boxType, SkeletonDataAsset skeletonDataAsset, Sprite sprite, bool _canBorn, bool _canBreak,
        bool _canBeKicked, bool _canBurn, bool _enemyCanFly, int _moveAmount)
    {
        base.BoxInit(boxType, skeletonDataAsset, sprite, _canBorn, _canBreak, _canBeKicked, _canBurn, _enemyCanFly, _moveAmount);
        overlapGroundKey = false;
        spriteRenderer.sprite = lockSp;
    }

    public override void Setup(ItemParam itemParam)
    {
        //if (GameManager.instance.IsGameOver)
        //{
        //    state = DynamicEntityState.Idle;
        //}
        //else
        //{
        isDie = false;

        this.itemParam = itemParam;

        movePointTf.parent = null;
        movePointTf.position = transform.position;

        direction = itemParam.direct;
        if (direction != DynamicEntityDirection.None)
        {
            RollBackData rollback = new RollBackData();
            rollback.IsMovement = false;
            rollback.Attack = new RollBackAttackData();
            rollback.Attack.box = this;
            switch (direction)
            {
                case DynamicEntityDirection.Down:
                    rollback.Attack.oldDirection = DynamicEntityDirection.Up;
                    break;
                case DynamicEntityDirection.Up:
                    rollback.Attack.oldDirection = DynamicEntityDirection.Down;
                    break;
                case DynamicEntityDirection.Right:
                    rollback.Attack.oldDirection = DynamicEntityDirection.Left;
                    break;
                case DynamicEntityDirection.Left:
                    rollback.Attack.oldDirection = DynamicEntityDirection.Right;
                    break;
            }
            GameManager.instance.AddRollBack(rollback);
        }
        state = DynamicEntityState.Walk;
        currentMoveSpeed = itemParam.speed;

        if (!CanKickFly && CanBreak) currentMoveSpeed = 0;

        if (HasEnemyAbove) currentMoveSpeed = 0;
        if (currentMoveSpeed == 0)
        {
            moveStepCount = 0;
            state = DynamicEntityState.Idle;
        }
        diedEnemyAmount = 0;

        //}
        //currentCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
    }

    public void RollBack(RollBackAttackData rollback)
    {
        isDie = false;

        movePointTf.parent = null;
        movePointTf.position = transform.position;

        direction = rollback.oldDirection;
        state = DynamicEntityState.Walk;
        currentMoveSpeed = 10f;

        if (!CanKickFly && CanBreak) currentMoveSpeed = 0;

        if (HasEnemyAbove) currentMoveSpeed = 0;
        if (currentMoveSpeed == 0)
        {
            moveStepCount = 0;
            state = DynamicEntityState.Idle;
        }
        diedEnemyAmount = 0;
    }

    protected override void Move()
    {
        if (isDie) return;
        if (state != DynamicEntityState.Walk) return;

        if (Vector3.Distance(transform.position, movePointTf.position) <= 0.01f)
        {
            if (moveStepCount == 0)
            {
                var tmpPos = transform.position + directPos[direction];
                CheckLocation(tmpPos);
            }
            else
            {
                state = DynamicEntityState.Idle;
                moveStepCount = 0;

                Interact();
            }
        }
    }

    protected override void CheckLocation(Vector3 tmpPos)
    {
        var location = MyGraph.instance.GetLocationFromPos(tmpPos);
        if (location == null) return;

        if (MapManager.instance.IsInOpenDoor(tmpPos))
        {
            moveStepCount = 0;
            state = DynamicEntityState.Idle;
            return;
        }

        if (location.tileType == TileType.CanMove)
        {
            
            movePointTf.position = tmpPos;
            moveStepCount++;
        }
        else if (location.tileType == TileType.Dynamic)
        {
            //Debug.Log(gameObject.name + " location.itemBase.entityType:" + location.itemBase.entityType + " moving:" + location.itemBase.IsMoving());
            if (EntityHelper.IsBox(location) || EntityHelper.IsBoxKey(location))
            {
                if (location.entityBase.IsMoving())
                {
                    movePointTf.position = tmpPos;
                    moveStepCount++;
                }
                else
                {
                    moveStepCount = 0;
                    state = DynamicEntityState.Idle;
                }

            }
            else
            {
                movePointTf.position = tmpPos;
                moveStepCount++;
            }
        }
        else if (location.tileType == TileType.Obtacle)
        {
            moveStepCount = 0;
            state = DynamicEntityState.Idle;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
        if (state == DynamicEntityState.Walk)
        {
            if (ColliderHelper.HasElastic(col))
            {
                direction = opDirects[direction];
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Debug.Log("onnnnnnnnn");

        if (overlapGroundKey) return;

        if (state != DynamicEntityState.Walk)
        {
            if (collision.gameObject.name.Contains("GroundKey"))
            {
                Interact();
            }
        }
    }

    protected override void Die()
    {
        isDie = true;
        MyGraph.instance.UpdateLocation(currentCell, null);
    }

    public override void RemoveOnTileMap()
    {
        Die();
    }

    public void Interact()
    {
        var _overlapGroundKey = false;
        var allGroundKey = MapManager.instance.AllGroundKey;
        var curCell = MyGraph.instance.GetCellFromPosition(transform.position, out _);
        foreach (var groundKey in allGroundKey)
        {
            var groundKeyCell = MyGraph.instance.GetCellFromPosition(groundKey.transform.position, out _);
            //Debug.Log("curCell:" + curCell + " groundKeyCell:" + groundKeyCell + " " + (curCell == groundKeyCell));

            if (curCell == groundKeyCell)
            {
                _overlapGroundKey = true;
                break;
            }
        }
        //Debug.Log("_overlapGroundKey:" + _overlapGroundKey);

        if (_overlapGroundKey)
        {
            //Debug.Log("overlapGroundKey:" + overlapGroundKey);
            AudioManager.instance.PlaySound(SoundName.key_unlock);

            if (overlapGroundKey == false)
            {
                overlapGroundKey = true;
                MapManager.instance.KeyMatchAmount++;
                spriteRenderer.sprite = unlock;
                if (MapManager.instance.KeyMatchAmount == MapManager.instance.TotalKey)
                {
                    if (MapManager.instance.GateCheckpoints.Count <= 1)
                        GameManager.instance.GameOver(true, GameEndReason.WinPuzzle);
                    else
                    {
                        this.PostEvent(EventID.EndPartOfMap);
                        MapManager.instance.OpenGateInBound();
                    }

                    MapManager.instance.ChangeKeyStage();
                }
            }
        }
        else
        {
            //Debug.Log("overlapGroundKey 1:" + overlapGroundKey);

            if (overlapGroundKey)
            {
                spriteRenderer.sprite = lockSp;

                MapManager.instance.KeyMatchAmount--;
                overlapGroundKey = false;
            }
        }
    }
}
