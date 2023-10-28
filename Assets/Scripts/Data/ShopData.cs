using Assets.Scripts.Define;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Assets.Scripts.Data
{
    public class ShopData
    {
        public enum PackageType
        {
            None,
            PileDiamondSale,
            PackDiamondSale,
            ChestDiamondSale,
            PileDiamond,
            PackDiamond,
            ChestDiamond,
            RemoveAd,
            WeeklyPack,
        }

        public static Dictionary<PackageType, (string packageId, float price, int gem, ProductType productType)> data = 
            new Dictionary<PackageType, (string packageId,  float price, int gem, ProductType productType)>
        {
            {PackageType.PileDiamondSale, ("buy.pile.sale",  1f, 40, ProductType.Consumable)},
            {PackageType.PackDiamondSale, ("buy.pack.sale", 5f, 240, ProductType.Consumable)},
            {PackageType.ChestDiamondSale, ("buy.chest.sale",  10f, 500, ProductType.Consumable)},

            {PackageType.PileDiamond, ("buy.pile",  1f, 20, ProductType.Consumable)},
            {PackageType.PackDiamond, ("buy.pack",  5f, 120, ProductType.Consumable)},
            {PackageType.ChestDiamond, ("buy.chest", 10f, 250, ProductType.Consumable)},

            {PackageType.RemoveAd, ("buy.removead", 1f, 0, ProductType.NonConsumable)},
            {PackageType.WeeklyPack, ("buy.weeklypack", 12f, 0, ProductType.Consumable)},
        };
    }
}