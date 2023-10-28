using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using Assets.Scripts.Common;
using System;
using System.Linq;

public enum ItemShopPackage
{
    ItemAngry,
    ItemInvisible,
    ItemFreeze,
    ItemPromotion
}

public class ItemShopItemView : MonoBehaviour
{
    static Dictionary<ItemShopPackage, EntityType> packagePair = new Dictionary<ItemShopPackage, EntityType>()
    {
        {ItemShopPackage.ItemAngry, EntityType.ItemAngry}, {ItemShopPackage.ItemInvisible, EntityType.ItemInvisible},
        {ItemShopPackage.ItemFreeze, EntityType.ItemFreeze}
    };

    [SerializeField] private ItemShopPackage itemShopPackage;

    [SerializeField] private int amount;
    [SerializeField] private int price;

    [SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI amountTxt;
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button mainBtn;
    [SerializeField] private Button viewAdBtn;
    [SerializeField] private Image soldOutImage;

    private EntityType entityType;

    private void OnEnable()
    {
        if (itemShopPackage == ItemShopPackage.ItemPromotion)
        {
            var bought = ES3.Load(StringDefine.BuyItemPromotionShopDateTime, DateTime.Now.AddDays(-1).Date);
            if (bought < DateTime.Now.Date)
            {
                //show
                soldOutImage.gameObject.SetActive(false);
                entityType = packagePair[packagePair.Keys.ElementAt(UnityEngine.Random.Range(0, packagePair.Keys.Count()))];
            }
            else
            {
                SoldOut();
                return;
            }
        }
        else entityType = packagePair[itemShopPackage];

        nameTxt.text = entityType.ToString().Replace("Item","");
        amountTxt.text = string.Format("x{0}",amount.ToString());
        icon.sprite = EntitySprites.instance.GetSprite(entityType);
        priceTxt.text = price.ToString();

        mainBtn.enabled = true;
        viewAdBtn.gameObject.SetActive(true);

        mainBtn.onClick.RemoveAllListeners();
        viewAdBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(Main);
        viewAdBtn.onClick.AddListener(ViewAd);
    }

    public void Main()
    {
        if(MoneyHelper.GetMoney(isGem:false) > price)
        {
            AudioManager.instance.PlaySound(SoundName.click_btn_common);

            Save();

            MoneyHelper.AddMoney(-price, isGem: false);
        }
        else AudioManager.instance.PlaySound(SoundName.click_fail);
    }

    private void Save()
    {
        if (entityType == EntityType.ItemAngry)
            ES3.Save(StringDefine.PlayerAngryItemAmount, ES3.Load(StringDefine.PlayerAngryItemAmount, 0) + amount);
        if (entityType == EntityType.ItemInvisible)
            ES3.Save(StringDefine.PlayerInvisibleItemAmount, ES3.Load(StringDefine.PlayerInvisibleItemAmount, 0) + amount);
        if (entityType == EntityType.ItemFreeze)
            ES3.Save(StringDefine.PlayerFreezeItemAmount, ES3.Load(StringDefine.PlayerFreezeItemAmount, 0) + amount);

        if (itemShopPackage == ItemShopPackage.ItemPromotion)
        {
            ES3.Save(StringDefine.BuyItemPromotionShopDateTime, DateTime.Now.Date);
            SoldOut();
        }
    }

    void SoldOut()
    {
        mainBtn.enabled = false;
        viewAdBtn.gameObject.SetActive(false);
        soldOutImage.gameObject.SetActive(true);
    }   
    
    public void ViewAd()
    {
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
            Save();
        }
        IronSourceAd.instance.ShowRewardedVideo("ShopItem_" + itemShopPackage, actionPrepare, actionSuccess, actionFailed);
    }    
}
