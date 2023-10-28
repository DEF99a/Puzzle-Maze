using Assets.Scripts.Define;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class Config
    {
        public static int LimitWhile = 1000;

        public static int CoinRemoveBox = 1;
        public static int CoinRemoveBomBox = 10;
        public static int CoinRemoveBallBox = 10;
        public static int CoinRemoveGiftBox = 20;
        public static int CoinRemoveGiftBoxByBom =50;
        //public static int CoinKillEnemy = 100;
        public static int CoinFirstPassLevel = 100;

        public static float SpeedUpPercent = 10;
        public static List<int> SpeedUpPrices = new List<int>() {2000,
4000,
8000,
16000,
32000,
64000,
128000,
256000,
512000,
1024000,
2048000};

        public static List<int> LifeUpPrices = new List<int>() {
20,
40,
80,
160,
320,
640,
1280,
2560,
5120,
10240};

        public static int CoinViewAdAtHome = 1000;
        public static int CoinViewAdAtShop = 1000;

        public static string DatetimeFormat = "yyyy/MM/dd HH:mm:ss";

        public static int SelectMap = 0;

        public static int GemRewardCompletedQuest = 20;
        public static int FirstLevel = 1;
        public static int LevelNotShowInter = 2;               

        public static PlayerType SelectPlayerType = PlayerType.None;

#if UNITY_EDITOR || UNITY_ANDROID
        public static PlayerType playerTypeDefault = PlayerType.Main1;
        public static PlayerType PlayerTypeFromWeeklyPackage = PlayerType.Main2;
        public static bool IsIOS = false;

        //public static PlayerType playerTypeDefault = PlayerType.Main1;
        //public static PlayerType PlayerTypeFromWeeklyPackage = PlayerType.Main1;
        //public static bool IsIOS = true;
#else
    public static PlayerType playerTypeDefault = PlayerType.Main1;
    public static PlayerType PlayerTypeFromWeeklyPackage = PlayerType.Main1;
    public static bool IsIOS = true;
#endif
    }
}