using System.Collections;
using System.Collections.Generic;
using com.adjust.sdk;
using Facebook.Unity;
using UnityEngine;

public class CommonInit : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_IOS
        if (EasyMobile.Privacy.AppTrackingManager.TrackingAuthorizationStatus == EasyMobile.AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusNotDetermined)
        {
            EasyMobile.Privacy.AppTrackingManager.RequestTrackingAuthorization(status =>
            {
                if (status == EasyMobile.AppTrackingAuthorizationStatus.ATTrackingManagerAuthorizationStatusAuthorized)
                    AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(true);
                else
                    AudienceNetwork.AdSettings.SetAdvertiserTrackingEnabled(false);

                InitComponent();
            });
        }
        else
            InitComponent();
#else
InitComponent();
#endif
    }

    void InitComponent()
    {
        Debug.Log("Begin InitComponent");
        if (!FB.IsInitialized)
        {
            // Initialize the Facebook SDK
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
            });
        }
        else
        {
            // Already initialized, signal an app activation App Event
            FB.ActivateApp();
        }

        IronSourceAd.instance.Init();

        Debug.Log("AdId:" + IronSource.Agent.getAdvertiserId());

        Debug.Log("End InitComponent");

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
