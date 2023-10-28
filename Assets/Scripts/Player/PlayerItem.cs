using Assets.Scripts.Common;
using Assets.Scripts.Controller;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlayerController : CharacterBase
{
    private float angryTimer, angryTime;
    private Coroutine angryCoroutine;
    private List<ParticleSystem> angryEffects;

    public override void ProcessEatItem(GameObject go, int balloon)
    {
        EntityType entityType;
        if (balloon == 1)
            entityType = go.GetComponent<Balloon>().entityType;
        else
        {
            entityType = go.GetComponent<ItemOnMap>().entityType;
            if (entityType == EntityType.ItemLife)
            {
                AudioManager.instance.PlaySound(SoundName.pick_life);
            }
            else if (entityType == EntityType.ItemFreeze)
            {
                AudioManager.instance.PlaySound(SoundName.pick_freeze);
            }
            else if (entityType == EntityType.ItemAngry)
            {
                AudioManager.instance.PlaySound(SoundName.pick_angry);
            }
            else if (entityType == EntityType.ItemInvisible)
            {
                AudioManager.instance.PlaySound(SoundName.pick_invisible);
            }
            else if (entityType == EntityType.ItemSpeed)
            {
                AudioManager.instance.PlaySound(SoundName.pick_speed);
            }
            else if (entityType.ToString().Contains("ItemCoin"))
            {
                AudioManager.instance.PlaySound(SoundName.pick_coin);
            }
            else if (entityType.ToString().Contains("ItemTimer"))
            {
                AudioManager.instance.PlaySound(SoundName.pick_clock);
            }
        }  
        ProcessActiveItem(entityType, go);
    }

    public void ProcessActiveItem(EntityType entityType, GameObject go)
    {
        var name = entityType.ToString();
        if (name.Contains("ItemCoinRewardEndGameX"))
            QuestHelper.Action(Assets.Scripts.Model.QuestType.UseItem, 1, EnemyType.None, EntityType.ItemCoinRewardEndGameX);
        else if(name.Contains("ItemCoin"))
            QuestHelper.Action(Assets.Scripts.Model.QuestType.UseItem, 1, EnemyType.None, EntityType.ItemCoin);
        else if (name.Contains("ItemTimer"))
            QuestHelper.Action(Assets.Scripts.Model.QuestType.UseItem, 1, EnemyType.None, EntityType.ItemTimer);
        else QuestHelper.Action(Assets.Scripts.Model.QuestType.UseItem, 1, EnemyType.None, entityType);

        switch (entityType)
        {
            case EntityType.ItemAngry:
                Angry(5f);
                break;
            case EntityType.ItemInvisible:
                Inivisible(5f);
                break;
            case EntityType.ItemFreeze:
                TakeIce();
                break;
            case EntityType.ItemSpeed:
                if (allEffectSpeed.ContainsKey(EffectItem.IncrSpeed))
                {
                    var esp = allEffectSpeed[EffectItem.IncrSpeed];
                    esp.time += 5;
                    allEffectSpeed[EffectItem.IncrSpeed] = esp;
                }
                else allEffectSpeed.Add(EffectItem.IncrSpeed, new EffectSpeed() { percent = 20, time = Time.time + 5 });
                break;
            case EntityType.ItemGun:
                break;
            case EntityType.ItemKillAll:
                var curEnemies1 = MapManager.instance.enemies.ToList();
                for (int k = 0; k < curEnemies1.Count; k++)
                    curEnemies1[k].Killed();
                break;
            case EntityType.ItemLife:
            case EntityType.ItemCoin100:
            case EntityType.ItemCoin200:
            case EntityType.ItemCoin500:

            case EntityType.ItemCoin1000:
            case EntityType.ItemCoin2000:
            case EntityType.ItemTimer5:
            case EntityType.ItemTimer10:
            case EntityType.ItemTimer20:
            case EntityType.ItemCoinRewardEndGameX2:
            case EntityType.ItemCoinRewardEndGameX3:
                GameUIManager.instance.PlayerEatItem(go.transform.position, entityType);
                break;
        }
    }

    private void Angry(float timeLife)
    {
        if(!subStates.Contains(DynamicEntityState.Angry))
            subStates.Add(DynamicEntityState.Angry);
        angryEffects.Add(EffectController.instance.GenEffectAngry(transform.position));

        //if (AngryEffect.instance.play == false)
        //{
        //    angryTimer = Time.time + timeLife;
        //    AngryEffect.instance.Setup(timeLife);
        //}
        //else
        //{
        //    angryTimer += timeLife;
        //    AngryEffect.instance.Setup(angryTimer - Time.time);
        //}
        
        //if (angryCoroutine == null)
        //{
        //    angryTimer = Time.time + timeLife;
        //    angryCoroutine = StartCoroutine(AngryIE());
        //}
        //else
        //{
        //    angryTimer += timeLife;
        //}
        //IEnumerator AngryIE()
        //{
        //    subState = DynamicEntityState.Angry;
        //    while (!isDie && GameManager.instance.IsPlaying && angryTimer > Time.time)
        //    {
        //        yield return null;
        //    }
        //    subState = DynamicEntityState.None;
        //    angryCoroutine = null;
        //}
    }
    
    public void TakeIce()
    {
        var curEnemies = MapManager.instance.enemies.ToList();
        for (int k = 0; k < curEnemies.Count; k++)
            curEnemies[k].Frozen(5f, true);
    }
}
