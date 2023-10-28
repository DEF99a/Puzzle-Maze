using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Assets.Scripts.Model;
using Assets.Scripts.Define;
using System;
using Assets.Scripts.Helper;
using Assets.Scripts.Hellper;

public class QuestItemView : MonoBehaviour
{
    public static Dictionary<QuestType, string> questDes = new Dictionary<QuestType, string>()
    {
        {QuestType.KillEnemy, "Kill {0} {1}" }, {QuestType.Upgrade, "Upgrade {0} times" },
        {QuestType.PassLevel, "Pass {0} levels" }, {QuestType.PassGameNoDie, "Pass game {0} times and no die" },
        {QuestType.UseItem, "Use {0} {1} times" }, {QuestType.ViewAd, "Watch the ad and get " },
        {QuestType.CollectGold, "Collect {0} Gold" }
    };
    public static Dictionary<QuestType, string> questDes2 = new Dictionary<QuestType, string>()
    {
        {QuestType.KillEnemy, "Kill {0} {1}" }, {QuestType.Upgrade, "Upgrade {0} times" },
        {QuestType.PassLevel, "Pass {0} levels" }, {QuestType.PassGameNoDie, "Pass game {0} times and no die" },
        {QuestType.UseItem, "Use {0} {1} times" }, {QuestType.ViewAd, "Watch the ad and get " },
        {QuestType.CollectGold, "Collect {0} Gold" }
    };

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI desTxt;

    [SerializeField] private Button btnAction;
    [SerializeField] private Sprite canRevSp;
    [SerializeField] private Sprite canNotRevSp;
    [SerializeField] private Sprite receivedSp;
    [SerializeField] private Sprite watchAdSp;
  
    [SerializeField] private Slider processSlider;
    [SerializeField] private TextMeshProUGUI processTxt;
    [SerializeField] private TextMeshProUGUI rewardTxt;

    [SerializeField] private RectTransform coinRect;

    private QuestModel mQuestModel;
    private TextMeshProUGUI actionTxt;
    private Image actionAdImg;

    private void Awake()
    {
        btnAction.onClick.RemoveAllListeners();
        btnAction.onClick.AddListener(ActionButton);

        actionTxt = btnAction.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        actionAdImg = btnAction.transform.GetChild(1).GetComponent<Image>();
    }

    private void ActionButton()
    {
        if (mQuestModel.myComplete >= mQuestModel.requestAmount)
        {
            // can reve
            QuestHelper.Complete(mQuestModel);
           
            btnAction.image.sprite = receivedSp;
            btnAction.GetComponentInChildren<TextMeshProUGUI>().text = "Claimed";
            btnAction.enabled = false;

            this.PostEvent(EventID.CompleteQuest, (mQuestModel.coinReward, coinRect.position, false));
        }
        else
        {
            if (mQuestModel.questType == QuestType.ViewAd)
            {
                void actionPrepare()
                {
                    MenuUIManager.instance.panelLoadingAd.StartLoading();
                }
                void actionFailed()
                {
                    MenuUIManager.instance.panelLoadingAd.StopLoading();
                }
                Action actionSuccess = () =>
                {
                    MenuUIManager.instance.panelLoadingAd.StopLoading();

                    QuestHelper.Action(QuestType.ViewAd, 1);

                    //mQuestModel.myComplete++;
                    //Setup(mQuestModel);
                    this.PostEvent(EventID.UpdateQuest, null);
                };
                IronSourceAd.instance.ShowRewardedVideo("WatchVideoQuest", actionPrepare, actionSuccess, actionFailed);
            }
            else
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
                    MenuUIManager.instance.panelLoadingAd.StopLoading();

                    QuestHelper.ChangeQuest(mQuestModel);

                    this.PostEvent(EventID.UpdateQuest, null);
                }
                IronSourceAd.instance.ShowRewardedVideo("ChangeQuest", actionPrepare, actionSuccess, actionFailed);
            }
        }
    }

    public void Setup(QuestModel questModel)
    {
        mQuestModel = questModel;

        EnemyType enemyType = EnemyType.None;
        EntityType entityType = EntityType.None;

        if (questModel.questType == QuestType.KillEnemy)
        {
            var q = (QuestKill)questModel;
            enemyType = q.enemyType;
            desTxt.text = string.Format(questDes[questModel.questType], questModel.requestAmount, q.enemyType);
        }
        else if (questModel.questType == QuestType.UseItem)
        {
            var q = (QuestUseItem)questModel;
            entityType = q.entityType;
            
            var str = q.entityType.ToString();
            str = string.Format("Item {0}", str.Replace("Item", ""));
            desTxt.text = string.Format(questDes[questModel.questType], str, questModel.requestAmount);
        }
        else
            desTxt.text = string.Format(questDes[questModel.questType], questModel.requestAmount);

        var sprite = EntitySprites.instance.GetQuestSprite(questModel.questType, enemyType, entityType);
        if (sprite != null)
            icon.sprite = sprite;

        if (questModel.canRev == 1)
        {
            btnAction.image.sprite = receivedSp;
            actionTxt.text = "Claimed";
            btnAction.enabled = false;
        }
        else
        {
            btnAction.enabled = true;
            if (questModel.myComplete >= questModel.requestAmount)
            {
                // can reve
                btnAction.image.sprite = canRevSp;
                actionTxt.text = "Collect";
                actionAdImg.gameObject.SetActive(false);
            }
            else
            {
                btnAction.image.sprite = canNotRevSp;
                actionAdImg.gameObject.SetActive(true);
                if (questModel.questType == QuestType.ViewAd)
                {
                    btnAction.image.sprite = watchAdSp;
                    actionTxt.text = "Watch";
                }
                else actionTxt.text = "Change";
            }
        }
        processSlider.maxValue = questModel.requestAmount;
        processSlider.value = questModel.myComplete;
        processTxt.text = string.Format("{0}/{1}", questModel.myComplete, questModel.requestAmount);

        rewardTxt.text = questModel.coinReward.ToString();
    }
}
