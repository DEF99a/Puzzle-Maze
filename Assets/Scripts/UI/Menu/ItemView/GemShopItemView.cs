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

public enum GemShopPackage
{
    Pile,
    Pack,
    Chest
}

public class GemShopItemView : MonoBehaviour
{
    [SerializeField] private GemShopPackage entityType;
    //[SerializeField] private bool isSoldOut;

    [SerializeField] private int amount;
    [SerializeField] private int price;

    //[SerializeField] private TextMeshProUGUI nameTxt;
    [SerializeField] private TextMeshProUGUI amountTxt;
    [SerializeField] private TextMeshProUGUI amountNotSaleOffTxt;
    [SerializeField] private TextMeshProUGUI priceTxt;
    [SerializeField] private Button mainBtn;
    [SerializeField] private RectTransform footRectTransform;


    PackageType packageType;

    private void Awake()
    {
        this.RegisterListener(EventID.BuyGemFirstTime, (dt) =>
        {
            UpdatePrice();
        });
    }
    private void OnEnable()
    {
        //nameTxt.text = entityType.ToString().Replace("Item","");
        amountTxt.text = string.Format("{0}",(amount * 2).ToString());
        amountNotSaleOffTxt.text = string.Format("{0}", amount.ToString());
        amountNotSaleOffTxt.gameObject.SetActive(true);
        footRectTransform.gameObject.SetActive(true);

        priceTxt.text = "$" + price;

        UpdatePrice();

        mainBtn.onClick.RemoveAllListeners();
        mainBtn.onClick.AddListener(Main);
    }

    private void UpdatePrice()
    {
        if (entityType == GemShopPackage.Pile)
        {
            if (ES3.Load(StringDefine.PurchasePileFirstTime, false))
            {
                amountTxt.text = amountNotSaleOffTxt.text;
                amountNotSaleOffTxt.gameObject.SetActive(false);
                footRectTransform.gameObject.SetActive(false);

                packageType = PackageType.PileDiamond;
            }
            else
            {
                packageType = PackageType.PileDiamondSale;
            }
        }
        else if (entityType == GemShopPackage.Pack)
        {
            if (ES3.Load(StringDefine.PurchasePackFirstTime, false))
            {
                amountTxt.text = amountNotSaleOffTxt.text;
                amountNotSaleOffTxt.gameObject.SetActive(false);
                footRectTransform.gameObject.SetActive(false);

                packageType = PackageType.PackDiamond;
            }
            else
            {
                packageType = PackageType.PackDiamondSale;
            }
        }
        else if (entityType == GemShopPackage.Chest)
        {
            if (ES3.Load(StringDefine.PurchaseChestFirstTime, false))
            {
                amountTxt.text = amountNotSaleOffTxt.text;
                amountNotSaleOffTxt.gameObject.SetActive(false);
                footRectTransform.gameObject.SetActive(false);

                packageType = PackageType.ChestDiamond;
            }
            else
            {
                packageType = PackageType.ChestDiamondSale;
            }
        }
    }    

    public void Main()
    {
        Purchaser.Instance.Buy(packageType);
    }
}
