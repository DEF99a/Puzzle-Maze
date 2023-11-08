using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using Assets.Scripts.Hellper;
using System.Linq;
using Assets.Scripts.Define;
using System;
using Spine.Unity;
using Assets.Scripts.Helper;
using Assets.Scripts.Common;
using DG.Tweening;
using Assets.Scripts.Model;

public class MenuUIManager : MonoBehaviour
{
    public static MenuUIManager instance;

    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI gemTxt;

    [SerializeField] private RectTransform questRectTransform;
    [SerializeField] private Button questBtn;
    [SerializeField] private TextMeshProUGUI questDesTxt;
    [SerializeField] private TextMeshProUGUI questReward;
    [SerializeField] private Image questCoinImg;
    [SerializeField] private Image questGemImg;
    [SerializeField] private Image shopNoticeImg;
    [SerializeField] private Sprite greenSp;
    [SerializeField] private Sprite violetSp;

    [SerializeField] private Button removeAdBtn;
    [SerializeField] private Button giftBtn;

    [SerializeField] private TextMeshProUGUI levelTxt;

    [SerializeField] private SkeletonGraphic skeletonGraphic;
    //[SerializeField] private Button unlockBtn;
    //[SerializeField] private Button pickedBtn;
    //[SerializeField] private Button selectBtn;
    //[SerializeField] private Button trialBtn;
    //[SerializeField] private TextMeshProUGUI priceUnlockTxt;

    [Header("speed up")]
    [SerializeField] private TextMeshProUGUI speedUpCurPercentTxt;
    [SerializeField] private List<Image> speedUpPercentImgs;
    [SerializeField] private Slider speedUpPercentSlider;
    [SerializeField] private Button speedUpBtn;
    [SerializeField] private TextMeshProUGUI speedUpPriceTxt;
    [SerializeField] private Image speedUpMaxBtn;


    [Header("life up")]
    [SerializeField] private TextMeshProUGUI lifeUpCurPercentTxt;
    [SerializeField] private List<Image> lifeUpPercentImgs;
    [SerializeField] private Slider lifeUpPercentSlider;
    [SerializeField] private Button lifeUpBtn;
    [SerializeField] private TextMeshProUGUI lifeUpPriceTxt;
    [SerializeField] private Image lifeUpMaxBtn;

    [SerializeField] public PanelLoadingAd panelLoadingAd;
    [SerializeField] private PanelShop panelShop;
    [SerializeField] private PanelCharacter panelCharacter;
    [SerializeField] private RectTransform panelSetting;
    [SerializeField] private RectTransform panelGift;

    [SerializeField] private Button playBtn;
    [SerializeField] private Button playTrialBtn;
    [SerializeField] private Button playWithoutTrialBtn;

    [SerializeField] private Button playerBtn;
    [SerializeField] private Sprite playerSpAndroid;
    [SerializeField] private Sprite playerSpIOS;

    public PanelQuest panelQuest;
    public PanelMap panelMap;

    private PlayerType curPlayerTypeIndex;
    private Coroutine showHideGiftBtnCor;
    private Vector3 questRectPos;

    private void Awake()
    {
        instance = this;

        var ironSourceAd = IronSourceAd.instance;
        var fireBaseManager = FireBaseManager.instance;
        var purchaser = Purchaser.Instance;

        this.RegisterListener(EventID.UpdateCoin, (dt) => CoinChange());
        this.RegisterListener(EventID.UpdateGem, (dt) => GemChange());
        this.RegisterListener(EventID.PickChar, (dt) =>
        {
            var playerSaver = PlayerHelper.GetCurrentPlayer();
            curPlayerTypeIndex = playerSaver.playerType;
            LoadPlayer();
        });
        this.RegisterListener(EventID.RemoveAd, (dt) =>
        {
            removeAdBtn.gameObject.SetActive(false);
        });
        this.RegisterListener(EventID.UpdateQuest, (dt) => ReceivedRequest(dt));
        this.RegisterListener(EventID.CompleteQuest, (dt) => ReceivedRequest(dt));

        playTrialBtn.onClick.RemoveAllListeners();
        playTrialBtn.onClick.AddListener(() =>
        {
            void actionPrepare()
            {
                panelLoadingAd.StartLoading();
            }
            void actionFailed()
            {
                panelLoadingAd.StopLoading();
            }
            void actionSuccess()
            {
                Config.SelectPlayerType = curPlayerTypeIndex;
                Config.SelectMap = 0;

                IronSourceAd.instance.HideBanner();
                SceneManager.LoadScene("InGame");
            }
            IronSourceAd.instance.ShowRewardedVideo("Trial-" + curPlayerTypeIndex, actionPrepare, actionSuccess, actionFailed);
        });

        playWithoutTrialBtn.onClick.RemoveAllListeners();
        playWithoutTrialBtn.onClick.AddListener(() =>
        {
            Play();
        });

    }

    private void ReceivedRequest(object dt)
    {
        UpdateQuest();
    }

    private void Start()
    {
        questRectPos = questRectTransform.localPosition;

        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
        //coinTxt.text = MoneyHelper.GetMoneyString(isGem: false);

        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);
        if (PurchaseHelper.HasRemoveAd()) removeAdBtn.gameObject.SetActive(false);
        levelTxt.text = "Level " + ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);

        var playerSaver = PlayerHelper.GetCurrentPlayer();
        curPlayerTypeIndex = playerSaver.playerType;
        LoadPlayer();

        //Config.SelectMap = 3;

        StartCoroutine(LoadAd());
        PurchaseHelper.RewardDailyVip();

        UpdateQuest();

        AudioManager.instance.StopMusic(SoundName.combat_music);
        AudioManager.instance.StopMusic(SoundName.puzzle_music);
        AudioManager.instance.PlayMusic(SoundName.ground_music);

        UpdateShopNotice();
        ShowHideGiftBtn();

        //if (Config.IsIOS) playerBtn.transform.GetChild(0).GetComponent<Image>().sprite = playerSpIOS;
        //else playerBtn.transform.GetChild(0).GetComponent<Image>().sprite = playerSpAndroid;
    }

    public void UpdateShopNotice()
    {
        shopNoticeImg.gameObject.SetActive(false);

        var date = ES3.Load(StringDefine.BuyFewPackageDateTime, DateTime.Now.AddDays(-1));
        if (date <= DateTime.Now)
        {
            shopNoticeImg.sprite = greenSp;
            shopNoticeImg.gameObject.SetActive(true);
        }
        else
        {
            date = ES3.Load(StringDefine.BuyStackPackageDateTime, DateTime.Now);
            if (date <= DateTime.Now)
            {
                shopNoticeImg.sprite = violetSp;
                shopNoticeImg.gameObject.SetActive(true);
            }
        }
    }

    IEnumerator LoadAd()
    {
        yield return new WaitForSeconds(1f);
        IronSourceAd.instance.LoadBanner();
        IronSourceAd.instance.ShowBanner();
    }

    private void UnLockCurChar()
    {
        var inList = PlayerDatas.instance.GetPlayerData(curPlayerTypeIndex);
        if (MoneyHelper.GetMoney(isGem: false) >= inList.coinPrice)
        {
            PlayerHelper.AddPlayer(new PlayerSaver()
            {
                playerType = inList.playerType
            });
            MoneyHelper.AddMoney(-inList.coinPrice, isGem: false);
            LoadPlayer();
        }
    }

    private void PickCurChar()
    {
        var players = PlayerHelper.GetPlayers();
        var inList = players.FirstOrDefault(p => p.playerType == curPlayerTypeIndex);
        PlayerHelper.UpdatePlayer(inList);
        LoadPlayer();
    }

    private void GemChange()
    {
        gemTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: true), 99_999);
    }

    private void CoinChange()
    {
        coinTxt.text = MoneyHelper.GetMoneyStringFormat(MoneyHelper.GetMoney(isGem: false), 99_999);
    }

    public void Play()
    {
        Config.SelectMap = 0;
        Config.SelectPlayerType = PlayerHelper.GetCurrentPlayer().playerType;

        IronSourceAd.instance.HideBanner();
        SceneManager.LoadScene("InGame");
    }

    private void UpdateQuest()
    {
        questRectTransform.gameObject.SetActive(false);

        var redImage = questBtn.transform.GetChild(1).GetComponent<Image>();
        redImage.gameObject.SetActive(false);

        var quests = QuestHelper.GetQuests();
        for (int k = 0; k < quests.Count; k++)
        {
            var quest = quests[k];
            if (quest.canRev == 0 && quest.myComplete >= quest.requestAmount)
            {
                if (!redImage.gameObject.activeSelf) redImage.gameObject.SetActive(true);
                Debug.Log("quest.showedNotice:" + quest.showedNotice);

                if (!quest.showedNotice)
                {
                    AudioManager.instance.PlaySound(SoundName.quest_noti);
                    questRectTransform.gameObject.SetActive(true);
                    //questDesTxt.text = QuestItemView.questDes2[quest.questType];
                    EnemyType enemyType = EnemyType.None;

                    if (quest.questType == QuestType.KillEnemy)
                    {
                        enemyType = ((QuestKill)quest).enemyType;
                    }
                    questDesTxt.text = string.Format(QuestItemView.questDes[quest.questType], quest.requestAmount, enemyType);

                    questGemImg.gameObject.SetActive(false);
                    questReward.text = quest.coinReward.ToString();
                    quest.showedNotice = true;
                    ES3.Save(StringDefine.Quests, quests);
                    break;
                }

                //questBtn.onClick.RemoveAllListeners();
                //questBtn.onClick.AddListener(() =>
                //{
                //    QuestHelper.Complete(quest);

                //    this.PostEvent(EventID.CompleteQuest, (quest.coinReward, Vector3.zero, false));
                //});
            }
        }

        //questRectTransform.gameObject.SetActive(true);

        if (questRectTransform.gameObject.activeSelf)
        {
            StartCoroutine(HideQuestNotice());
        }

        IEnumerator HideQuestNotice()
        {
            var tmp = questRectPos;
            tmp.y += 200;
            questRectTransform.localPosition = tmp;
            yield return new WaitForSeconds(1f);

            questRectTransform.DOLocalMoveY(questRectTransform.localPosition.y - 200, 1f);

            yield return new WaitForSeconds(3f);

            questRectTransform.DOLocalMoveY(questRectTransform.localPosition.y + 200, 1f).OnComplete(() =>
            {
                questRectTransform.gameObject.SetActive(false);
            });
        }
    }

    private void LoadSpeedUp(int percent, bool unlocked)
    {
        speedUpBtn.interactable = unlocked;

        speedUpBtn.gameObject.SetActive(percent != 200);
        speedUpMaxBtn.gameObject.SetActive(percent == 200);

        var tmpPercent = percent;
        if (percent >= 100) tmpPercent -= 100;

        speedUpCurPercentTxt.text = string.Format("+{0}%", percent / 20 * Config.SpeedUpPercent);
        if (percent != 200)
            speedUpPriceTxt.text = Config.SpeedUpPrices[percent / 20].ToString();

        var nextPercent = tmpPercent + 20;
        speedUpPercentSlider.maxValue = 100;
        speedUpPercentSlider.value = nextPercent;

        var index = tmpPercent / 20;
        for (int k = 0; k < speedUpPercentImgs.Count; k++)
        {
            if (k == index)
            {
                speedUpPercentImgs[k].gameObject.SetActive(true);
            }
            else
                speedUpPercentImgs[k].gameObject.SetActive(false);
        }
    }

    private void LoadLifeUp(int percent, bool unlocked)
    {
        lifeUpBtn.interactable = unlocked;

        lifeUpBtn.gameObject.SetActive(percent != 10);
        lifeUpMaxBtn.gameObject.SetActive(percent == 10);

        var tmpPercent = percent;
        if (percent >= 5) tmpPercent -= 5;

        Debug.Log("percent:" + percent);

        lifeUpCurPercentTxt.text = string.Format("+{0}", percent);
        if (percent != 10) lifeUpPriceTxt.text = Config.LifeUpPrices[percent].ToString();

        var nextPercent = tmpPercent + 1;
        lifeUpPercentSlider.maxValue = 5;
        lifeUpPercentSlider.value = nextPercent;

        var index = tmpPercent;
        for (int k = 0; k < lifeUpPercentImgs.Count; k++)
        {
            if (k == index)
            {
                lifeUpPercentImgs[k].gameObject.SetActive(true);
            }
            else
                lifeUpPercentImgs[k].gameObject.SetActive(false);
        }
    }

    public void OnClickSpeedUpBtn()
    {
        var playerSaver = PlayerHelper.GetCurrentPlayer();

        var index = playerSaver.speedUpPercent / 20;
        var price = Config.SpeedUpPrices[index];
        if (MoneyHelper.GetMoney(isGem: false) >= price)
        {
            AudioManager.instance.PlaySound(SoundName.click_btn_common);
            MoneyHelper.AddMoney(-price, isGem: false);
        }
        else
        {
            AudioManager.instance.PlaySound(SoundName.click_fail);
            return;
        }
        QuestHelper.Action(Assets.Scripts.Model.QuestType.Upgrade, 1);

        playerSaver.speedUpPercent += 20;
        PlayerHelper.UpdatePlayer(playerSaver);
        LoadSpeedUp(playerSaver.speedUpPercent, true);

        this.PostEvent(EventID.UpdateQuest, null);
        AudioManager.instance.PlaySound(SoundName.upgrade_speed, 0.2f);
    }

    public void OnClickLifeUpBtn()
    {
        var playerSaver = PlayerHelper.GetCurrentPlayer();

        var index = playerSaver.lifeUpPercent;
        var price = Config.LifeUpPrices[index];
        if (MoneyHelper.GetMoney(isGem: true) >= price)
        {
            AudioManager.instance.PlaySound(SoundName.click_btn_common);
            MoneyHelper.AddMoney(-price, isGem: true);
        }
        else
        {
            AudioManager.instance.PlaySound(SoundName.click_fail);
            return;
        }
        QuestHelper.Action(Assets.Scripts.Model.QuestType.Upgrade, 1);

        playerSaver.lifeUpPercent += 1;
        PlayerHelper.UpdatePlayer(playerSaver);
        LoadLifeUp(playerSaver.lifeUpPercent, true);

        this.PostEvent(EventID.UpdateQuest, null);
        AudioManager.instance.PlaySound(SoundName.upgrade_life, 0.2f);
    }

    private void LoadPlayer()
    {
        Debug.Log("curPlayerTypeIndex:" + curPlayerTypeIndex);

        PlayerHelper.LoadSkeleton(this.curPlayerTypeIndex, skeletonGraphic);

        var curPlayer = PlayerHelper.GetCurrentPlayer();
        var players = PlayerHelper.GetPlayers();
        var inList = players.FirstOrDefault(p => p.playerType == curPlayerTypeIndex);

        int speedUpPercent = 0;
        int lifeUpPercent = 0;
        bool unlocked = false;

        playBtn.gameObject.SetActive(true);
        playTrialBtn.gameObject.SetActive(false);
        playWithoutTrialBtn.gameObject.SetActive(false);
        if (curPlayer.playerType == curPlayerTypeIndex)
        {
            //pickedBtn.gameObject.SetActive(true);
            speedUpPercent = curPlayer.speedUpPercent;
            lifeUpPercent = curPlayer.lifeUpPercent;
            unlocked = true;
        }
        else if (inList == null)
        {
            playBtn.gameObject.SetActive(false);
            playTrialBtn.gameObject.SetActive(true);
            playWithoutTrialBtn.gameObject.SetActive(true);
            playTrialBtn.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = "Level " + ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);

            //unlockBtn.gameObject.SetActive(true);
            //priceUnlockTxt.text = PlayerDatas.instance.GetPlayerData(curPlayerTypeIndex).coinPrice.ToString();
        }
        else
        {
            //selectBtn.gameObject.SetActive(true);
            speedUpPercent = inList.speedUpPercent;
            lifeUpPercent = inList.lifeUpPercent;
            unlocked = true;
        }

        LoadSpeedUp(speedUpPercent, unlocked);
        LoadLifeUp(lifeUpPercent, unlocked);
    }

    public void OnClickNextPlayer()
    {
        var players = PlayerDatas.instance.GetPlayerDatas();
        var id = players.FindIndex(a => a.playerType == curPlayerTypeIndex);
        id += 1;
        if (id >= PlayerDatas.instance.GetPlayerDatas().Count)
        {
            id = 0;
        }
        curPlayerTypeIndex = players[id].playerType;

        //var index = (int)curPlayerTypeIndex;
        //index += 1;
        ////if (index >= Enum.GetNames(typeof(PlayerType)).Length)
        //if (index >= PlayerDatas.instance.GetPlayerDatas().Count)
        //{
        //    index = 1;
        //}

        //curPlayerTypeIndex = (PlayerType)index;
        LoadPlayer();
    }

    public void OnClickPrevPlayer()
    {
        var players = PlayerDatas.instance.GetPlayerDatas();
        var id = players.FindIndex(a => a.playerType == curPlayerTypeIndex);
        id -= 1;
        if (id <= 0)
        {
            id = players.Count - 1;
        }
        curPlayerTypeIndex = players[id].playerType;

        //var index = (int)curPlayerTypeIndex;
        //index -= 1;
        //if (index <= 0)
        //{
        //    index = Enum.GetNames(typeof(PlayerType)).Length - 1;
        //    index = PlayerDatas.instance.GetPlayerDatas().Count - 1;
        //}

        //curPlayerTypeIndex = (PlayerType)index;
        LoadPlayer();
    }

    public void ViewAd()
    {
        void actionPrepare()
        {
            panelLoadingAd.StartLoading();
        }
        void actionFailed()
        {
            panelLoadingAd.StopLoading();
        }
        void actionSuccess()
        {
            MoneyHelper.AddMoney(Config.CoinViewAdAtHome, isGem: false);
            LoadPlayer();
        }
        IronSourceAd.instance.ShowRewardedVideo("Menu", actionPrepare, actionSuccess, actionFailed);
    }

    public void ShowShop()
    {
        //IronSourceAd.instance.HideBanner();
        panelShop.Show(0);
    }

    public void ShowCharacterPanel()
    {
        IronSourceAd.instance.HideBanner();
        panelCharacter.gameObject.SetActive(true);
    }

    public void ShowSettingPanel()
    {
        //IronSourceAd.instance.HideBanner();
        panelSetting.gameObject.SetActive(true);
    }

    public void OnClickCloseSettingPanel()
    {
        //IronSourceAd.instance.ShowBanner();
        panelSetting.gameObject.SetActive(false);
    }

    public void RestoreBtnClicked()
    {
    }

    public void ShowQuestPanel()
    {
        //IronSourceAd.instance.HideBanner();
        panelQuest.Show();
    }

    public void ShowMapPanel()
    {
        //IronSourceAd.instance.HideBanner();
        panelMap.gameObject.SetActive(true);
    }

    public void OnClickMoreGem()
    {
        panelShop.Show(2);
    }

    public void OnClickMoreCoin()
    {
        panelShop.Show(1);
    }

    public void RemoveAd()
    {
    }

    private void ShowHideGiftBtn()
    {
        var revAmount = ES3.Load(StringDefine.RevGiftAmount, 0);
        var time = ES3.Load(StringDefine.RevGiftTime, DateTime.Now);
        giftBtn.gameObject.SetActive(time <= DateTime.Now && revAmount < 10);
        if (time <= DateTime.Now && revAmount < 10)
        {
            giftBtn.onClick.RemoveAllListeners();
            giftBtn.onClick.AddListener(() =>
            {
                panelGift.gameObject.SetActive(true);
            });

            if (showHideGiftBtnCor == null)
                showHideGiftBtnCor = StartCoroutine(ShowHideGiftBtnIE());
        }
        IEnumerator ShowHideGiftBtnIE()
        {
            while (true)
            {
                yield return new WaitForSeconds(5f);
                revAmount = ES3.Load(StringDefine.RevGiftAmount, 0);
                time = ES3.Load(StringDefine.RevGiftTime, DateTime.Now);
                giftBtn.gameObject.SetActive(time <= DateTime.Now && revAmount < 10);
            }
        }
    }

    public void OnClickRevGift()
    {
        void actionPrepare()
        {
            panelLoadingAd.StartLoading();
        }
        void actionFailed()
        {
            panelLoadingAd.StopLoading();
        }
        void actionSuccess()
        {
            MoneyHelper.AddMoney(5, isGem: true);

            ES3.Save(StringDefine.RevGiftTime, DateTime.Now.AddMinutes(10));

            var n = ES3.Load(StringDefine.RevGiftAmount, 0) + 1;
            ES3.Save(StringDefine.RevGiftAmount, n);
            giftBtn.gameObject.SetActive(false);

            if (n >= 10)
            {
                if (showHideGiftBtnCor != null)
                {
                    StopCoroutine(showHideGiftBtnCor);
                    showHideGiftBtnCor = null;
                }
            }
            panelGift.gameObject.SetActive(false);
        }
        IronSourceAd.instance.ShowRewardedVideo("ReceiveGift", actionPrepare, actionSuccess, actionFailed);
    }
}
