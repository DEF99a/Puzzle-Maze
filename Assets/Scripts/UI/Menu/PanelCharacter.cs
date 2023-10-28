using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Helper;
using Assets.Scripts.Common;
using UnityEngine.SceneManagement;

[System.Serializable]
public class TabInfo
{
    public PlayerRarity playerRarity;
    public Button btn;
}

public class PanelCharacter : MonoBehaviour
{
    static Dictionary<PlayerType, (string, string)> skillInfos = new Dictionary<PlayerType, (string, string)>()
    {
       {PlayerType.Main1, ("Water bomb", "Fire a water ball that explode after 5 seconds") },
        {PlayerType.Main2, ("Kamehameha", "Fire a energy beam that destroy box or enemy") },
          {PlayerType.Mario, ("Fire ball", "Fire a fire ball") },
            {PlayerType.Mario2, ("Water bomb", "Fire a water ball that explode after 5 seconds") },
              {PlayerType.Dinosaur, ("Blade", "Use blade to break box or kill enemy") },
                {PlayerType.RedImpostor, ("Knife", "Use knife to break box or kill enemy") },
    };    

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI gemTxt;

    [SerializeField] private RectTransform characterContainer;
    [SerializeField] private GameObject characterItemViewPf;
    [SerializeField] private List<TabInfo> tabs;
    [SerializeField] private Sprite tabOn;
    [SerializeField] private Sprite tabOff;

    [SerializeField] private SkeletonGraphic skeletonGraphic;
    [SerializeField] private TextMeshProUGUI lifeText;
    [SerializeField] private TextMeshProUGUI speedTxt;
    [SerializeField] private Button pickedBtn;
    [SerializeField] private Button selectBtn;
    [SerializeField] private Button buyByCoinBtn;
    [SerializeField] private Button unlockByAdBtn;
    [SerializeField] private Button trialBtn;

    [SerializeField] private RectTransform infoSkillRect;
    [SerializeField] private Image infoSkillImg;
    [SerializeField] private TextMeshProUGUI infoSkillNameTxt;
    [SerializeField] private Text infoSkillDesTxt;

    [SerializeField] private Sprite disableSp;
    [SerializeField] private Sprite greenSp;
    [SerializeField] private Sprite blueSp;

    [SerializeField] private Image logo;
    [SerializeField] private Sprite playerSpAndroid;
    [SerializeField] private Sprite playerSpIOS;

    PlayerRarity curPlayerRarity;
    PlayerType curPlayerTypeSelect;
    PlayerData curChar;
    PlayerSaver curPlayerSaver;

    private void Awake()
    {
        this.RegisterListener(EventID.UpdateCoin, (dt) => CoinChange());
        this.RegisterListener(EventID.UpdateGem, (dt) => GemChange());
    }
    private void GemChange()
    {
        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);
    }

    private void CoinChange()
    {
        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
    }

    private void OnEnable()
    {
        for (int k = 0; k < tabs.Count; k++)
        {
            var tab = tabs[k];
            tab.btn.onClick.RemoveAllListeners();
            if (k != 0) continue;

            tab.btn.onClick.AddListener(() =>
            {
                Select(tab.playerRarity, true);
            });
        }
        Select(tabs[0].playerRarity, true);

        selectBtn.onClick.RemoveAllListeners();
        selectBtn.onClick.AddListener(PickCurChar);

        buyByCoinBtn.onClick.RemoveAllListeners();
        buyByCoinBtn.onClick.AddListener(UnLockCurChar);

        trialBtn.onClick.RemoveAllListeners();
        trialBtn.onClick.AddListener(() =>
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
                Config.SelectPlayerType = curChar.playerType;
                Config.SelectMap = 0;

                IronSourceAd.instance.HideBanner();
                SceneManager.LoadScene("InGame");
            }
            IronSourceAd.instance.ShowRewardedVideo("Trial-" + curChar.playerType, actionPrepare, actionSuccess, actionFailed);
        });

        unlockByAdBtn.onClick.RemoveAllListeners();
        unlockByAdBtn.onClick.AddListener(() =>
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
                string key = StringDefine.ViewAdUnlockCharacter + curChar.playerType;
                ES3.Save(key, ES3.Load(key, 0) + 1);
                unlockByAdBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("({0}/{1})",
                    ES3.Load(key, 0), curChar.adAmount);

                if(ES3.Load(key, 0) >= curChar.adAmount)
                {
                    AudioManager.instance.PlaySound(SoundName.player_unlock, 0.2f);

                    PlayerHelper.AddPlayer(new PlayerSaver()
                    {
                        playerType = curChar.playerType
                    });
                    Select(curPlayerRarity, false);
                }               
            }
            IronSourceAd.instance.ShowRewardedVideo("UnlockChar-" + curChar.playerType, actionPrepare, actionSuccess, actionFailed);
        });

        GemChange();
        CoinChange();

        //if (Config.IsIOS) logo.sprite = playerSpIOS;
        //else logo.sprite = playerSpAndroid;
    }

    private void UnLockCurChar()
    {
        bool canBuy = false;
        int price = 0;
        bool isGem = false;
        if (curChar.coinPrice > 0)
        {
            if (MoneyHelper.GetMoney(isGem: false) >= curChar.coinPrice)
                canBuy = true;
            price = curChar.coinPrice;
        }
        else if (curChar.gemPrice > 0)
        {
            if (MoneyHelper.GetMoney(isGem: true) >= curChar.gemPrice)
                canBuy = true;
            price = curChar.gemPrice;
            isGem = true;
        }
        if (canBuy)
        {
            AudioManager.instance.PlaySound(SoundName.click_btn_common);
            AudioManager.instance.PlaySound(SoundName.player_unlock, 0.2f);

            PlayerHelper.AddPlayer(new PlayerSaver()
            {
                playerType = curChar.playerType
            });
            Select(curPlayerRarity, false);

            MoneyHelper.AddMoney(-price, isGem);
        }
        else AudioManager.instance.PlaySound(SoundName.click_fail);
    }

    private void PickCurChar()
    {
        AudioManager.instance.PlaySound(SoundName.click_btn_common);
        AudioManager.instance.PlaySound(SoundName.player_select, 0.1f);

        PlayerHelper.UpdatePlayer(curPlayerSaver);
        Select(curPlayerRarity, false);
        this.PostEvent(EventID.PickChar, null);
    }

    private void Select(PlayerRarity playerRarity, bool first)
    {
        curPlayerRarity = playerRarity;

        while (characterContainer.childCount > 0)
        {
            var chiild = characterContainer.GetChild(0);
            ObjectPool.instance.Disable(chiild.gameObject);
        }

        var playerDatas = PlayerDatas.instance.GetPlayerDatas(playerRarity);
        var players = PlayerHelper.GetPlayers();
        var curPlayer = PlayerHelper.GetCurrentPlayer();
        if (first) curPlayerTypeSelect = curPlayer.playerType;

        for (int k = 0; k < tabs.Count; k++)
        {
            var tab = tabs[k];
            tab.btn.image.sprite = tab.playerRarity == playerRarity ? tabOn : tabOff;
        }

        PlayerData playerData1 = null;
        PlayerSaver playerSaver1 = null;
        for (int k = 0; k < playerDatas.Count(); k++)
        {
            PlayerData playerData = null;
            PlayerSaver playerSaver = null;
            if (playerDatas.Count() > k)
            {
                playerData = playerDatas.ElementAt(k);
                playerSaver = players.FirstOrDefault(p => p.playerType == playerData.playerType);
            }
            var go = ObjectPool.instance.GetGameObject(characterItemViewPf, Vector3.zero, Quaternion.identity);
            go.transform.parent = characterContainer;
            go.transform.localScale = new Vector3(1, 1, 1);
            var charItemView = go.GetComponent<CharacterItemView>();
            charItemView.Setup(playerData, playerSaver, curPlayer.playerType == playerData?.playerType);

            var btn = go.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => { GetInfoChar(playerData, playerSaver, curPlayer.playerType, playerData != null ? playerData.playerType : PlayerType.None); });

            if (playerData != null && curPlayerTypeSelect == playerData.playerType)
            {
                playerData1 = playerData;
                playerSaver1 = playerSaver;
            }
        }
        GetInfoChar(playerData1, playerSaver1, curPlayer.playerType, curPlayerTypeSelect);
        this.PostEvent(EventID.SelectCharInList, curPlayerTypeSelect);
    }

    private void GetInfoChar(PlayerData playerData, PlayerSaver playerSaver, PlayerType curPlayer, PlayerType curSelect)
    {
        if (playerData != null)
        {
            curChar = playerData;
            curPlayerTypeSelect = curSelect;
            this.PostEvent(EventID.SelectCharInList, curPlayerTypeSelect);

            //Debug.Log(curChar.playerType);

            PlayerHelper.LoadSkeleton(playerData.playerType, skeletonGraphic);
            if (playerSaver != null)
            {
                curPlayerSaver = playerSaver;

                speedTxt.text = PlayerHelper.GetPlayerSpeed(playerSaver).ToString();
                lifeText.text = PlayerHelper.GetPlayerLife(playerSaver).ToString();

                pickedBtn.gameObject.SetActive(curSelect == curPlayer);
                selectBtn.gameObject.SetActive(curSelect != curPlayer);
                unlockByAdBtn.gameObject.SetActive(false);
                buyByCoinBtn.gameObject.SetActive(false);
                trialBtn.gameObject.SetActive(false);
            }
            else
            {
                speedTxt.text = playerData.speed.ToString();
                lifeText.text = playerData.life.ToString();

                pickedBtn.gameObject.SetActive(false);
                selectBtn.gameObject.SetActive(false);

                if (playerData.coinPrice > 0)
                {
                    unlockByAdBtn.gameObject.SetActive(false);
                    buyByCoinBtn.gameObject.SetActive(true);
                    buyByCoinBtn.transform.GetChild(1).gameObject.SetActive(true);
                    buyByCoinBtn.transform.GetChild(2).gameObject.SetActive(true);
                    buyByCoinBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = MoneyHelper.GetMoneyStringFormat(playerData.coinPrice);

                    buyByCoinBtn.transform.GetChild(3).gameObject.SetActive(false);
                    buyByCoinBtn.transform.GetChild(4).gameObject.SetActive(false);

                    if (MoneyHelper.GetMoney(isGem: false) >= curChar.coinPrice)
                    {
                        buyByCoinBtn.enabled = true;
                        buyByCoinBtn.image.sprite = greenSp;
                    }
                    else
                    {
                        buyByCoinBtn.enabled = false;
                        buyByCoinBtn.image.sprite = disableSp;
                    }
                }
                else if (playerData.gemPrice > 0)
                {
                    unlockByAdBtn.gameObject.SetActive(false);
                    buyByCoinBtn.gameObject.SetActive(true);
                    buyByCoinBtn.transform.GetChild(1).gameObject.SetActive(false);
                    buyByCoinBtn.transform.GetChild(2).gameObject.SetActive(false);
                    buyByCoinBtn.transform.GetChild(3).gameObject.SetActive(true);
                    buyByCoinBtn.transform.GetChild(4).gameObject.SetActive(true);
                    buyByCoinBtn.transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = MoneyHelper.GetMoneyStringFormat(playerData.gemPrice);

                    if (MoneyHelper.GetMoney(isGem: true) >= curChar.gemPrice)
                    {
                        buyByCoinBtn.enabled = true;
                        buyByCoinBtn.image.sprite = greenSp;
                    }
                    else
                    {
                        buyByCoinBtn.enabled = false;
                        buyByCoinBtn.image.sprite = disableSp;
                    }
                }
                else
                {
                    unlockByAdBtn.gameObject.SetActive(true);
                    buyByCoinBtn.gameObject.SetActive(false);
                    unlockByAdBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = string.Format("({0}/{1})",
                        ES3.Load(StringDefine.ViewAdUnlockCharacter + playerData.playerType, 0), playerData.adAmount);
                }

                trialBtn.gameObject.SetActive(true);

                //unlockPriceTxt.text = playerData.coinPrice.ToString();


            }

            var sp = EntitySprites.instance.GetSkillSprite(playerData.playerType);
            infoSkillRect.gameObject.SetActive(sp != null);

            if (sp != null) {
                infoSkillImg.sprite = sp;
                infoSkillDesTxt.text = skillInfos[playerData.playerType].Item2;
                infoSkillNameTxt.text = skillInfos[playerData.playerType].Item1;
            }
        }
        else
        {
            //pickedBtn.gameObject.SetActive(false);
            //selectBtn.gameObject.SetActive(false);
            //unlockBtn.gameObject.SetActive(false);
        }
    }

    public void OnClickCloseBtn()
    {
        gameObject.SetActive(false);
        IronSourceAd.instance.ShowBanner();
    }
}
