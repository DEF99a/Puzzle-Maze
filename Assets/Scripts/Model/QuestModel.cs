using Assets.Scripts.Define;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Model
{
    public enum QuestType
    {
        KillEnemy,
        Upgrade,
        PassLevel,
        PassGameNoDie,
        UseItem,
        ViewAd,
        CollectGold
    }

    [System.Serializable]
    public class QuestKill : QuestModel
    {
        public EnemyType enemyType;
    }

    [System.Serializable]
    public class QuestUseItem : QuestModel
    {
        public EntityType entityType;
    }

    [System.Serializable]
    public class QuestModel
    {
        public QuestType questType;
        public int requestAmount;
        public int coinReward;

        [HideInInspector] public int myComplete;
        [HideInInspector] public int canRev;// 0: chưa nhận, 1: đã nhận
        [HideInInspector] public bool showedNotice;

        public bool IsComplete()
        {
            if (canRev == 1)
            {
                return true;
            }
            else
            {
                if (myComplete >= requestAmount)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }        
    }
}