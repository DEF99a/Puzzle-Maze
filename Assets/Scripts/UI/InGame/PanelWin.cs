//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using DG.Tweening;
//using UnityEngine.SceneManagement;
//using TMPro;
//using Assets.Scripts.Helper;
//using Assets.Scripts.Hellper;
//using Assets.Scripts.Common;
//using System.Linq;

//public class PanelWin : MonoBehaviour
//{
//    [SerializeField] private RectTransform scoreRect;
//    [SerializeField] private RectTransform scoreRectPos2;
//    [SerializeField] private Button watchButton;
//    [SerializeField] private RectTransform funcBtn;
//    [SerializeField] private RectTransform scoreLinePartial;
//    [SerializeField] private RectTransform scoreLineTotal;
//    [SerializeField] private TextMeshProUGUI scoreTotalTxt;
//    [SerializeField] private List<ScoreLineItemView> scoreLineItemViews;
//    [SerializeField] private TextMeshProUGUI scoreViewAdTxt;
//    [SerializeField] private PanelLoadingAd panelLoadingAd;
//    [SerializeField] private RectTransform starsRect;

//    private Vector3 scoreFirstPos;
//    int curIndex = 0;
//    private int coinTotal;
//    private int maxLevel;

//    private int curLevelNumber;

//    private void Awake()
//    {
//        scoreFirstPos = scoreRect.localPosition;
//        this.RegisterListener(EventID.EndScoreLine, EndScoreLine);
//    }

//    private void EndScoreLine(object obj)
//    {
//        curIndex++;
//        int coin;
//        int amount;
//        if (curIndex == 1)
//        {
//            coin = GameManager.instance.coinFromPairs[CoinFrom.BonusTime].Item2;
//            amount = GameManager.instance.coinFromPairs[CoinFrom.BonusTime].Item1;
//        }
//        else if (curIndex == 2)
//        {
//            coin = GameManager.instance.coinFromPairs[CoinFrom.EatBalloon].Item2;
//            amount = GameManager.instance.coinFromPairs[CoinFrom.EatBalloon].Item1;
//        }
//        else if (curIndex == 3)
//        {
//            coin = GameManager.instance.coinFromPairs[CoinFrom.RemoveBox].Item2;
//            amount = GameManager.instance.coinFromPairs[CoinFrom.RemoveBox].Item1;
//        }
//        else
//        {
//            scoreLinePartial.gameObject.SetActive(true);
//            scoreLineTotal.gameObject.SetActive(true);
//            StartCoroutine(RunScoreTotal());
//            return;
//        }
//        coinTotal += coin;
//        scoreLineItemViews[curIndex].Setup(amount, coin);
//    }

//    private void OnEnable()
//    {
//        AudioManager.instance.PlaySound(SoundName.Win);

//        maxLevel = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
//        if (Config.SelectMap == 0 || maxLevel == Config.SelectMap)
//        {
//            curLevelNumber = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
//            int nextLevel = curLevelNumber + 1;
//            //if (nextLevel > MapManager.instance.levelPfs.Count - 1)
//            //    nextLevel -= 1;
//            ES3.Save(StringDefine.CurrentLevel, nextLevel);
//        }
//        else curLevelNumber = Config.SelectMap;

//        Debug.Log("currrrrrrr:" + ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel));

//        QuestHelper.Action(Assets.Scripts.Model.QuestType.PassLevel, 1);

//        FireBaseManager.instance.LogGameWin(MapManager.instance.CurLevelNumber);

//        var total = 0;
//        foreach (var c in GameManager.instance.coinFromPairs)
//        {
//            total += c.Value.Item2;
//        }
//        MoneyHelper.AddMoney(total, isGem: false);

//        scoreRect.localPosition = scoreFirstPos;
//        scoreRect.gameObject.SetActive(true);
//        watchButton.gameObject.SetActive(false);
//        funcBtn.gameObject.SetActive(false);
//        //StartCoroutine(Run());
//        scoreLinePartial.gameObject.SetActive(false);
//        scoreLineTotal.gameObject.SetActive(false);

//        for (int k = 0; k < scoreLineItemViews.Count; k++)
//            scoreLineItemViews[k].gameObject.SetActive(false);
//        scoreLineItemViews[curIndex].Setup(GameManager.instance.coinFromPairs[CoinFrom.KillEnemy].Item1,
//            GameManager.instance.coinFromPairs[CoinFrom.KillEnemy].Item2);
//        coinTotal += GameManager.instance.coinFromPairs[CoinFrom.KillEnemy].Item2;

//        int starAmount = 3;
//        for(int k = 0; k < starsRect.childCount; k++)
//        {
//            var star = starsRect.GetChild(k);
//            if(k < starAmount)
//            {
//                star.GetChild(0).gameObject.SetActive(true);
//            }
//            else
//            {
//                star.GetChild(0).gameObject.SetActive(false);
//            }
//        }    

//        var levelEndGameInfos = ES3.Load(StringDefine.LevelEndGameInfos, new List<LevelEndGameInfo>());
//        var le = levelEndGameInfos.FirstOrDefault(l => l.levelAmount == MapManager.instance.CurLevelNumber);
//        if (le == null)
//        {
//            levelEndGameInfos.Add(new LevelEndGameInfo() { levelAmount = MapManager.instance.CurLevelNumber, starAmount = starAmount });
//            ES3.Save(StringDefine.LevelEndGameInfos, levelEndGameInfos);
//        }
//        else
//        {
//            le.starAmount = starAmount;
//            ES3.Save(StringDefine.LevelEndGameInfos, levelEndGameInfos);
//        }
//    }

//    private IEnumerator RunScoreTotal()
//    {
//        var scoreStep = coinTotal / 5;
//        var scoreTmp = 0;
//        scoreTotalTxt.text = scoreTmp.ToString();
//        for (int k = 0; k < 5; k++)
//        {
//            yield return new WaitForSeconds(0.05f);

//            scoreTmp += scoreStep;
//            scoreTotalTxt.text = scoreTmp.ToString();
//        }
//        scoreTotalTxt.text = coinTotal.ToString();
//        scoreViewAdTxt.text = coinTotal.ToString();
//        StartCoroutine(Run());
//    }

//    private IEnumerator Run()
//    {
//        yield return new WaitForSeconds(0.5f);
//        scoreRect.DOLocalMove(scoreRectPos2.localPosition, 0.25f).OnComplete(() =>
//        {
//            watchButton.gameObject.SetActive(true);
//            StartCoroutine(ShowFunc());
//        });

//        IEnumerator ShowFunc()
//        {
//            yield return new WaitForSeconds(3f);
//            funcBtn.gameObject.SetActive(true);
//        }
//    }

//    public void Quit()
//    {
//        MapManager.instance.ResetMovePoints();
//        ObjectPool.instance.DisableAll();
//        if (MapManager.instance.CurLevelNumber == 0)
//        {
//            actionSuccess();
//        }
//        else
//        {
//            IronSourceAd.instance.ShowInterstitialAd("GameWinQuit", curLevelNumber, actionSuccess, actionSuccess);
//        }
//        void actionSuccess()
//        {
//            SceneManager.LoadScene("Menu");
//        }
//    }

//    public void Next()
//    {
//        MapManager.instance.ResetMovePoints();
//        ObjectPool.instance.DisableAll();

//        if (MapManager.instance.CurLevelNumber == 0)
//        {
//            actionSuccess();
//        }
//        else
//        {
//            IronSourceAd.instance.ShowInterstitialAd("GameWinNext", curLevelNumber, actionSuccess, actionSuccess);
//        }

//        void actionSuccess()
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        }
//    }

//    public void Replay()
//    {
//        if (Config.SelectMap == 0 || maxLevel == Config.SelectMap)
//        {
//            var nextLevel = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
//            //if (nextLevel == MapManager.instance.levelPfs.Count - 1)
//            //    nextLevel = MapManager.instance.levelPfs.Count - 1;
//            //else 
//            nextLevel -= 1;
//            ES3.Save(StringDefine.CurrentLevel, nextLevel);
//        }

//        MapManager.instance.ResetMovePoints();
//        ObjectPool.instance.DisableAll();
//        if (MapManager.instance.CurLevelNumber == 0)
//        {
//            actionSuccess();
//        }
//        else
//        {
//            IronSourceAd.instance.ShowInterstitialAd("GameWinReplay", curLevelNumber, actionSuccess, actionSuccess);
//        }

//        void actionSuccess()
//        {
//            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
//        }
//    }

//    public void ViewAd()
//    {
//        void actionPrepare()
//        {
//            panelLoadingAd.StartLoading();
//        }
//        void actionFailed()
//        {
//            panelLoadingAd.StopLoading();
//        }
//        void actionSuccess()
//        {
//            watchButton.interactable = false;
//            MoneyHelper.AddMoney(coinTotal, isGem: false);
//        }
//        IronSourceAd.instance.ShowRewardedVideo("GameWinX2", actionPrepare, actionSuccess, actionFailed);
//    }
//}
