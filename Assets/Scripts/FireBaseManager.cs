using Assets.Scripts.Define;
using Firebase.Analytics;
//using Firebase.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBaseManager : MonoBehaviour
{
    static FireBaseManager _instance;

    public static FireBaseManager instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("FireBaseManager");
                _instance = go.AddComponent<FireBaseManager>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    //private bool firebaseAvailable;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("FireBaseManager");
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                CalculateRetention();

                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                //app = Firebase.FirebaseApp.DefaultInstance;
                // Set a flag here to indicate whether Firebase is ready to use by your app.
            }
            else
            {
                Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
            }
        });

        //StartCoroutine(CalculateRetentionIE());
    }

    private void CalculateRetention()
    {
        //float time = 0;
        //while (true)
        //{
        //    yield return new WaitForSeconds(0.5f);
        //    time += 0.5f;

        //    if (time >= 5)
        //        break;
        //}
        //if (!firebaseAvailable) yield break;
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            return;
        }

        //Debug.Log("CalculateRetention");
        var defaultDate = DateTime.Now.AddDays(-1000).Date;
        var lastDate = ES3.Load(StringDefine.LastPlayDate, defaultDate);
        //Debug.Log("lastDateStr:" + lastDateStr);
        if (lastDate == defaultDate)
        {
            ES3.Save(StringDefine.LastPlayDate, DateTime.Now.Date);
        }
        else
        {
            var days = (DateTime.Now.Date - lastDate).Days;
            Debug.Log("dayssssssss:" + days);
            if (days == 1 || days == 3 || days == 5 || days == 7)
            {
                string key = string.Format("{0}_{1}", StringDefine.Retention, days);
                if (ES3.Load(key, 0) == 0)
                {
                    ES3.Save(key, 1);
                    LogRetentionConversion(days, ES3.Load(StringDefine.CurrentLevel, 0));
                }
            }
        }
    }

    public void LogPlayLevel(int level)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Level", level.ToString())
        };
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseAnalytics.LogEvent("LogStartLevel", parameters);
        }
    }

    public void LogGameWin(int level)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Level", level.ToString())
        };
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseAnalytics.LogEvent("LogGameWin", parameters);
        }
    }

    public void LogGameOver(int level)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Level", level.ToString())
        };
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseAnalytics.LogEvent("LogGameOver", parameters);
        }
    }

    public void LogShowRewardedAd(string eventName)
    {
       Parameter[] parameters = new Parameter[]
        {
            new Parameter("Event", eventName)
        };

        if (Application.internetReachability != NetworkReachability.NotReachable && !string.IsNullOrEmpty(eventName))
        {
            FirebaseAnalytics.LogEvent("LogShowVideo", parameters);
        }
    }

    public void LogRewardedVideoAd(string eventName)
    {
      Parameter[] parameters = new Parameter[]
        {
            new Parameter("Event", eventName)
        };
        if (Application.internetReachability != NetworkReachability.NotReachable && !string.IsNullOrEmpty(eventName))
        {
            FirebaseAnalytics.LogEvent("LogRewardedVideo", parameters);
        }
    }

    public void LogShowIntersAd(string eventName, int level)
    {
      Parameter[] parameters = new Parameter[]
        {
            new Parameter("Event", eventName)
        };
        if (level > 0)
        {
            parameters = new Parameter[]
             {
                new Parameter("Event", eventName),
                new Parameter("Level", level)
             };
        }
        if (Application.internetReachability != NetworkReachability.NotReachable)
            FirebaseAnalytics.LogEvent("LogShowIntersAd", parameters);
    }

    public void LogClickIntersAd(string eventName, int level)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Event", eventName)
        };
        if (level > 0)
        {
            parameters = new Parameter[]
             {
                new Parameter("Event", eventName),
                new Parameter("Level", level)
             };
        }

        if (Application.internetReachability != NetworkReachability.NotReachable)
            FirebaseAnalytics.LogEvent("LogClickedIntersAd", parameters);
    }

    public void LogCloseIntersAd(string eventName, int level)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Event", eventName)
        };
        if (level > 0)
        {
            parameters = new Parameter[]
             {
                new Parameter("Event", eventName),
                new Parameter("Level", level)
             };
        }
        if (Application.internetReachability != NetworkReachability.NotReachable)
            FirebaseAnalytics.LogEvent("LogClosedIntersAd", parameters);
    }

    public void LogClickBanner()
    {
       if (Application.internetReachability != NetworkReachability.NotReachable)
            FirebaseAnalytics.LogEvent("LogClickBanner");
    }

    public void LogJoystickMode(int level)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Level", level.ToString())
        };
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseAnalytics.LogEvent("LogJoystick", parameters);
        }
    }

    public void LogGamepadMode(int level)
    {
       Parameter[] parameters = new Parameter[]
        {
            new Parameter("Level", level.ToString())
        };
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseAnalytics.LogEvent("LogGamepad", parameters);
        }
    }

    public void LogRate(string name)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Star", name)
        };
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            FirebaseAnalytics.LogEvent("LogRate", parameters);
        }
    }

    public void LogClickInApp(string name)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("item", name)
        };
        if (Application.internetReachability != NetworkReachability.NotReachable) FirebaseAnalytics.LogEvent("PurchaseInAppClicked", parameters);
    }

    public void LogPurchaseInApp(string name)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("item", name)     
        };
        if (Application.internetReachability != NetworkReachability.NotReachable) FirebaseAnalytics.LogEvent("PurchaseInAppSuccess", parameters);
    }

    public void LogRetentionConversion(int days, int curLevel)
    {
        Parameter[] parameters = new Parameter[]
        {
            new Parameter("Level", curLevel.ToString())
        };
        switch (days)
        {
            case 1:
                FirebaseAnalytics.LogEvent("d1Retention", parameters);
                break;
            case 3:
                FirebaseAnalytics.LogEvent("d3Retention", parameters);
                break;
            case 5:
                FirebaseAnalytics.LogEvent("d5Retention", parameters);
                break;
            case 7:
                FirebaseAnalytics.LogEvent("d7Retention", parameters);
                break;
        }
    }
}
