using Assets.Scripts.Common;
using Assets.Scripts.Define;
using Spine.Unity;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Hellper
{
    public class PlayerHelper
    {
        //public static float GetCurrentPlayerSpeed()
        //{
        //    var currentPlayer = GetCurrentPlayer();
        //    return PlayerDatas.instance.GetPlayerData(currentPlayer.playerType).speed * (1 + currentPlayer.speedUpPercent * 15f / 2000);
        //}

        //public static int GetCurrentPlayerLife()
        //{
        //    var currentPlayer = GetCurrentPlayer();
        //    return PlayerDatas.instance.GetPlayerData(currentPlayer.playerType).life + currentPlayer.lifeUpPercent;
        //}

        public static float GetPlayerSpeed(PlayerType playerType)
        {
            var players = GetPlayers();
            var player = players.FirstOrDefault(p => p.playerType == playerType);
            float more = 0f;
            if (player != null) more = player.speedUpPercent * Config.SpeedUpPercent / 2000;

            return PlayerDatas.instance.GetPlayerData(playerType).speed * (1 + more);
        }

        public static int GetPlayerLife(PlayerType playerType)
        {
            var players = GetPlayers();
            var player = players.FirstOrDefault(p => p.playerType == playerType);
            int more = 0;
            if (player != null) more = player.lifeUpPercent;

            return PlayerDatas.instance.GetPlayerData(playerType).life + more;
        }

        public static float GetPlayerSpeed(PlayerSaver playerSaver)
        {
            return PlayerDatas.instance.GetPlayerData(playerSaver.playerType).speed * (1 + playerSaver.speedUpPercent * Config.SpeedUpPercent / 2000);
        }

        public static int GetPlayerLife(PlayerSaver playerSaver)
        {
            return PlayerDatas.instance.GetPlayerData(playerSaver.playerType).life + playerSaver.lifeUpPercent;
        }

        public static PlayerSaver GetCurrentPlayer()
        {
#if UNITY_EDITOR
            return ES3.Load(StringDefine.CurrentPlayer, new PlayerSaver() { playerType = PlayerType.Main1 });
#else
        return ES3.Load(StringDefine.CurrentPlayer, new PlayerSaver());
#endif
        }

        public static void UpdatePlayer(PlayerSaver playerSaver)
        {
            ES3.Save(StringDefine.CurrentPlayer, playerSaver);
            var players = GetPlayers();
            var inList = players.FirstOrDefault(p => p.playerType == playerSaver.playerType);
            if (inList == null)
                players.Add(playerSaver);
            else
            {
                inList.Copy(playerSaver);
            }
            ES3.Save(StringDefine.Players, players);
        }

        public static void AddPlayer(PlayerSaver playerSaver)
        {
            var players = GetPlayers();
            var inList = players.FirstOrDefault(p => p.playerType == playerSaver.playerType);
            if (inList == null)
                players.Add(playerSaver);
            else
            {
                inList.Copy(playerSaver);
            }
            ES3.Save(StringDefine.Players, players);
        }

        public static void RemovePlayer(PlayerType playerType)
        {
            var players = GetPlayers();
            var inList = players.FirstOrDefault(p => p.playerType == playerType);
            if (inList != null)
                players.Remove(inList);
            ES3.Save(StringDefine.Players, players);
        }

        public static bool HasPlayer(PlayerType playerType)
        {
            var players = GetPlayers();
            var inList = players.FirstOrDefault(p => p.playerType == playerType);
            if (inList != null)
                return true;
            return false;
        }

        public static List<PlayerSaver> GetPlayers()
        {
#if UNITY_EDITOR
            return ES3.Load(StringDefine.Players, new List<PlayerSaver>() { new PlayerSaver() { playerType = PlayerType.Main1 } });
#else
            return ES3.Load(StringDefine.Players, new List<PlayerSaver>());
#endif
        }

        public static void LoadSkeleton(PlayerType playerType, SkeletonGraphic skeletonGraphic, string name = "", bool loop = true)
        {
            skeletonGraphic.Clear();

            Debug.Log("Playertype:" + playerType);

            var playerPrefab = PrefabCache.instance.GetPlayerPrefab(playerType);
            var skeletonDataAsset = playerPrefab.transform.GetChild(0).GetComponent<SkeletonAnimation>().skeletonDataAsset;

            //var skeletonDataAsset = EntityAnimData.instance.GetPlayerAnimData(playerType);
            skeletonGraphic.skeletonDataAsset = skeletonDataAsset;
            skeletonGraphic.Initialize(true);
            if (name == "") name = "front_idle";
            skeletonGraphic.AnimationState.SetAnimation(0, name, loop);
            skeletonGraphic.SetMaterialDirty();
        }
    }
}