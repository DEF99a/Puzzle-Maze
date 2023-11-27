using System.Runtime.InteropServices;
using System.Collections.Generic;

public static class FBInstant
{
    [DllImport("__Internal")]
    public static extern string fbinstant_getSupportedAPIs();

    [DllImport("__Internal")]
    public static extern void fbinstant_switchGameAsync(string appId, string entryPointDataJsonStr);

    [DllImport("__Internal")]
    public static extern void logEvent(string eventName, long valueToSum, string jsonStr);

    [DllImport("__Internal")]
    public static extern string getEntryPointData();

    [DllImport("__Internal")]
    public static extern string getLocale();

    [DllImport("__Internal")]
    public static extern void fbinstant_updateAsync(string jsonStr);

    [DllImport("__Internal")]
    public static extern void fbinstant_shareAsync(string jsonStr);

    [DllImport("__Internal")]
    public static extern void fbinstant_getInterstitialAdAsync(string adId);

    [DllImport("__Internal")]
    public static extern void fbinstant_showInterstitialAdAsync();

    [DllImport("__Internal")]
    public static extern void fbinstant_getRewardedVideoAsync(string adId);

    [DllImport("__Internal")]
    public static extern void fbinstant_showRewardedVideoAsync();

    [DllImport("__Internal")]
    public static extern void shareMessenger();

    //[DllImport("__Internal")]
    //public static extern void creatShortcut();

    //[DllImport("__Internal")]
    //public static extern void inviteFB();


    private static List<string> supportedApis = null; // use memory cache
    public static List<string> getSupportedAPIs()
    {
        if (supportedApis != null)
        {
            return supportedApis;
        }

        string apiJsonStr = fbinstant_getSupportedAPIs();
        string[] apiList = SimpleJson.SimpleJson.DeserializeObject<string[]>(apiJsonStr);
        supportedApis = new List<string>(apiList);
        return supportedApis;
    }

    /// <summary>
    /// Only Support on Web & Android
    /// </summary>
    /// <param name="appId"></param>
    /// <param name="entryPointData"></param>
    public static void switchGameAsync(string appId, Dictionary<string, object> entryPointData)
    {
        fbinstant_switchGameAsync(appId, SimpleJson.SimpleJson.SerializeObject(entryPointData));
    }

    public static void shareAsync(Dictionary<string, object> p, System.Action cb)
    {
        shareAsync_Callback = cb;
        fbinstant_shareAsync(SimpleJson.SimpleJson.SerializeObject(p));
    }

    public static void updateAsync(Dictionary<string, object> p)
    {
        fbinstant_updateAsync(SimpleJson.SimpleJson.SerializeObject(p));
    }

    public static void PreloadInterstitialAd(string adId)
    {
        fbinstant_getInterstitialAdAsync(adId);
    }

    public static void ShowInterstitialAd(System.Action preloadMethod)
    {
        ShowInterstitialAd_Preload_Method = preloadMethod;
        fbinstant_showInterstitialAdAsync();
    }

    public static void PreloadRewaredVideoAd(string adId, System.Action<bool> onReadCallback = null)
    {
        PreloadRewaredVideoAd_Ready_Callback = onReadCallback;
        fbinstant_getRewardedVideoAsync(adId);
    }

    public static void ShowRewaredVideoAd(System.Action completeCallback, System.Action<FBError> errorCallback, System.Action preloadMethod)
    {
        ShowRewaredVideoAd_Complete_Callback = completeCallback;
        ShowRewaredVideoAd_Error_Callback = errorCallback;
        ShowRewaredVideoAd_Preload_Method = preloadMethod;
        fbinstant_showRewardedVideoAsync();
    }

    public static void ShareMessenger()
    {
        shareMessenger();
    }

    public static System.Action shareAsync_Callback = null;

    public static System.Action ShowInterstitialAd_Preload_Method = null;

    public static System.Action ShowRewaredVideoAd_Preload_Method = null;
    public static System.Action ShowRewaredVideoAd_Complete_Callback = null;
    public static System.Action<FBError> ShowRewaredVideoAd_Error_Callback = null;

    public static System.Action<bool> PreloadRewaredVideoAd_Ready_Callback = null;
}
