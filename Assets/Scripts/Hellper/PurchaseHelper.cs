using Assets.Scripts.Common;
using Assets.Scripts.Hellper;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Helper
{
    public class PurchaseHelper
    {
        public static bool HasRemoveAd()
        {
            var buy = ES3.Load(StringDefine.PurchaseRemoveAd, false);
            if (buy) return true;
            var date = ES3.Load(StringDefine.PurchaseWeeklyPack, DateTime.Now.AddDays(-1000).Date);
            Debug.Log("HasRemoveAd:" + date);
            if (date >= DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }

        public static bool InVip()
        {
            var date = ES3.Load(StringDefine.PurchaseWeeklyPack, DateTime.Now.AddDays(-1000).Date);
            if (date >= DateTime.Now.Date)
            {
                return true;
            }
            return false;
        }

        public static (bool has, int coin, int gem, int angry, int invisible, int freeze) RewardDailyVip()
        {
            if (!InVip()) return (false, 0, 0, 0, 0, 0);

            var date = ES3.Load(StringDefine.RewardWeeklyPack, DateTime.Now.AddDays(-1000).Date);
            if (date <= DateTime.Now.Date)
            {
                MoneyHelper.AddMoney(1000, isGem: false);
                MoneyHelper.AddMoney(30, isGem: true);
                ES3.Save(StringDefine.PlayerAngryItemAmount, ES3.Load(StringDefine.PlayerAngryItemAmount, 0) + 1);
                ES3.Save(StringDefine.PlayerInvisibleItemAmount, ES3.Load(StringDefine.PlayerInvisibleItemAmount, 0) + 1);
                ES3.Save(StringDefine.PlayerFreezeItemAmount, ES3.Load(StringDefine.PlayerFreezeItemAmount, 0) + 1);

                ES3.Save(StringDefine.RewardWeeklyPack, DateTime.Now.Date.AddDays(1));

                if (!PlayerHelper.HasPlayer(Config.PlayerTypeFromWeeklyPackage) && Config.playerTypeDefault != Config.PlayerTypeFromWeeklyPackage)
                {
                    PlayerHelper.AddPlayer(new PlayerSaver()
                    {
                        playerType = Config.PlayerTypeFromWeeklyPackage
                    });
                    ES3.Save(StringDefine.HasCharacterFromWeeklyPackage, true);
                }

                return (true, 1000, 30, 1, 1, 1);
            }
            else
            {
                var check = ES3.Load(StringDefine.HasCharacterFromWeeklyPackage, false);
                if (check && PlayerHelper.HasPlayer(Config.PlayerTypeFromWeeklyPackage) && Config.playerTypeDefault != Config.PlayerTypeFromWeeklyPackage)
                {
                    PlayerHelper.RemovePlayer(Config.PlayerTypeFromWeeklyPackage);
                    ES3.Save(StringDefine.HasCharacterFromWeeklyPackage, false);
                }
            }
            return (false, 0, 0, 0, 0, 0);
        }
    }
}