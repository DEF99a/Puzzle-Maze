using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;


namespace QMG
{
	[DisallowMultipleComponent]
	public sealed class IGExporter : MonoBehaviour
	{
        public static string globalGoName = "";

        public static System.Action shareAsync_Callback = null;

        public static System.Action ShowInterstitialAd_Preload_Method = null;

        public static System.Action ShowRewaredVideoAd_Preload_Method = null;
        public static System.Action ShowRewaredVideoAd_Complete_Callback = null;
        public static System.Action<FBError> ShowRewaredVideoAd_Error_Callback = null;

        public static System.Action<bool> PreloadRewaredVideoAd_Ready_Callback = null;

        public static System.Type gameLogicEntryTypeName = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void IGExporterGameStart()
		{
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return;
            }

			GameObject go = new GameObject();
			go.name = IGExporter.globalGoName;
			DontDestroyOnLoad(go);

			go.AddComponent<IGExporter>();
        }

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void IGExporterGameEnd()
		{
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                return;
            }
        }

        #region Common Promise Callback
        private void Promise_on_context_chooseAsync()
        {
            if (GameContext.chooseAsync_Callback != null)
            {
                GameContext.chooseAsync_Callback();
            }
        }

        private void Promise_on_context_inviteAsync()
        {

        }

        private void Promise_on_context_getPlayersAsync(string jsonStr)
        {
            if (GameContext.getPlayersAsync_Callback != null)
            {
                ContextPlayerEntry[] playerArr = SimpleJson.SimpleJson.DeserializeObject<ContextPlayerEntry[]>(jsonStr);
                GameContext.getPlayersAsync_Callback(playerArr);
            }
        }

        private void Promise_on_player_getDataAsync(string jsonStr)
        {
            Debug.Log("asdasd: " + jsonStr);
            Dictionary<string, object> data = SimpleJson.SimpleJson.DeserializeObject<Dictionary<string, object>>(jsonStr);
            if (ContextPlayer.getDataAsync_Callback != null)
            {
                ContextPlayer.getDataAsync_Callback(data);
            }
        }

        private void Promise_on_player_setDataAsync()
        {
            if (ContextPlayer.setDataAsync_Callback != null)
            {
                ContextPlayer.setDataAsync_Callback();
            }
        }

        private void Promise_on_player_canSubscribeBotAsync(string jsonStr)
        {
            bool can_subscribe = SimpleJson.SimpleJson.DeserializeObject<bool>(jsonStr);
            if (ContextPlayer.canSubscribeBotAsync_Callback != null)
            {
                ContextPlayer.canSubscribeBotAsync_Callback(can_subscribe);
            }
        }

        private void Promise_on_player_subscribeBotAsync_Success()
        {
            if (ContextPlayer.subscribeBot_Success_Callback != null)
            {
                ContextPlayer.subscribeBot_Success_Callback();
            }
        }

        private void Promise_on_player_subscribeBotAsync_Error(string errJsonStr)
        {
            if (ContextPlayer.subscribeBot_Error_Callback != null)
            {
                FBError err = SimpleJson.SimpleJson.DeserializeObject<FBError>(errJsonStr);
                ContextPlayer.subscribeBot_Error_Callback(err);
            }
        }

        private void Promise_on_fbinstant_shareAsync()
        {
            if (shareAsync_Callback != null)
            {
                shareAsync_Callback();
            }
        }

        private void Promise_on_fbinstant_showInterstitialAdAsync()
        {
            if (ShowInterstitialAd_Preload_Method != null)
            {
                ShowInterstitialAd_Preload_Method();
            }
        }

        private void Promise_on_fbinstant_showRewardedVideoAsync_Complete()
        {
            if (ShowRewaredVideoAd_Complete_Callback != null)
            {
                ShowRewaredVideoAd_Complete_Callback();
            }
        }

        private void Promise_on_fbinstant_showRewardedVideoAsync_Error(string errJsonStr)
        {
            if (ShowRewaredVideoAd_Error_Callback != null)
            {
                FBError err = SimpleJson.SimpleJson.DeserializeObject<FBError>(errJsonStr);
                ShowRewaredVideoAd_Error_Callback(err);
            }
        }

        private void Promise_on_fbinstant_showRewardedVideoAsync_Preload()
        {
            if (ShowRewaredVideoAd_Preload_Method != null)
            {
                ShowRewaredVideoAd_Preload_Method();
            }
        }

        private void Promise_on_fbinstant_getRewardedVideoAsync(string jsonStr)
        {
            bool isReady = SimpleJson.SimpleJson.DeserializeObject<bool>(jsonStr);
            if (PreloadRewaredVideoAd_Ready_Callback != null)
            {
                PreloadRewaredVideoAd_Ready_Callback(isReady);
            }
        }
        #endregion
	}

}
