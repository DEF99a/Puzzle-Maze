using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.Helper;
using Assets.Scripts.Hellper;
using Assets.Scripts.Common;
using System.Linq;

public class PanelWin2 : MonoBehaviour
{
    public static PanelWin2 instance;

    [SerializeField] private RectTransform container;

    [SerializeField] private RectTransform scoreRect;
    //[SerializeField] private RectTransform scoreRectPos2;
    [SerializeField] private Button watchButton;
    [SerializeField] private RectTransform funcBtn;
    //[SerializeField] private RectTransform scoreLinePartial;
    //[SerializeField] private RectTransform scoreLineTotal;
    [SerializeField] private TextMeshProUGUI scoreTotalTxt;
    [SerializeField] private List<ScoreLineItemView> scoreLineItemViews;
    [SerializeField] private TextMeshProUGUI scoreViewAdTxt;
    [SerializeField] private PanelLoadingAd panelLoadingAd;
    [SerializeField] private RectTransform starsRect;
    [SerializeField] private TextMeshProUGUI scoreTotalTxtEnd;
    [SerializeField] private TextMeshProUGUI levelAmountTxt;
    [SerializeField] private RectTransform panelRate;

    //private Vector3 scoreFirstPos;
    int curIndex = 0;
    private int coinTotal;
    private int maxLevel;

    private int curLevelNumber;
    private int starAmount;

    private void Awake()
    {
        instance = this;
        //scoreFirstPos = scoreRect.localPosition;
        //this.RegisterListener(EventID.EndScoreLine, EndScoreLine);
    }

    private void EndScoreLine(object obj)
    {
        curIndex++;
        int coin;
        int amount;
        if (curIndex == 1)
        {
            coin = GameManager.instance.coinFromPairs[CoinFrom.BonusTime].Item2;
            amount = GameManager.instance.coinFromPairs[CoinFrom.BonusTime].Item1;
        }
        else if (curIndex == 2)
        {
            coin = GameManager.instance.coinFromPairs[CoinFrom.EatBalloon].Item2;
            amount = GameManager.instance.coinFromPairs[CoinFrom.EatBalloon].Item1;
        }
        else if (curIndex == 3)
        {
            coin = GameManager.instance.coinFromPairs[CoinFrom.RemoveBox].Item2;
            amount = GameManager.instance.coinFromPairs[CoinFrom.RemoveBox].Item1;
        }
        else
        {
            //scoreLinePartial.gameObject.SetActive(true);
            //scoreLineTotal.gameObject.SetActive(true);
            StartCoroutine(RunScoreTotal());
            return;
        }
        coinTotal += coin;
        scoreTotalTxt.text = coinTotal.ToString();

        scoreLineItemViews[curIndex].Setup(amount, coin, GameManager.instance.GameX2, curIndex == 1);
    }

    private void OnEnable()
    {
        GameManager.instance.Stop();

        AudioManager.instance.StopMusic(MapManager.instance.GameMusicStr);
        AudioManager.instance.PlaySound(SoundName.level_complete);
        AudioManager.instance.PlaySound(SoundName.collect_coin, 1);
        levelAmountTxt.text = MapManager.instance.CurLevelNumber.ToString();

        maxLevel = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
        if (Config.SelectMap == 0 || maxLevel == Config.SelectMap)
        {
            curLevelNumber = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
            int nextLevel = curLevelNumber + 1;
            //if (nextLevel > MapManager.instance.levelPfs.Count - 1)
            //    nextLevel -= 1;
            ES3.Save(StringDefine.CurrentLevel, nextLevel);
        }
        else curLevelNumber = Config.SelectMap;

        Debug.Log("currrrrrrr:" + ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel));

        QuestHelper.Action(Assets.Scripts.Model.QuestType.PassLevel, 1);

        FireBaseManager.instance.LogGameWin(MapManager.instance.CurLevelNumber);

        var total = 0;
        foreach (var c in GameManager.instance.coinFromPairs)
        {
            total += c.Value.Item2;
        }
        coinTotal = total;

        MoneyHelper.AddMoney(total, isGem: false);

        //scoreRect.localPosition = scoreFirstPos;
        scoreRect.gameObject.SetActive(true);
        watchButton.gameObject.SetActive(false);
        funcBtn.gameObject.SetActive(false);
        scoreTotalTxtEnd.transform.parent.gameObject.SetActive(false);
        panelRate.gameObject.SetActive(false);
        scoreTotalTxt.text = "0";

        //StartCoroutine(Run());
        //scoreLinePartial.gameObject.SetActive(false);
        //scoreLineTotal.gameObject.SetActive(false);

        for (int k = 0; k < scoreLineItemViews.Count; k++)
            scoreLineItemViews[k].gameObject.SetActive(false);       

        starAmount = 0;
        if (GameManager.instance.GameTime > 0)
            starAmount++;
        if (GameManager.instance.GameTime >= GameManager.instance.GameTimeToGetStar)
            starAmount++;
        if (MapManager.instance.playerLife >= MapManager.instance.maxPlayerLife)
            starAmount++;

        var levelEndGameInfos = ES3.Load(StringDefine.LevelEndGameInfos, new List<LevelEndGameInfo>());
        var le = levelEndGameInfos.FirstOrDefault(l => l.levelAmount == MapManager.instance.CurLevelNumber);
        if (le == null)
        {
            levelEndGameInfos.Add(new LevelEndGameInfo() { levelAmount = MapManager.instance.CurLevelNumber, starAmount = starAmount });
            ES3.Save(StringDefine.LevelEndGameInfos, levelEndGameInfos);
        }
        else
        {
            if (le.starAmount < starAmount)
            {
                le.starAmount = starAmount;
                ES3.Save(StringDefine.LevelEndGameInfos, levelEndGameInfos);
            }
        }

        if (MapManager.instance.CurLevelNumber >= 5 && !ES3.Load(StringDefine.ShowRate, false))
        {
            container.gameObject.SetActive(false);
            panelRate.gameObject.SetActive(true);
            ES3.Save(StringDefine.ShowRate, true);
        }
        else
        {
            StartCoroutine(RunStar(starAmount));
            IronSourceAd.instance.ShowBanner();
        }
    }

    IEnumerator RunStar(int starAmount)
    {
        for (int k = 0; k < starsRect.childCount; k++)
        {
            var star = starsRect.GetChild(k);
            star.GetChild(0).gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(0.2f);
        for (int k = 0; k < starsRect.childCount; k++)
        {
            var star = starsRect.GetChild(k);
            if (k < starAmount)
            {
                AudioManager.instance.PlaySound(SoundName.count_star);
                star.GetChild(0).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.2f);
            }
        }
        StartCoroutine(RunScoreTotal());

        //yield return new WaitForSeconds(0.2f);
        //scoreLineItemViews[curIndex].Setup(GameManager.instance.coinFromPairs[CoinFrom.KillEnemy].Item1,
        //    GameManager.instance.coinFromPairs[CoinFrom.KillEnemy].Item2, GameManager.instance.GameX2, false);
        //coinTotal += GameManager.instance.coinFromPairs[CoinFrom.KillEnemy].Item2;
    }

    private IEnumerator RunScoreTotal()
    {
        //var scoreStep = coinTotal / 5;
        //var scoreTmp = 0;
        //scoreTotalTxt.text = scoreTmp.ToString();
        //for (int k = 0; k < 5; k++)
        //{
        yield return null;

        //    scoreTmp += scoreStep;
        //    scoreTotalTxt.text = scoreTmp.ToString();
        //}
        scoreTotalTxt.text = coinTotal.ToString();
        scoreViewAdTxt.text = coinTotal.ToString();
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        //yield return new WaitForSeconds(2f);

        //scoreRect.DOLocalMove(scoreRectPos2.localPosition, 0.25f).OnComplete(() =>
        //{
        //    watchButton.gameObject.SetActive(true);
        //    StartCoroutine(ShowFunc());
        //});
        scoreRect.gameObject.SetActive(false);
        scoreTotalTxtEnd.transform.parent.gameObject.SetActive(true);
        scoreTotalTxtEnd.text = coinTotal.ToString();
        yield return new WaitForSeconds(0.5f);

        watchButton.gameObject.SetActive(true);

        //StartCoroutine(ShowFunc());
        yield return new WaitForSeconds(1f);
        funcBtn.gameObject.SetActive(true);

        //IEnumerator ShowFunc()
        //{
        //    yield return new WaitForSeconds(1f);
        //    funcBtn.gameObject.SetActive(true);
        //}
    }

    public void Quit()
    {
        MapManager.instance.ResetMovePoints();
        ObjectPool.instance.DisableAll();
        if (MapManager.instance.CurLevelNumber <= Config.LevelNotShowInter)
        {
            actionSuccess();
        }
        else
        {
            IronSourceAd.instance.ShowInterstitialAd("GameWinQuit", curLevelNumber, actionSuccess, actionSuccess);
        }
        void actionSuccess()
        {
            //if (MapManager.instance.CurLevelNumber >= 15 && !ES3.Load(StringDefine.ShowRate, false))
            //{
            //    panelRate.gameObject.SetActive(true);
            //    ES3.Save(StringDefine.ShowRate, true);
            //}
            //else
            //{
                IronSourceAd.instance.HideBanner();
                SceneManager.LoadScene("Menu");
            //}
        }
    }

    public void Next()
    {
        IronSourceAd.instance.HideBanner();
        MapManager.instance.ResetMovePoints();
        ObjectPool.instance.DisableAll();

        if (MapManager.instance.CurLevelNumber <= Config.LevelNotShowInter)
        {
            actionSuccess();
        }
        else
        {
            IronSourceAd.instance.ShowInterstitialAd("GameWinNext", curLevelNumber, actionSuccess, actionSuccess);
        }

        void actionSuccess()
        {
            Config.SelectMap = curLevelNumber + 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void Replay()
    {
        //if (Config.SelectMap == 0 || maxLevel == Config.SelectMap)
        //{
        //  var nextLevel = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
        //if (nextLevel == MapManager.instance.levelPfs.Count - 1)
        //    nextLevel = MapManager.instance.levelPfs.Count - 1;
        //else 
        //    nextLevel -= 1;
        //    ES3.Save(StringDefine.CurrentLevel, nextLevel);
        //}
        IronSourceAd.instance.HideBanner();
        Config.SelectMap = curLevelNumber;

        MapManager.instance.ResetMovePoints();
        ObjectPool.instance.DisableAll();
        if (MapManager.instance.CurLevelNumber <= Config.LevelNotShowInter)
        {
            actionSuccess();
        }
        else
        {
            IronSourceAd.instance.ShowInterstitialAd("GameWinReplay", curLevelNumber, actionSuccess, actionSuccess);
        }

        void actionSuccess()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void ViewAd()
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
            watchButton.interactable = false;
            MoneyHelper.AddMoney(coinTotal, isGem: false);
            scoreTotalTxtEnd.text = (coinTotal * 2).ToString();
        }
        IronSourceAd.instance.ShowRewardedVideo("GameWinX2", actionPrepare, actionSuccess, actionFailed);
    }

    public void Rate4Star()
    {
        //IronSourceAd.instance.HideBanner();

        panelRate.gameObject.SetActive(false);
        container.gameObject.SetActive(true);

        //gameObject.SetActive(false);
        FireBaseManager.instance.LogRate("Rate1-4");
        //SceneManager.LoadScene("Menu");

        StartCoroutine(RunStar(starAmount));
        IronSourceAd.instance.ShowBanner();
    }

    public void Rate5Star()
    {
        //IronSourceAd.instance.HideBanner();

        panelRate.gameObject.SetActive(false);
        //gameObject.SetActive(false);
        container.gameObject.SetActive(true);
        FireBaseManager.instance.LogRate("Rate5");

        StartCoroutine(RunStar(starAmount));
        IronSourceAd.instance.ShowBanner();

        //StartCoroutine(LoadYourAsyncScene());

        //static IEnumerator LoadYourAsyncScene()
        //{
        //    AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Menu");

        //    while (!asyncLoad.isDone)
        //    {
        //        yield return null;
        //    }

        //    EasyMobile.StoreReview.RequestRating();
        //    FireBaseManager.instance.LogRate("Rate5");
        //}
    }
}
