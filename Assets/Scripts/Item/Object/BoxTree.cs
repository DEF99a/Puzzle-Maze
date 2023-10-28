using Assets.Scripts.Bullet;
using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxTree : Box
{
    private bool bornAfterDie;

    protected override void OnAnimationComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == "animation")
        {
            if (!bornAfterDie)
                ObjectPool.instance.Disable(gameObject);
            else
            {
                skeletonAnimation.enabled = false;
            }
        }
    }

    private void Reborn()
    {
        base.Init();

        skeletonAnimation.enabled = true;
        skeletonAnimation.AnimationName = "idle";
        skeletonAnimation.loop = true;
        skeletonAnimation.AnimationState.Complete -= OnAnimationComplete;
        skeletonAnimation.AnimationState.Complete += OnAnimationComplete;

        isDie = false;
        state = DynamicEntityState.Idle;
        moveStepCount = 0;
        diedEnemyAmount = 0;
    }

    public void TreeSetup(bool _bornAfterDie)
    {
        bornAfterDie = _bornAfterDie;
    }

    public override void Setup(ItemParam itemParam)
    {
        itemParam.speed = 0;
        base.Setup(itemParam);
    }

    protected override void Die()
    {
        if (!CanBreak)
        {
            state = DynamicEntityState.Idle;
            return;
        }
        if (isDie) return;

        base.Die();

        if (isDie && bornAfterDie)
        {
            StartCoroutine(BornIE());
        }
    }

    protected override void PlaySoundDie()
    {
        Debug.Log("BoxTree PlaySoundDie");
        if (boxType == BoxType.box04_grass_01 || boxType == BoxType.box04_grass_02) AudioManager.instance.PlaySound(SoundName.tree_break_1, 0.1f);
        else AudioManager.instance.PlaySound(SoundName.tree_break_2, 0.1f);
    }

    IEnumerator BornIE()
    {
        yield return new WaitForSeconds(5f);
        var loc = MyGraph.instance.GetLocationFromPos(transform.position);
        if(loc.tileType == TileType.CanMove || (!EntityHelper.IsBox(loc) && !EntityHelper.IsBoxKey(loc)))
        {
            AudioManager.instance?.PlaySound(SoundName.tree_respawn);
            Reborn();
        }
        else
        {
            ObjectPool.instance.Disable(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDie) return;

        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
        if (col.CompareTag("RedImpostorCollider") || col.CompareTag("BombEffect"))
        {
            Die();
        }
    }
}
