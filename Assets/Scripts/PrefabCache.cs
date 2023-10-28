using Assets.Scripts.Common;
using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class EnemyTypePrefab
{
    public EnemyType enemyType;
    public GameObject pf;
}

[System.Serializable]
public class PlayerTypePrefab
{
    public PlayerType enemyType;
    public GameObject pf;
}

public class PrefabCache : MonoBehaviour
{
    public static PrefabCache instance;
    public List<EnemyTypePrefab> enemyTypePrefabs;
    public List<PlayerTypePrefab> playerTypePrefabs;
    public List<PlayerTypePrefab> playerTypePrefabsIOS;

    public GameObject boxPf;
    public GameObject bombBoxPf;
    public GameObject giftBoxPf;
    public GameObject ballBoxPf;
    public GameObject stonePf;
    public GameObject balloonPf;
    public GameObject itemSpritePf;
    public GameObject iceEffectPf;
    public GameObject itemOnmapPf;
    public GameObject boxKeyPf;
    public GameObject boxKey1Pf;
    public GameObject groundKeyPf;
    public GameObject boxTreePf;
    public GameObject boxWaterMelon;

    public GameObject coinFlyUp;
    public GameObject poisonPf;


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

#if UNITY_ANDROID
        Debug.Log("UNITY_ANDROID");
#endif

#if UNITY_EDITOR
        Debug.Log("Unity Editor");
#elif UNITY_ANDROID
    Debug.Log("Unity iPhone");
#else
    Debug.Log("Any other platform");
#endif
    }

    public GameObject GetEnemyPrefab(EnemyType enemyType)
    {
        return enemyTypePrefabs.First(ep => ep.enemyType == enemyType).pf;
    }

    public GameObject GetPlayerPrefab(PlayerType enemyType)
    {
        var pp = playerTypePrefabs;
        if(Config.IsIOS) pp = playerTypePrefabsIOS;

        return pp.First(ep => ep.enemyType == enemyType).pf;
    }
}
