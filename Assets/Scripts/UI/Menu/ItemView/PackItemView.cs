using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using Assets.Scripts.Common;
using System;
using static Assets.Scripts.Data.ShopData;
using Spine.Unity;
using Assets.Scripts.Hellper;

public enum ShopPack
{
    RemoveAd,
    WeeklyPack
}

public class PackItemView : MonoBehaviour
{
    [SerializeField] private ShopPack entityType;

    [SerializeField] private int price;
      
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button mainBtn;
    [SerializeField] private Image soldOutImage;
    [SerializeField] private SkeletonGraphic skeletonGraphic;

    PackageType packageType;

    private void Awake()
    {
        this.RegisterListener(EventID.RemoveAd, (dt) =>
        {
            if (entityType == ShopPack.RemoveAd)
                gameObject.SetActive(false);
        });
        this.RegisterListener(EventID.WeeklyPack, (dt) =>
        {
            if (entityType == ShopPack.WeeklyPack)
            {
                soldOutImage.gameObject.SetActive(true);
                mainBtn.enabled = false;

                this.PostEvent(EventID.RemoveAd, null);

                ES3.Save(StringDefine.PurchaseWeeklyPack, DateTime.Now.Date.AddDays(6));

                PurchaseHelper.RewardDailyVip();
            }
        });
    }

    private void OnEnable()
    {
        if (entityType == ShopPack.RemoveAd)
            packageType = PackageType.RemoveAd;
        else if (entityType == ShopPack.WeeklyPack)
            packageType = PackageType.WeeklyPack;

        mainBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(Main);

        if (entityType == ShopPack.WeeklyPack)
        {
            var date = ES3.Load(StringDefine.PurchaseWeeklyPack, DateTime.Now.Date.AddDays(-7));
            if(date < DateTime.Now.Date)
            {
                soldOutImage.gameObject.SetActive(false);
                mainBtn.enabled = true;
            }
            else
            {
                soldOutImage.gameObject.SetActive(true);
                mainBtn.enabled = false;
            }
        }
        if(skeletonGraphic != null && entityType == ShopPack.WeeklyPack)
        {
            PlayerHelper.LoadSkeleton(Config.PlayerTypeFromWeeklyPackage, skeletonGraphic, loop: true);
        }    
    }

    public void Main()
    {
    }
}
