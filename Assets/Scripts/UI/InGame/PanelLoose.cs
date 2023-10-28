using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Assets.Scripts.Helper;
using Spine.Unity;
using Assets.Scripts.Hellper;
using Assets.Scripts.Common;

public class PanelLoose : MonoBehaviour
{
    [SerializeField] private RectTransform opportunityRect;
    [SerializeField] private Button opportunityCloseBtn;
    [SerializeField] private TextMeshProUGUI timeTxt;

    //[SerializeField] private RectTransform failedRect;
    //[SerializeField] private SkeletonGraphic skeletonGraphic;

    [SerializeField] private RectTransform hasLifeRect;
    //[SerializeField] private TextMeshProUGUI lifeBuyTxt;
    [SerializeField] private TextMeshProUGUI yourCoinTxt;
    [SerializeField] private TextMeshProUGUI needCoinTxt;
    [SerializeField] private TextMeshProUGUI yourLifeTxt;
    [SerializeField] private Button payCoinBtn;
    [SerializeField] private Image timeOuter;

    //[SerializeField] private RectTransform buyLifeRect;
    //[SerializeField] private RectTransform shopRect;

    [SerializeField] private PanelLoadingAd panelLoadingAd;

    private Coroutine coroutine;

    //private void OnEnable()
    //{
    //    Show(GameEndReason.PlayerOutLife);
    //}

    public void Show(GameEndReason gameEndReason)
    {
        GameManager.instance.Stop();

        AudioManager.instance.StopMusic(MapManager.instance.GameMusicStr);
        AudioManager.instance.PlaySound(SoundName.level_fail, 0.2f);

        gameObject.SetActive(true);
        FireBaseManager.instance.LogGameOver(MapManager.instance.CurLevelNumber);

        hasLifeRect.gameObject.SetActive(false);
        //buyLifeRect.gameObject.SetActive(false);
        //shopRect.gameObject.SetActive(false);
        //if (gameEndReason == GameEndReason.OutBox || gameEndReason == GameEndReason.AllEnemyDie || GameManager.instance?.useLifeAmount >= 1)
        //{
        //    Debug.Log("aaaaaaaaa");
        //    opportunityRect.gameObject.SetActive(false);
        //    failedRect.gameObject.SetActive(true);

        //    PlayerHelper.LoadSkeleton(PlayerHelper.GetCurrentPlayer().playerType, skeletonGraphic, "die_start", false);
        //}
        //else
        //{
        //    failedRect.gameObject.SetActive(false);
            opportunityRect.gameObject.SetActive(true);
            coroutine = StartCoroutine(Run());
        //}
        IronSourceAd.instance.ShowBanner();

    }

    private IEnumerator Run()
    {
        int i = 10;
        timeTxt.text = i.ToString();
        opportunityCloseBtn.gameObject.SetActive(false);
        timeOuter.fillAmount = 1f;

        while (i > 0)
        {
            yield return new WaitForSeconds(1f);
            i -= 1;
            timeOuter.fillAmount = i * 1f / 10;

            AudioManager.instance?.PlaySound(SoundName.clock_count_lose);

            timeTxt.text = i.ToString();
            if (i == 7)
                opportunityCloseBtn.gameObject.SetActive(true);
        }
        timeOuter.fillAmount = 0f;

        opportunityRect.gameObject.SetActive(false);
        var life = ES3.Load(StringDefine.LifeBuy, 0);
        //if (life > 0)
        //{
        ShowHasLifePopup(life);
        //}
        //else buyLifeRect.gameObject.SetActive(true);
    }

    public void OpportunityClose()
    {
        StopCoroutine(coroutine);

        opportunityRect.gameObject.SetActive(false);
        var life = ES3.Load(StringDefine.LifeBuy, 0);
        //if (life > 0)
        //{
            ShowHasLifePopup(life);
        //}
        //else buyLifeRect.gameObject.SetActive(true);
    }

    private void ShowHasLifePopup(int life)
    {
        int needLife = life >= 3 ? 0 : 3 - life;
        int needCoin = needLife * 500;

        yourLifeTxt.text = string.Format("{0}", life);
        yourCoinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
        needCoinTxt.text = needCoin.ToString();

        hasLifeRect.gameObject.SetActive(true);

        if ( needLife == 0 || MoneyHelper.GetMoney(false) >= needCoin)
        {
            payCoinBtn.interactable = true;

            payCoinBtn.onClick.RemoveAllListeners();
            payCoinBtn.onClick.AddListener(() =>
            {
                AudioManager.instance.PlaySound(SoundName.click_btn_common);
                if (life <= 3) ES3.Save(StringDefine.LifeBuy, 0);
                else ES3.Save(StringDefine.LifeBuy, life - 3);
                MoneyHelper.AddMoney(-needCoin, false);
                gameObject.SetActive(false);
                GameManager.instance.Continue(3);
            });
        }
        else payCoinBtn.interactable = false;
    }

    public void OpportunityViewAd(int life)
    {
        void actionPrepare()
        {
            panelLoadingAd.StartLoading();
        }
        void actionFailed()
        {
            panelLoadingAd.StopLoading();
        }
        void actionSuccess()
        {
            gameObject.SetActive(false);
            IronSourceAd.instance.HideBanner();

            GameManager.instance.Continue(life);
        }
        IronSourceAd.instance.ShowRewardedVideo("GameLooseOpportunity", actionPrepare, actionSuccess, actionFailed);
    }

    //public void ClickShop()
    //{
    //    buyLifeRect.gameObject.SetActive(false);
    //    shopRect.gameObject.SetActive(true);
    //}

    //public void ClickCloseShop()
    //{
    //    int life = ES3.Load(StringDefine.LifeBuy, 0);
    //    if (life > 0)
    //    {
    //        hasLifeRect.gameObject.SetActive(true);
    //        lifeBuyTxt.text = life.ToString();
    //    }
    //    else buyLifeRect.gameObject.SetActive(true);

    //    shopRect.gameObject.SetActive(false);
    //}

    //public void ClickBuyShop(int life)
    //{
    //    int price;
    //    if (life == 1) price = 500;
    //    else price = 1000;

    //    if (MoneyHelper.GetMoney(isGem: false) >= price)
    //    {
    //        ES3.Save(StringDefine.LifeBuy, life + ES3.Load(StringDefine.LifeBuy, 0));
    //        MoneyHelper.AddMoney(-price, isGem: false);
    //    }
    //}

    public void Quit()
    {
        IronSourceAd.instance.HideBanner();
        MapManager.instance.ResetMovePoints();
        ObjectPool.instance.DisableAll();

        void actionSuccess()
        {
            SceneManager.LoadScene("Menu");
        }
        IronSourceAd.instance.ShowInterstitialAd("GameLooseQuit", MapManager.instance.CurLevelNumber, actionSuccess, actionSuccess);
    }

    public void Replay()
    {
        IronSourceAd.instance.HideBanner();
        MapManager.instance.ResetMovePoints();
        ObjectPool.instance.DisableAll();

        void actionSuccess()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        IronSourceAd.instance.ShowInterstitialAd("GameLooseReplay", MapManager.instance.CurLevelNumber, actionSuccess, actionSuccess);
    }

    public void OnClickContinue1Life()
    {
        void actionPrepare()
        {
            panelLoadingAd.StartLoading();
        }
        void actionFailed()
        {
            panelLoadingAd.StopLoading();
        }
        void actionSuccess()
        {
            IronSourceAd.instance.HideBanner();
            gameObject.SetActive(false);
            GameManager.instance.Continue(1);
        }
        IronSourceAd.instance.ShowRewardedVideo("GameLooseContinue1Life", actionPrepare, actionSuccess, actionFailed);
    }

    //public void OnClickContinue()
    //{
    //    int life = ES3.Load(StringDefine.LifeBuy, 0);
    //    if (life < 0) return;

    //    gameObject.SetActive(false);
    //    GameManager.instance.Continue(life);
    //}    
}
