using Assets.Scripts.Helper;
using com.adjust.sdk;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronSourceAd : MonoBehaviour
{
    static IronSourceAd _instance;

    public static IronSourceAd instance
    {
        get
        {
            if(_instance == null)
            {
                var go = new GameObject("IronSourceAd");
                _instance = go.AddComponent<IronSourceAd>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    //private const string IRON_ANDROID_ID = "f2929461";
    public static string REWARDED_INSTANCE_ID = "0";

    private Action rewardAction;
    private Action interAction;

    private string rewaredEventName;
    private string interAdEventName;
    private int interAdAtLevel;

    public void Init()
    {
//#if UNITY_EDITOR
//        Debug.unityLogger.logEnabled = true;
//#else
//        Debug.unityLogger.logEnabled = false;
//#endif

        //if (FB.IsInitialized)
        //{
        //    FB.ActivateApp();
        //}
        //else
        //{
        //    FB.Init(() =>
        //    {
        //        FB.ActivateApp();
        //    });
        //}

        //IronSource.Agent.init(IRON_ANDROID_ID, IronSourceAdUnits.REWARDED_VIDEO,
        //    IronSourceAdUnits.INTERSTITIAL, IronSourceAdUnits.OFFERWALL, IronSourceAdUnits.BANNER);


        IronSourceEvents.onRewardedVideoAdClosedEvent += onRewardedVideoAdClosedEvent;
        IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
        IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;


        IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
        IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;

        IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;

        var developerSettings = Resources.Load<IronSourceMediationSettings>(IronSourceConstants.IRONSOURCE_MEDIATION_SETTING_NAME);
        if (developerSettings != null)
        {
            string appKey = "";
#if UNITY_ANDROID
            appKey = developerSettings.AndroidAppKey;
#elif UNITY_IOS
            appKey = developerSettings.IOSAppKey;
#endif
            Debug.Log("App key :" + appKey);

            if (appKey.Equals(string.Empty))
            {
                Debug.LogWarning("IronSourceInitilizer Cannot init without AppKey");
            }
            else
            {
                IronSource.Agent.init(appKey);
            }
        }

        //init adjust
        AdjustConfig adjustConfig = new AdjustConfig("tq5lv5v9o6ww", AdjustEnvironment.Production, true);
        Adjust.start(adjustConfig);
        LoadInterstitialAds();
        LoadRewardedAds();
    }


    private void Start()
    {
        //IronSource.Agent.loadInterstitial();
        //IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
    }

    void OnApplicationPause(bool pauseStatus)
    {
        // Check the pauseStatus to see if we are in the foreground
        // or background
        if (!pauseStatus)
        {
            //app resume
            //if (FB.IsInitialized)
            //{
            //    FB.ActivateApp();
            //}
            //else
            //{
            //    //Handle FB.Init
            //    FB.Init(() =>
            //    {
            //        FB.ActivateApp();
            //    });
            //}
        }
    }

    private void BannerAdClickedEvent()
    {
        FireBaseManager.instance.LogClickBanner();
    }

    private void onRewardedVideoAdClosedEvent()
    {
        LoadRewardedAds();
    }

    private void LoadRewardedAds()
    {
        Debug.Log(">>LoadRewardedAds.......");
        IronSource.Agent.loadISDemandOnlyRewardedVideo(REWARDED_INSTANCE_ID);
    }

    private void LoadInterstitialAds()
    {
        Debug.Log("LoadInterstitialAds.......");
        IronSource.Agent.loadInterstitial();
    }

    private void InterstitialAdClosedEvent()
    {
        if (this.interAction != null) this.interAction();

        IronSource.Agent.loadInterstitial();
        FireBaseManager.instance.LogCloseIntersAd(interAdEventName, interAdAtLevel);
    }

    private void InterstitialAdOpenedEvent()
    {
        FireBaseManager.instance.LogShowIntersAd(interAdEventName, interAdAtLevel);
    }


    private void InterstitialAdClickedEvent()
    {
        FireBaseManager.instance.LogClickIntersAd(interAdEventName, interAdAtLevel);
    }

    private void RewardedVideoAdOpenedEvent()
    {
        FireBaseManager.instance.LogShowRewardedAd(this.rewaredEventName);
    }

    private void RewardedVideoAdRewardedEvent(IronSourcePlacement obj)
    {
        if (this.rewardAction != null)
        {
            this.rewardAction();
            this.rewardAction = null;
        }
        FireBaseManager.instance.LogRewardedVideoAd(this.rewaredEventName);
    }

    public void ShowRewardedVideo(string eventName, Action actionWaiting, Action actionSuccess, Action actionFailed)
    {
        this.rewaredEventName = eventName;
#if UNITY_EDITOR
        actionSuccess();
#else
    ShowOnAndroid();
#endif
        void ShowOnAndroid()
        {
            this.rewardAction = actionSuccess;
            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                Debug.Log("Ad Failed");
                actionWaiting();
                StartCoroutine(WaittingRewardedVideoIE(actionFailed));
            }
        }
    }

    IEnumerator WaittingRewardedVideoIE(Action actionFailed)
    {
        float time = 2f;
        while (time > 0)
        {
            yield return new WaitForSeconds(0.2f);
            time -= 0.2f;

            if (IronSource.Agent.isRewardedVideoAvailable())
            {
                IronSource.Agent.showRewardedVideo();
                yield break;
            }
        }
        actionFailed?.Invoke();
    }

    public void ShowInterstitialAd(string eventName, int level, Action actionSuccess, Action actionFail)
    {
        this.interAdEventName = eventName;
        this.interAdAtLevel = level;

        if (PurchaseHelper.HasRemoveAd())
        {
            actionSuccess?.Invoke();
            return;
        }

#if UNITY_EDITOR
        actionSuccess?.Invoke();
#else
    ShowOnAndroid();
#endif
        void ShowOnAndroid()
        {
            interAction = actionSuccess;
            if (IronSource.Agent.isInterstitialReady())
            {
                IronSource.Agent.showInterstitial();
            }
            else
                actionFail?.Invoke();
        }
    }

    public void LoadBanner(bool onBottom = true)
    {
        IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, onBottom ? IronSourceBannerPosition.BOTTOM : IronSourceBannerPosition.TOP);
    }

    public void ShowBanner()
    {
        IronSource.Agent.displayBanner();
    }

    public void HideBanner()
    {
        IronSource.Agent.hideBanner();
    }
}
