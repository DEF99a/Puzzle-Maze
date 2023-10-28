using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using Assets.Scripts.Common;
using System;

public enum GoldShopPackage
{
    Few,
    Stack,
    Pack,
    Chest
}

public class GoldShopItemView : MonoBehaviour
{
    [SerializeField] private GoldShopPackage entityType;
    //[SerializeField] private bool isSoldOut;

    [SerializeField] private int amount;
    [SerializeField] private int price;

    //[SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI amountTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private TextMeshProUGUI priceSaleOff;

    [SerializeField] private Button mainBtn;
    [SerializeField] private Button viewAdBtn;

    [SerializeField] private Image soldOutImg;
    [SerializeField] private RectTransform footRectTransform;
    [SerializeField] private RectTransform saleOnRect;
    [SerializeField] private RectTransform saleOffRect;

    private int mPrice;

    private void OnEnable()
    {
        mPrice = price;

        //nameTxt.text = entityType.ToString().Replace("Item","");
        amountTxt.text = string.Format("{0}",amount.ToString());
        //icon.sprite = EntitySprites.instance.GetSprite(entityType);
        if (price > 0)
            priceTxt.text = price.ToString();
        else priceTxt.text = "Free";

        mainBtn.enabled = true;

        mainBtn.onClick.RemoveAllListeners();
        viewAdBtn?.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(Main);
        viewAdBtn?.onClick.AddListener(ViewAd);

        if (entityType == GoldShopPackage.Few)
        {
            var date = ES3.Load(StringDefine.BuyFewPackageDateTime, DateTime.Now.AddDays(-1));
            if (date <= DateTime.Now)
            {
                // can buy
                soldOutImg.gameObject.SetActive(false);
            }
            else
            {
                // sold out
                FewGemSoldOut();
            }
        }
        else if(entityType == GoldShopPackage.Stack)
        {
            var date = ES3.Load(StringDefine.BuyStackPackageDateTime, DateTime.Now);
            Debug.Log("ddddd 2:" + date);

            if (date <= DateTime.Now)
            {
                // can buy
                priceTxt.text = string.Format("({0})", mPrice);
                priceTxt.fontStyle = FontStyles.Strikethrough;
                mPrice /= 2;

                priceSaleOff.text = mPrice.ToString();
                priceSaleOff.gameObject.SetActive(true);
                footRectTransform.gameObject.SetActive(true);
                viewAdBtn.gameObject.SetActive(true);

                saleOnRect.gameObject.SetActive(true);
                saleOffRect.gameObject.SetActive(false);
            }
            else
            {
                // sold out
                BoughtStackFree();
            }
        }    
    }

    private void BoughtStackFree()
    {
        mPrice = price;
        viewAdBtn.gameObject.SetActive(false);

        saleOffRect.GetChild(1).GetComponent<TextMeshProUGUI>().text = mPrice.ToString();
        footRectTransform.gameObject.SetActive(false);

        saleOffRect.gameObject.SetActive(true);
        saleOnRect.gameObject.SetActive(false);
    }    

    private void FewGemSoldOut()
    {
        soldOutImg.gameObject.SetActive(true);
        mainBtn.enabled = false;
    }    

    public void Main()
    {
        if(entityType == GoldShopPackage.Few)
        {
            var date = ES3.Load(StringDefine.BuyFewPackageDateTime, DateTime.Now.AddDays(-1));
            Debug.Log("hahaha date:"  + date + " com:" + (date <= DateTime.Now));

            if (date <= DateTime.Now)
            {
                AudioManager.instance.PlaySound(SoundName.click_btn_common);

                ES3.Save(StringDefine.BuyFewPackageDateTime, DateTime.Now.AddHours(8));
                MoneyHelper.AddMoney(amount, isGem: false);
                FewGemSoldOut();

                PanelShop.instance.UpdateShopNotice();
                Debug.Log("hahaha");
            }         
            else AudioManager.instance.PlaySound(SoundName.click_fail);
        }
        else if (entityType == GoldShopPackage.Stack)
        {
            var date = ES3.Load(StringDefine.BuyStackPackageDateTime, DateTime.Now);
            Debug.Log("ddddd:" + date);
            if (date <= DateTime.Now)
            {
                ES3.Save(StringDefine.BuyStackPackageDateTime, DateTime.Now.AddMinutes(30));                
            }

            if (MoneyHelper.GetMoney(isGem: true) >= mPrice)
            {
                AudioManager.instance.PlaySound(SoundName.click_btn_common);
                MoneyHelper.AddMoney(amount, isGem: false);
                MoneyHelper.AddMoney(-mPrice, isGem: true);
                BoughtStackFree();
            }              
            else AudioManager.instance.PlaySound(SoundName.click_fail);
        }
        else if(MoneyHelper.GetMoney(isGem:true) >= price)
        {
            AudioManager.instance.PlaySound(SoundName.click_btn_common);
            MoneyHelper.AddMoney(amount, isGem: false);
            MoneyHelper.AddMoney(-price, isGem: true);
        }    
        else AudioManager.instance.PlaySound(SoundName.click_fail);
    }

    public void ViewAd()
    {
        void actionPrepare()
        {
          MenuUIManager.instance. panelLoadingAd.StartLoading();
        }
        void actionFailed()
        {
            MenuUIManager.instance.panelLoadingAd.StopLoading();
        }
        void actionSuccess()
        {            
            var date = ES3.Load(StringDefine.BuyStackPackageDateTime, DateTime.Now);
            Debug.Log("ddddd 3:" + date);
            if (date <= DateTime.Now)
            {
                ES3.Save(StringDefine.BuyStackPackageDateTime, DateTime.Now.AddMinutes(30));
            }

            MoneyHelper.AddMoney(amount, isGem: false);
            BoughtStackFree();

            PanelShop.instance.UpdateShopNotice();
            //MoneyHelper.AddMoney(Config.CoinViewAdAtShop, isGem: false);
        }
        IronSourceAd.instance.ShowRewardedVideo("StackGold", actionPrepare, actionSuccess, actionFailed);
    }    
}
