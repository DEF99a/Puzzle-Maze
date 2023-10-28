using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Define
{
    public enum EntityType
    {
        None,
        Box,
        BombBox,
        GiftBox,
        BallBox,
        Enemy,
        Player,

        ItemLife = 20,
        ItemAngry,
        ItemInvisible,
        ItemFreeze,
        ItemTimer5,
        ItemTimer10,
        ItemTimer20,
        ItemCoin100,
        ItemCoin200,
        ItemCoin500,
        ItemCoin1000,
        ItemCoin2000,
        ItemCoinRewardEndGameX2,
        ItemCoinRewardEndGameX3,
        ItemSpeed,
        ItemGun,
        ItemKillAll,

        ItemCoin,
        ItemTimer,
        ItemCoinRewardEndGameX,

        Mario2Bomb,

        BoxKey,
        BoxKey1,
        GroundKeyUnLock
    }
}