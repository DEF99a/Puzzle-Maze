using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public partial class EnemyController : CharacterBase
{
    public override void ProcessEatItem(GameObject go, int balloon)
    {
        EntityType entityType;
        if (balloon == 1)
            entityType = go.GetComponent<Balloon>().entityType;
        else entityType = go.GetComponent<ItemOnMap>().entityType;

        switch (entityType)
        {
            case EntityType.ItemLife:
                this.life += 1;
                break;
            case EntityType.ItemAngry:
                if (allEffectSpeed.ContainsKey(EffectItem.Angry))
                {
                    var esp = allEffectSpeed[EffectItem.Angry];
                    esp.time += 5;
                    allEffectSpeed[EffectItem.Angry] = esp;
                }
                else allEffectSpeed.Add(EffectItem.Angry, new EffectSpeed() { percent = 10, time = Time.time + 5 });
                break;
            case EntityType.ItemInvisible:
                Inivisible(5f);
                break;
            case EntityType.ItemFreeze:
                if (PlayerController.instance.IsLive())
                    PlayerController.instance.Frozen(5f, true);
                break;
            case EntityType.ItemSpeed:
                if (allEffectSpeed.ContainsKey(EffectItem.IncrSpeed))
                {
                    var esp = allEffectSpeed[EffectItem.Angry];
                    esp.time += 5;
                    allEffectSpeed[EffectItem.Angry] = esp;
                }
                else allEffectSpeed.Add(EffectItem.IncrSpeed, new EffectSpeed() { percent = 20, time = Time.time + 5 });
                break;
            case EntityType.ItemGun:
                break;
            case EntityType.ItemKillAll:
                if (PlayerController.instance.IsLive())
                    PlayerController.instance.Killed();
                break;
        }
    }
}
