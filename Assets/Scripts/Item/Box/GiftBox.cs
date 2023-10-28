using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBox : BoxBase
{
    private int coinDieByBomEffect;

    protected override void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "animation")
        {
            ObjectPool.instance.Disable(gameObject);

            if (GetItemParam.characterBase?.entityType == EntityType.Player)
                GameManager.instance.UpdateGameCoin(transform.position, Config.CoinRemoveGiftBox, CoinFrom.RemoveBox);
        }
        else if (trackEntry.Animation.Name == "open")
        {
            ObjectPool.instance.Disable(gameObject);
            GameManager.instance.UpdateGameCoin(transform.position, coinDieByBomEffect, CoinFrom.RemoveBox);
            GameManager.instance.GenItemOnMap(transform.position + new Vector3(0, -0.1f));
        }
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
            else
            {
                Debug.Log("die1");
                Die();
            }
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
                    if (moveStepCount > 0)
                    {
                        moveStepCount = 0;
                        state = DynamicEntityState.Idle;
                    }
                    else
                    {
                        Debug.Log("die2");
                        Die();
                    }
                }
            }
            else
            {
                moveStepCount++;
                movePointTf.position = tmpPos;
            }
        }
        else if (location.tileType == TileType.Obtacle)
        {
            if (moveStepCount > 0)
            {
                moveStepCount = 0;
                state = DynamicEntityState.Idle;
            }
            else
            {
                Debug.Log("die3");
                Die();
            }
        }
        if (!isDie && state == DynamicEntityState.Idle)
            AudioManager.instance.PlaySound(SoundName.block_collision);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDie) return;

        if (col.CompareTag("RedImpostorCollider"))
        {
            Debug.Log("die4");
            Die();
        }
        else if (state == DynamicEntityState.Walk)
        {
            if (ColliderHelper.HasElastic(col))
            {
                direction = opDirects[direction];
                AudioManager.instance.PlaySound(SoundName.block_bounce);
            }
        }
        else if (col.CompareTag("BombEffect"))
        {
            DieByBombEffect();
            if (isDie)
            {
                var bombEffect = col.gameObject.GetComponent<BombEffect>();
                if (bombEffect.bombEffectParam.characterBase != null && bombEffect.bombEffectParam.characterBase.entityType == EntityType.Player)
                {
                    coinDieByBomEffect = Config.CoinRemoveGiftBoxByBom * GeneralHelper.Pow(2, bombEffect.bombEffectParam.diedGiftBoxAmount);
                    //GameManager.instance.UpdateGameCoin(transform.position,
                    //    , CoinFrom.RemoveBox);
                    bombEffect.bombEffectParam.diedGiftBoxAmount++;
                }
                //GameManager.instance.GenItemOnMap(transform.position + new Vector3(0, -0.1f));
            }
        }
    }

    protected override void Die()
    {
        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }
        if (isDie) return;

        AudioManager.instance.PlaySound(SoundName.gift_break, 0.1f);

        base.Die();
    }

    private void DieByBombEffect()
    {
        if (isDie) return;
        Debug.Log("die5");

        AudioManager.instance.PlaySound(SoundName.gift_burn);

        isDie = true;
        movePointTf.parent = transform;
        movePointTf.position = transform.position;

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
        skeletonAnimation.AnimationName = "open";

        if (CanBorn)
        {
            Debug.Log("Dieeeeeee:" + boxType);
            MapManager.instance.DecreaseBoxAmount();
        }
        if (CanKickFly)
        {
            Debug.Log("Dieeeeeee 2:" + boxType);
            MapManager.instance.DecreaseBoxCanFlyAmount();
        }

        MyGraph.instance.UpdateLocation(currentCell, null);
        //base.Die();
    }
}
