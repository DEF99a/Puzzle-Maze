using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestDatas : MonoBehaviour
{
    public static QuestDatas instance;

    public List<QuestKill> questKills;
    public List<QuestUseItem> questUseItems;
    public List<QuestModel> questModels;

    public Dictionary<QuestType, List<QuestModel>> pairs = new Dictionary<QuestType, List<QuestModel>>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (var k in questKills)
        {
            if (!pairs.ContainsKey(k.questType))
                pairs.Add(k.questType, new List<QuestModel>());
            pairs[k.questType].Add(k);
        }
        foreach (var k in questUseItems)
        {
            if (!pairs.ContainsKey(k.questType))
                pairs.Add(k.questType, new List<QuestModel>());
            pairs[k.questType].Add(k);
        }
        foreach (var k in questModels)
        {
            if (!pairs.ContainsKey(k.questType))
                pairs.Add(k.questType, new List<QuestModel>());
            pairs[k.questType].Add(k);
        }
    }
}
