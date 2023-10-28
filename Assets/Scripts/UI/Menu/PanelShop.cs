using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Assets.Scripts.Helper;
using UnityEngine.UI;
using System;
using Assets.Scripts.Common;

public class PanelShop : MonoBehaviour
{
    public static PanelShop instance;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI gemTxt;
    [SerializeField] private Sprite tabOnSprite;
    [SerializeField] private Sprite tabOffSprite;
    [SerializeField] private List<Button> tabs;
    [SerializeField] private List<RectTransform> containers;

    [SerializeField] private Image coinNoticeImg;

    [SerializeField] private Sprite greenSp;
    [SerializeField] private Sprite violetSp;

    [SerializeField] private RectTransform weeklyPackAndroid;
    [SerializeField] private RectTransform weeklyPackIOS;

    private void Awake()
    {
        instance = this;

        this.RegisterListener(EventID.UpdateCoin, (dt) => CoinChange());
        this.RegisterListener(EventID.UpdateGem, (dt) => GemChange());

        tabs[0].onClick.RemoveAllListeners();
        tabs[0].onClick.AddListener(() => { Select(0); });
        tabs[1].onClick.RemoveAllListeners();
        tabs[1].onClick.AddListener(() => { Select(1); });
        tabs[2].onClick.RemoveAllListeners();
        tabs[2].onClick.AddListener(() => { Select(2); });
    }

    private void GemChange()
    {
        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);
    }

    private void CoinChange()
    {
        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
    }

    private void Start()
    {
        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);
    }

    public void Show(int index)
    {
        gameObject.SetActive(true);
        Select(index);

        UpdateShopNotice();
    }

    private void Select(int index)
    {
        //Debug.Log("index:" + index);
        for (int k = 0; k < containers.Count; k++)
            tabs[k].image.sprite = (k == index) ? tabOnSprite : tabOffSprite;

        for (int k = 0; k < containers.Count; k++)
            containers[k].gameObject.SetActive(k == index);

        if (index == 2)
        {
            weeklyPackIOS.gameObject.SetActive(Config.IsIOS);
            weeklyPackAndroid.gameObject.SetActive(!Config.IsIOS);

            if (PurchaseHelper.HasRemoveAd())
            {
                containers[2].GetChild(0).GetChild(0).GetChild(3).gameObject.SetActive(false);
            }
        }
    }

    public void OnClickCloseBtn()
    {
        gameObject.SetActive(false);
        MenuUIManager.instance.UpdateShopNotice();
        //IronSourceAd.instance.ShowBanner();
    }

    public void UpdateShopNotice()
    {
        var date = ES3.Load(StringDefine.BuyFewPackageDateTime, DateTime.Now.AddDays(-1));
        if (date <= DateTime.Now)
        {
            coinNoticeImg.sprite = greenSp;
            coinNoticeImg.gameObject.SetActive(true);
        }
        else
        {
            date = ES3.Load(StringDefine.BuyStackPackageDateTime, DateTime.Now);
            if (date <= DateTime.Now)
            {
                coinNoticeImg.sprite = violetSp;
                coinNoticeImg.gameObject.SetActive(true);
            }
            else coinNoticeImg.gameObject.SetActive(false);
        }
    }
}
