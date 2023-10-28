using Assets.Scripts.Hellper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Common;
using System;
using Assets.Scripts.Helper;
using DG.Tweening;

public class PanelQuest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI gemTxt;
    [SerializeField] private RectTransform gemSourceRectTransform;

    [SerializeField] private TextMeshProUGUI gemCompleteAllQuestTxt;
    [SerializeField] private Slider processAllQuestSlider;
    [SerializeField] private TextMeshProUGUI processAllQuestTxt;
    [SerializeField] private Button actionAllQuestBtn;
    [SerializeField] private Sprite canNotRevSp;
    [SerializeField] private Sprite CanRevSp;

    [SerializeField] private RectTransform questContainer;
    [SerializeField] private GameObject questItemViewPf;

    [SerializeField] private RectTransform tfCoinTarget;
    [SerializeField] private RectTransform tfGemTarget;

    [SerializeField] private GameObject panelHide;
    [SerializeField] private GameObject pfGemEffect;
    [SerializeField] private GameObject pfCoinEffect;

    [SerializeField] private TextMeshProUGUI hourTxt;
    [SerializeField] private TextMeshProUGUI minTxt;

    //[SerializeField] private TextMeshProUGUI refreshAmountTxt;
    [SerializeField] private Button refreshBtn;
    private Coroutine runTimeCo;

    private void Awake()
    {
        this.RegisterListener(EventID.CompleteQuest, (dt) => ReceivedRequest(dt));
        this.RegisterListener(EventID.UpdateQuest, (dt) => UpdateAllQuest(false));
        this.RegisterListener(EventID.UpdateCoin, (dt) => CoinChange());
        this.RegisterListener(EventID.UpdateGem, (dt) => GemChange());

        actionAllQuestBtn.onClick.RemoveAllListeners();
        actionAllQuestBtn.onClick.AddListener(ClaimWhenAllQuestCompleted);
    }

    private void ClaimWhenAllQuestCompleted()
    {
        AudioManager.instance.PlaySound(SoundName.collect_diamond);

        ES3.Save(StringDefine.RevRewardAllCompletedQuestDate, DateTime.Now.Date);
        ES3.Save(StringDefine.TotalReceivedQuest, 0);

        MoneyHelper.AddMoney(Config.GemRewardCompletedQuest, true);

        ShowHideAllQuestBtn();

        //MoneyHelper.AddMoney(Config.GemRewardCompletedQuest, isGem: true);
        //actionAllQuestBtn.image.sprite = canNotRevSp;
        //actionAllQuestBtn.enabled = false;
        //actionAllQuestBtn.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
    }

    private void GemChange()
    {
        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);
    }

    private void CoinChange()
    {
        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
    }


    public void Show()
    {
        gameObject.SetActive(true);
        UpdateAllQuest(false);
        if (runTimeCo != null)
            StopCoroutine(runTimeCo);
        runTimeCo = StartCoroutine(RunTime());
    }

    IEnumerator RunTime()
    {
        var sec = (int)(DateTime.Now.Date.AddDays(1) - DateTime.Now).TotalSeconds;

        while (sec > 0)
        {
            int s = GeneralHelper.FormatTime(sec, out string h, out string m);
            hourTxt.text = h;
            minTxt.text = m;

            if (s > 0)
            {
                yield return new WaitForSeconds(s);
                sec -= s;
            }
            else
            {
                yield return new WaitForSeconds(60);
                sec -= 60;
            }
        }
        yield return new WaitForSeconds(1f);
        UpdateAllQuest(false);
    }

    private void UpdateAllQuest(bool force)
    {
        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);

        while (questContainer.childCount > 0)
        {
            ObjectPool.instance.Disable(questContainer.GetChild(0).gameObject);
        }

        var quests = QuestHelper.GetQuests(force);
        for (int k = 0; k < quests.Count; k++)
        {
            var questModel = quests[k];

            var go = ObjectPool.instance.GetGameObject(questItemViewPf, Vector3.zero, Quaternion.identity);
            var itemView = go.GetComponent<QuestItemView>();
            itemView.Setup(questModel);
            go.transform.SetParent(questContainer);
            go.transform.localScale = new Vector3(1, 1, 1);
        }
      
        int am = ES3.Load(StringDefine.RefreshQuestAmount, 0);
        //refreshAmountTxt.text = string.Format("{0}/1", am);
        if (am >= 1)
        {
            //refreshBtn.enabled = false;
            //refreshBtn.image.sprite = canNotRevSp;
            refreshBtn.gameObject.SetActive(false);
        }

        ShowHideAllQuestBtn();
    }

    private void ShowHideAllQuestBtn()
    {
        int max = 10;
        int questCompletedAmount = ES3.Load(StringDefine.TotalReceivedQuest, 0);
        gemCompleteAllQuestTxt.text = Config.GemRewardCompletedQuest.ToString();
        processAllQuestSlider.maxValue = max;
        processAllQuestSlider.value = questCompletedAmount;
        processAllQuestTxt.text = string.Format("{0}/{1}", questCompletedAmount, max);
        var date = ES3.Load(StringDefine.RevRewardAllCompletedQuestDate, DateTime.Now.Date.AddDays(-1));

        //if (date < DateTime.Now.Date)
        //{
            if (questCompletedAmount >= max)
            {
                //can rev
                processAllQuestSlider.gameObject.SetActive(false);
                actionAllQuestBtn.gameObject.SetActive(true);
                var tm = actionAllQuestBtn.GetComponentInChildren<TextMeshProUGUI>();
                tm.alpha = 1f;
                tm.text = "Claim";
            }
            else
            {
                processAllQuestSlider.gameObject.SetActive(true);
                actionAllQuestBtn.gameObject.SetActive(false);
            }
        //}
        //else
        //{
        //    processAllQuestSlider.gameObject.SetActive(false);
        //    actionAllQuestBtn.gameObject.SetActive(true);

        //    actionAllQuestBtn.interactable = false;
        //    var tm = actionAllQuestBtn.GetComponentInChildren<TextMeshProUGUI>();
        //    tm.alpha = 128f / 255;
        //    tm.text = "Claimed";
        //}
    }    

    private void ReceivedRequest(object dta)
    {
        var dt = (ValueTuple<int, Vector3, bool>)dta;
        if(dt.Item2 == Vector3.zero)
        {
            EndEffect();
            return;
        }
      
        ES3.Save(StringDefine.TotalReceivedQuest, ES3.Load(StringDefine.TotalReceivedQuest, 0) + 1);
        ShowHideAllQuestBtn();

        AudioManager.instance.PlaySound(SoundName.collect_coin);

        panelHide.gameObject.SetActive(true);
        int countRemovedGo = 0;
        int coin = dt.Item1;
        var n = Mathf.Ceil(dt.Item1 * 1f / 50);
        //if (coin >= 100) n = coin / 50;
        Debug.Log("n:" + n);

        var distance = 0.75f;
        for (int k = 0; k < n; k++)
        {
            Vector3 pos = dt.Item2 + new Vector3(UnityEngine.Random.Range(-distance, distance), UnityEngine.Random.Range(-distance, distance));
            var go = ObjectPool.instance.GetGameObject((dt.Item3 ? pfGemEffect : pfCoinEffect), dt.Item2, Quaternion.identity);
            //AudioManager.instance.PlayCollectCoin();

            go.transform.DOMove(pos, 0.5f).OnUpdate(() =>
            {
            }).OnComplete(() =>
            {
                go.transform.DOMove((dt.Item3 ? tfGemTarget.position : tfCoinTarget.position), 0.5f).OnUpdate(() =>
                 {
                 }).OnComplete(() =>
                 {
                     ObjectPool.instance.Disable(go);
                     countRemovedGo++;
                     if (countRemovedGo == n)
                         EndEffect();
                 });
            });
        }

        void EndEffect()
        {
            MoneyHelper.AddMoney(dt.Item1, dt.Item3);
            panelHide.gameObject.SetActive(false);
            //UpdateAllQuest();
        }
    }

    public void Refresh()
    {
        int am = ES3.Load(StringDefine.RefreshQuestAmount, 0);
        if (am == 0)
        {
            refreshBtn.gameObject.SetActive(true);
            void actionPrepare()
            {
                MenuUIManager.instance.panelLoadingAd.StartLoading();
            }
            void actionFailed()
            {
                MenuUIManager.instance.panelLoadingAd.StopLoading();
            }
            void actionSuccess()
            {
                UpdateAllQuest(true);
            }
            IronSourceAd.instance.ShowRewardedVideo("RefreshQuest", actionPrepare, actionSuccess, actionFailed);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            Debug.Log("OnApplicationPause");
            if (runTimeCo != null)
                StopCoroutine(runTimeCo);
            runTimeCo = StartCoroutine(RunTime());
        }
    }

    public void OnClickCloseBtn()
    {
        gameObject.SetActive(false);
        //IronSourceAd.instance.ShowBanner();
    }
}
