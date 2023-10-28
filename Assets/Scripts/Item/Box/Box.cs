using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : BoxBase
{
    public void BoxSpecialInit(SkeletonDataAsset skeletonDataAsset, Sprite sprite)
    {
        skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
        skeletonAnimation.Initialize(true);
        skeletonAnimation.AnimationName = "idle";
        skeletonAnimation.loop = true;
        skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;
        skeletonAnimation.gameObject.SetActive(true);

        spriteRenderer.gameObject.SetActive(true);
        spriteRenderer.sprite = sprite;
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
                else
                {
                    if (moveStepCount > 0)
                    {
                        moveStepCount = 0;
                        state = DynamicEntityState.Idle;
                    }
                    else Die();
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
            if (moveStepCount > 0)
            {
                moveStepCount = 0;
                state = DynamicEntityState.Idle;
            }
            else Die();
        }
        if(!isDie && state == DynamicEntityState.Idle)
            AudioManager.instance.PlaySound(SoundName.block_collision);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDie) return;

        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + col.tag + " CanBurn:" + CanBurn);
        if (col.CompareTag("RedImpostorCollider") && CanBreak)
        {
            Die();
        }
        else if (col.CompareTag("BombEffect") && CanBurn)
        {
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
    }

    protected override void Die()
    {
        //Debug.Log("Die canBreak:" + CanBreak + " CanBurn:" + CanBurn);
        if (isDie) return;
        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }

        base.Die();
        PlaySoundDie();

        if (skeletonAnimation != null && spriteRenderer != null && skeletonAnimation.gameObject.activeSelf && spriteRenderer.gameObject.activeSelf)
            spriteRenderer.gameObject.SetActive(false);

        if (HasEnemyAbove) HasEnemyAbove = false;

        MapManager.instance.RemoveNormalBox(this);
        if (GetItemParam.characterBase?.entityType == EntityType.Player)
            GameManager.instance.UpdateGameCoin(transform.position, Config.CoinRemoveBox, CoinFrom.RemoveBox);
    }

    public override void RemoveOnTileMap()
    {
        if (isDie) return;
        if (!CanBreak && !CanBurn)
        {
            state = DynamicEntityState.Idle;
            return;
        }

        base.Die();
        PlaySoundDie();

        if (skeletonAnimation != null && spriteRenderer != null && skeletonAnimation.gameObject.activeSelf && spriteRenderer.gameObject.activeSelf)
            spriteRenderer.gameObject.SetActive(false);

        if (HasEnemyAbove) HasEnemyAbove = false;
        MapManager.instance.RemoveNormalBox(this);
    }

    protected virtual void PlaySoundDie()
    {
        if (boxType == BoxType.tile_stone_01 || boxType == BoxType.tile_stone_02 || boxType == BoxType.tile_stone_03)
            AudioManager.instance.PlaySound(SoundName.stone_break, 0.1f);
        else AudioManager.instance.PlaySound(SoundName.soil_break_2, 0.1f);
    }    
}
