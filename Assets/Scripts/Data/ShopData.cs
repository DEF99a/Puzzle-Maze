using Assets.Scripts.Define;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
    }
}