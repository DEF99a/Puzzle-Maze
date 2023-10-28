using Assets.Scripts.Common;
using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDatas : MonoBehaviour
{
    public static PlayerDatas instance;
    [SerializeField] private List<PlayerData> playerDatas;
    [SerializeField] private List<PlayerData> playerDatasIOS;

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
        }
    }

    public List<PlayerData> GetPlayerDatas()
    {
        var pp = playerDatas;
        if (Config.IsIOS) pp = playerDatasIOS;

        return pp;
    }

    public PlayerData GetPlayerData(PlayerType playerType)
    {
        var pp = playerDatas;
        if (Config.IsIOS) pp = playerDatasIOS;

        return pp.FirstOrDefault(p => p.playerType == playerType);
    }

    public List<PlayerData> GetPlayerDatas(PlayerRarity playerType)
    {
        var pp = playerDatas;
        if (Config.IsIOS) pp = playerDatasIOS;

        return pp.Where(p => p.playerRarity == playerType).ToList();
    }
}
