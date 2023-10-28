using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxWaterMelon : Box
{
    protected override void Die()
    {
        if (!CanBreak)
        {
            state = DynamicEntityState.Idle;
            return;
        }
        if (isDie) return;
        base.Die();
        AudioManager.instance.PlaySound(SoundName.melon_break, 0.1f);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (isDie) return;

        //Debug.Log("target:" + col.gameObject.name + " object: " + gameObject.name + " : " + Time.time);
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
}
