using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallBox : BoxBase
{
    [SerializeField] private GameObject explodeItem;

    private int collideStaticItemAmount;
    private int mExplodeWhenCollideAmount;
    private int collideCount;
    private bool mHasPoisonItem;

    public override void BoxInit(BoxType boxType, SkeletonDataAsset skeletonDataAsset, Sprite sprite, bool _canBorn, bool _canBreak,
        bool _canBeKicked, bool _canBurn, bool _enemyCanFly, int _moveAmount)
    {
        base.BoxInit(boxType, skeletonDataAsset, sprite, _canBorn, _canBreak, _canBeKicked, _canBurn, _enemyCanFly, _moveAmount);
        collideStaticItemAmount = 0;
    }

    public void BallBoxSetup(bool hasPoisonItem, int explodeWhenCollideAmount = -1)
    {
        collideCount = 0;
        mExplodeWhenCollideAmount = explodeWhenCollideAmount;
        mHasPoisonItem = hasPoisonItem;
    }

    protected override void Update()
    {
        if (isDie) return;

        ProcessAnim();
        Move();
        UpdateCellMap();
    }

    protected override void CheckLocation(Vector3 tmpPos)
    {
        var location = MyGraph.instance.GetLocationFromPos(tmpPos);
        if (location == null) return;
        if (MapManager.instance.IsInOpenDoor(tmpPos))
        {
            if (moveStepCount > 0)
            {
                moveStepCount = 0;
                state = DynamicEntityState.Idle;
            }
            else Die();
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
                else CollideStaticItem();
            }
            else
            {
                movePointTf.position = tmpPos;
                moveStepCount++;
            }
        }
        else if (location.tileType == TileType.Obtacle)
        {
            CollideStaticItem();
        }
        if (!isDie && state == DynamicEntityState.Idle)
            AudioManager.instance.PlaySound(SoundName.block_collision);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDie) return;

        if (col.CompareTag("RedImpostorCollider"))
        {
            Die();
        }
        else if (state == DynamicEntityState.Walk)
        {
            if (ColliderHelper.HasElastic(col))
            {
                Elastic();
                AudioManager.instance.PlaySound(SoundName.jelly_bounce);
            }
        }
        else if (col.CompareTag("BombEffect"))
        {
            Die();
        }
    }

    protected virtual void CollideStaticItem()
    {
        if (moveStepCount > 0)
        {
            collideCount++;

            if (collideStaticItemAmount == 0)
            {
                moveStepCount = 0;
                collideStaticItemAmount++;
                Elastic();
            }
            else
            {
                moveStepCount = 0;
                collideStaticItemAmount = 0;
                state = DynamicEntityState.Idle;
                if (collideCount != mExplodeWhenCollideAmount)
                    itemParam.characterBase = null;
            }

            if (collideCount == mExplodeWhenCollideAmount)
            {
                Die();
                if (mHasPoisonItem)
                {
                    var location = MyGraph.instance.GetLocationFromPos(transform.position);
                    var go = ObjectPool.instance.GetGameObject(explodeItem, location.ToVector3(), Quaternion.identity);
                    go.GetComponent<TrapItem>().Setup(true);
                }
            }
        }
        else Die();
    }

    private void Elastic()
    {
        direction = opDirects[direction];
        AudioManager.instance.PlaySound(SoundName.jelly_bounce);
    }

    private void ProcessAnim()
    {
        if (isDie) return;

        string animName = "";
        if (state == DynamicEntityState.Walk)
        {
            if (direction == DynamicEntityDirection.Down)
                animName = "move_down";
            else if (direction == DynamicEntityDirection.Up)
                animName = "move_up";
            else if (direction == DynamicEntityDirection.Left)
                animName = "move_left";
            else animName = "move_right";
        }
        else if (state == DynamicEntityState.Idle)
            animName = "idle";
        if (animName != skeletonAnimation.AnimationName)
            skeletonAnimation.AnimationName = animName;
    }
    // explode
    protected override void Die()
    {
        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }

        if (isDie) return;
        AudioManager.instance.PlaySound(SoundName.jelly_break, 0.1f);

        base.Die();
        if (GetItemParam.characterBase?.entityType == EntityType.Player)
            GameManager.instance.UpdateGameCoin(transform.position, Config.CoinRemoveBallBox, CoinFrom.RemoveBox);
    }
}
