
using Assets.Scripts.Define;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Helper
{
    public class MoneyHelper
    {
#if UNITY_EDITOR
        static int DefaultGem = 0;
        static int DefaultCoin = 0;
#else
        static int DefaultGem = 0;
        static int DefaultCoin = 0;
#endif

        public static int AddMoney(int money, bool isGem)
        {
            var key = isGem ? StringDefine.Gem : StringDefine.Coin;

            int curMoney = ES3.Load(key, isGem ? DefaultGem : DefaultCoin) + money;

            Debug.Log("adddmoneyyyy");

            ES3.Save(key, curMoney);
            if (isGem)
            {
                AudioManager.instance?.PlaySound(SoundName.collect_diamond);

                EventDispatcher.Instance.PostEvent(EventID.UpdateGem, null);
            }
            else EventDispatcher.Instance.PostEvent(EventID.UpdateCoin, null);
            return curMoney;
        }

        public static int GetMoney(bool isGem)
        {
            var key = isGem ? StringDefine.Gem : StringDefine.Coin;
            return ES3.Load(key, isGem ? DefaultGem : DefaultCoin);
        }

        public static string GetMoneyString(bool isGem)
        {
            return GetMoney(isGem).ToString();
        }

        public static string GetMoneyStringFormat(int money, int minMoneyToFormat = 1000)
        {
            if (money >= 1000_000)
            {
                int n = money / 1000_0000;
                if (n > 100) return n + "M";
                else if (n > 10) return (money * 1f / 1000_000).ToString("0.0") + "M";
                return (money * 1f / 1000_000).ToString("0.00") + "M";
            }
            else if (money >= minMoneyToFormat)
            {
                int n = money / 1000;
                if (n > 100) return n + "K";
                else if (n > 10) return (money * 1f / 1000).ToString("0.0") + "K";
                return (money * 1f / 1000).ToString("0.00") + "K";
            }

            return money.ToString();
        }
    }
}