using Assets.Scripts.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelPause : MonoBehaviour
{
    public void ClickCloseBtn()
    {
        Time.timeScale = 1;
        IronSourceAd.instance.HideBanner();
        gameObject.SetActive(false);
    }

    public void Quit()
    {
        if (MapManager.instance.CurLevelNumber <= Config.LevelNotShowInter)
        {
            actionSuccess();
        }
        else
        {
            IronSourceAd.instance.ShowInterstitialAd("SettingQuit", MapManager.instance.CurLevelNumber, actionSuccess, actionSuccess);
        }

        void actionSuccess()
        {
            Time.timeScale = 1;
            IronSourceAd.instance.HideBanner();
            MapManager.instance.ResetMovePoints();
            ObjectPool.instance.DisableAll();
            SceneManager.LoadScene("Menu");
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        IronSourceAd.instance.HideBanner();
        gameObject.SetActive(false);
    }

    public void Replay()
    {
        if (MapManager.instance.CurLevelNumber <= Config.LevelNotShowInter)
        {
            actionSuccess();
        }
        else
        {
            IronSourceAd.instance.ShowInterstitialAd("SettingReplay", MapManager.instance.CurLevelNumber, actionSuccess, actionSuccess);
        }

        void actionSuccess()
        {
            Time.timeScale = 1;
            IronSourceAd.instance.HideBanner();
            MapManager.instance.ResetMovePoints();
            ObjectPool.instance.DisableAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void OnEnable()
    {
        IronSourceAd.instance.ShowBanner();
    }
}
