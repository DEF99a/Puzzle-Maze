using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using Assets.Scripts.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Hellper
{
    public class QuestHelper
    {
        public static List<QuestModel> GetQuests(bool force = false)
        {
            var quests = ES3.Load(StringDefine.Quests, new List<QuestModel>());
            if (quests.Count == 0 || force)
            {
                GenNew();
            }
            else
            {
                var date = ES3.Load(StringDefine.DateQuests, DateTime.Now.Date);
                if (DateTime.Now.Date > date)
                {
                    GenNew();
                }
            }

            void GenNew()
            {
                quests.Clear();

                int i = 0;
                int max = 5;
                if (PurchaseHelper.InVip()) max = 6;

                var questTypes = QuestDatas.instance.pairs.Keys.ToList();

                do
                {
                    var qt = questTypes[UnityEngine.Random.Range(0, questTypes.Count())];
                    var list = QuestDatas.instance.pairs[qt];
                    var quest = list[UnityEngine.Random.Range(0, list.Count)];
                    questTypes.Remove(qt);
                    //Debug.Log("qt:" + qt);

                    quests.Add(quest);

                    i++;
                } while (i < max);

                ES3.Save(StringDefine.Quests, quests);
                ES3.Save(StringDefine.DateQuests, DateTime.Now.Date);
                if (force) ES3.Save(StringDefine.RefreshQuestAmount, 1);
                else ES3.Save(StringDefine.RefreshQuestAmount, 0);
            }

            quests = quests.OrderByDescending(q => q.canRev == 0 && q.myComplete >= q.requestAmount).ToList();
            return quests;
        }

        public static bool Action(QuestType questType, int amount, EnemyType enemyType = EnemyType.None, EntityType entityType = EntityType.None)
        {
            var quests = GetQuests();
            var quest = quests.FirstOrDefault(q => q.questType == questType);
            Debug.Log("Action quest type:" + questType);
            if (quest == null) return false;

            if (questType == QuestType.KillEnemy)
            {
                QuestKill qk = (QuestKill)quest;
                if (qk.enemyType == enemyType)
                    qk.myComplete += amount;
            }
            else if (questType == QuestType.UseItem)
            {
                QuestUseItem qu = (QuestUseItem)quest;
                if (qu.entityType == entityType)
                    qu.myComplete += amount;
            }
            else
            {
                quest.myComplete += amount;
            }
            Debug.Log("QQQQQQQQQ:" + quest.showedNotice);
            ES3.Save(StringDefine.Quests, quests);

            //if (quest.myComplete >= quest.requestAmount)
            //{
            //    EventDispatcher.Instance.PostEvent(EventID.CompleteQuestInGame, (amount, enemyType, questType));
            //}   
            return false;
        }

        public static void ChangeQuest(QuestModel questModel)
        {
            EnemyType enemyType = EnemyType.None;
            EntityType entityType = EntityType.None;
            if (questModel is QuestKill qk) enemyType = qk.enemyType;
            if (questModel is QuestUseItem qu) entityType = qu.entityType;

            var quests = GetQuests();
            var questTypes = QuestDatas.instance.pairs.Keys.ToList();
            int index = 0;
            for (int k = 0; k < quests.Count; k++)
            {
                var quest = quests[k];
                //Debug.Log("questType:" + quest.questType);

                questTypes.Remove(quest.questType);

                if (enemyType != EnemyType.None)
                {
                    if (quest.questType == questModel.questType && ((QuestKill)quest).enemyType == enemyType)
                    {
                        index = k;
                        quests.RemoveAt(k);
                        k--;
                    }
                }
                else if (entityType != EntityType.None)
                {
                    if (quest.questType == questModel.questType && ((QuestUseItem)quest).entityType == entityType)
                    {
                        index = k;
                        quests.RemoveAt(k);
                        k--;
                    }
                }
                else
                {
                    if (quest.questType == questModel.questType)
                    {
                        index = k;
                        quests.RemoveAt(k);
                        k--;
                    }
                }
            }

            //Debug.Log("questTypes.Count():" + String.Join(",", questTypes.ToArray()));

            var qt = questTypes[UnityEngine.Random.Range(0, questTypes.Count())];

            var list = QuestDatas.instance.pairs[qt];
            var _quest = list[UnityEngine.Random.Range(0, list.Count)];
            quests.Insert(index, _quest);

            ES3.Save(StringDefine.Quests, quests);
        }

        public static void Complete(QuestModel questModel)
        {
            var quests = GetQuests();
            EnemyType enemyType = EnemyType.None;
            EntityType entityType = EntityType.None;
            if (questModel is QuestKill qk) enemyType = qk.enemyType;
            if (questModel is QuestUseItem qu) entityType = qu.entityType;

            for (int k = 0; k < quests.Count; k++)
            {
                var quest = quests[k];
                if (quest.questType == questModel.questType)
                {
                    if (enemyType != EnemyType.None)
                    {
                        if (((QuestKill)quest).enemyType == enemyType)
                        {
                            quest.canRev = 1;
                        }
                    }
                    else if (entityType != EntityType.None)
                    {
                        if (((QuestUseItem)quest).entityType == entityType)
                        {
                            quest.canRev = 1;
                        }
                    }
                    else quest.canRev = 1;
                    break;
                }
            }
            ES3.Save(StringDefine.Quests, quests);
        }
        
    }
}