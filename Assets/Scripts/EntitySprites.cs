using Assets.Scripts.Define;
using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EntitySprite
{
    public EntityType entityType;
    public Sprite sprite;
}

[System.Serializable]
public class QuestSprite
{
    public QuestType questType;
    public EnemyType enemyType;
    public EntityType entityType;
    public Sprite sprite;
}

[System.Serializable]
public class DoorOpenSprite
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public class PlayerSprite
{
    public Sprite sprite;
    public PlayerType playerType;
}

[System.Serializable]
public class SkillSprite
{
    public Sprite sprite;
    public PlayerType playerType;
}


public class EntitySprites : MonoBehaviour
{
    public static EntitySprites instance;

    [SerializeField] private List<EntitySprite> data;
    [SerializeField] private List<QuestSprite> questSprites;
    [SerializeField] private List<PlayerSprite> playerSprites;
    [SerializeField] private List<SkillSprite> skillSprites;

    public List<DoorOpenSprite> doorOpenSprites;

    private Dictionary<EntityType, Sprite> pair;
    private Dictionary<PlayerType, Sprite> playerSpritePair;

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

        pair = new Dictionary<EntityType, Sprite>();
        foreach (var es in data)
        {
            pair.Add(es.entityType, es.sprite);
        }

        playerSpritePair = new Dictionary<PlayerType, Sprite>();
        foreach (var es in playerSprites)
        {
            playerSpritePair.Add(es.playerType, es.sprite);
        }
    }

    public Sprite GetSprite(EntityType entityType)
    {
        return pair[entityType];
    }

    public Sprite GetQuestSprite(QuestType questType, EnemyType enemyType, EntityType entityType)
    {
        QuestSprite quest;
        if (questType == QuestType.KillEnemy)
        {
            quest = questSprites.FirstOrDefault(q => q.questType == questType && q.enemyType == enemyType);
        }
        else if (questType == QuestType.UseItem)
        {
            quest = questSprites.FirstOrDefault(q => q.questType == questType && q.entityType == entityType);
        }
        else
        {
            quest = questSprites.FirstOrDefault(q => q.questType == questType);
        }
        if (quest != null) return quest.sprite;

        return null;
    }

    public Sprite GetPlayerSprite(PlayerType entityType)
    {
        return playerSpritePair[entityType];
    }

    public Sprite GetSkillSprite(PlayerType entityType)
    {
        var pf = skillSprites.FirstOrDefault(ss => ss.playerType == entityType);
        return pf != null ? pf.sprite : null;
    }
}
