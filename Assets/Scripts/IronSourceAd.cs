using Assets.Scripts.Helper;
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


    public void ShowRewardedVideo(string eventName, Action actionWaiting, Action actionSuccess, Action actionFailed)
    {
        actionSuccess?.Invoke();
    }

    public void ShowInterstitialAd(string eventName, int level, Action actionSuccess, Action actionFail)
    {
        actionSuccess?.Invoke();
    }

    public void LoadBanner(bool onBottom = true)
    {
    }

    public void ShowBanner()
    {
    }

    public void HideBanner()
    {
    }
}
