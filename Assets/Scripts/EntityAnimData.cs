using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

[System.Serializable]
public class EntityAnim
{
    public EntityType entityType;
    public SkeletonDataAsset skeletonData;
}


[System.Serializable]
public class PlayerAnim
{
    public PlayerType playerType;
    public SkeletonDataAsset skeletonData;
}


[System.Serializable]
public class BoxAnim
{
    public BoxType boxType;
    public SkeletonDataAsset skeletonData;
}


public class EntityAnimData : MonoBehaviour
{
    public static EntityAnimData instance;

    public List<PlayerAnim> playerAnims;
    public List<BoxAnim> boxAnims;

    [SerializeField] private List<EntityAnim> data;

    private Dictionary<EntityType, SkeletonDataAsset> pair;
    private Dictionary<PlayerType, SkeletonDataAsset> playerPair;
    private Dictionary<BoxType, SkeletonDataAsset> boxPair;

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

        pair = new Dictionary<EntityType, SkeletonDataAsset>();
        foreach (var es in data)
        {
            pair.Add(es.entityType, es.skeletonData);
        }

        playerPair = new Dictionary<PlayerType, SkeletonDataAsset>();
        foreach (var es in playerAnims)
        {
            playerPair.Add(es.playerType, es.skeletonData);
        }

        boxPair = new Dictionary<BoxType, SkeletonDataAsset>();
        foreach (var es in boxAnims)
        {
            boxPair.Add(es.boxType, es.skeletonData);
        }
    }

    public SkeletonDataAsset GetAnimData(EntityType entityType)
    {
        return pair[entityType];
    }

    public SkeletonDataAsset GetPlayerAnimData(PlayerType entityType)
    {
        return playerPair[entityType];
    }

    public SkeletonDataAsset GetBoxAnimData(BoxType entityType)
    {
        if (boxPair.ContainsKey(entityType) && boxPair[entityType] != null)
            return boxPair[entityType];
        return null;
    }
}
