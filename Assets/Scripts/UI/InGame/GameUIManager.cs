using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Assets.Scripts.Define;
using Assets.Scripts.Helper;
using DG.Tweening;
using System;
using Assets.Scripts.Common;
using UnityEngine.UI;
using Assets.Scripts.Hellper;
using Spine.Unity;
using Spine;
using Assets.Scripts.Model;

public class GameUIManager : MonoBehaviour
{
    public static GameUIManager instance;

    [SerializeField] private TextMeshProUGUI endGameTxt;
    [SerializeField] private RectTransform gameboy;
    [SerializeField] private RectTransform joystick;

    [SerializeField] private RectTransform lifeIcon;
    [SerializeField] private TextMeshProUGUI lifeAmountTxt;
    [SerializeField] private RectTransform timeIcon;
    [SerializeField] private TextMeshProUGUI minuteTxt;
    [SerializeField] private TextMeshProUGUI secondTxt;
    [SerializeField] private RectTransform coinIcon;
    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private RectTransform x2Icon;
    [SerializeField] private TextMeshProUGUI x2Txt;

    [SerializeField] private TextMeshProUGUI diedMonsterTxt;

    [SerializeField] private TextMeshProUGUI angryAmountTxt;
    [SerializeField] private TextMeshProUGUI freezeAmountTxt;
    [SerializeField] private TextMeshProUGUI invisibleAmountTxt;
    [SerializeField] private PanelPause panelPause;
    [SerializeField] private PanelWin2 panelWin;
    [SerializeField] private PanelLoose panelLoose;

    [SerializeField] private RectTransform levelNoticeRect;
    [SerializeField] private Button actionBtn;
    [SerializeField] private RectTransform leftRect;
    [SerializeField] private RectTransform shootBtn;
    [SerializeField] private Image gameNotice;

    public PanelLoadingAd panelLoadingAd;

    private bool isPause;
    private float itemSpeed = 130;

    [HideInInspector] public int freezeAmount, angryAmount, invisibleAmount;
    [HideInInspector] public int moreFreeze, moreAngry, moreInvisible;
    Queue<string> queue = new Queue<string>();
    Coroutine gameNoticeCor;
    private DateTime shootDateTime;

    private void Awake()
    {
        instance = this;
        this.RegisterListener(EventID.UpdateController, (dt) =>
        {
            UpdateController(dt);

            var isJoystick = ES3.Load(StringDefine.IsJoystick, 0);
            if (isJoystick == 1)
                FireBaseManager.instance.LogJoystickMode(MapManager.instance.CurLevelNumber);
            else
                FireBaseManager.instance.LogGamepadMode(MapManager.instance.CurLevelNumber);
        });
        //this.RegisterListener(EventID.CompleteQuestInGame, (dt) =>
        //{
        //    (int, EnemyType, QuestType) res = (ValueTuple<int, EnemyType, QuestType>)dt;
        //    var text = string.Format(QuestItemView.questDes[res.Item3], res.Item1, res.Item2);
        //    ShowGameNotice("Complete quest: " + text);
        //});

        gameboy.gameObject.SetActive(false);
        joystick.gameObject.SetActive(false);
        actionBtn.gameObject.SetActive(false);
        shootBtn.gameObject.SetActive(false);
    }

    private void UpdateController(object obj)
    {
        var isJoystick = ES3.Load(StringDefine.IsJoystick, 0);
        gameboy.gameObject.SetActive(isJoystick != 1);
        joystick.gameObject.SetActive(isJoystick == 1);
    }

    void Start()
    {
        var ironSourceAd = IronSourceAd.instance;
        var fireBaseManager = FireBaseManager.instance;

        isPause = false;
        freezeAmount = ES3.Load(StringDefine.PlayerFreezeItemAmount, 0);
        angryAmount = ES3.Load(StringDefine.PlayerAngryItemAmount, 0);
        invisibleAmount = ES3.Load(StringDefine.PlayerInvisibleItemAmount, 0);
        if (freezeAmount < 2) moreFreeze = 2 - freezeAmount;
        if (angryAmount < 2) moreAngry = 2 - angryAmount;
        if (invisibleAmount < 2) moreInvisible = 2 - invisibleAmount;

        UpdateFreeze(freezeAmount + moreFreeze);
        UpdateInvisible(invisibleAmount + moreInvisible);
        UpdateAngry(angryAmount + moreAngry);
    }

    public void ShowController()
    {
        AudioManager.instance?.PlaySound(SoundName.ready);

        UpdateController(null);
        PlayerType playerType = Config.SelectPlayerType;
        if (playerType == PlayerType.None) playerType = PlayerHelper.GetCurrentPlayer().playerType;

        //var playerType = PlayerHelper.GetCurrentPlayer().playerType;
        actionBtn.gameObject.SetActive(true);
        var sp = EntitySprites.instance.GetSkillSprite(playerType);
        if (sp != null) actionBtn.transform.GetChild(2).GetComponent<Image>().sprite = sp;

        actionBtn.transform.GetChild(0).gameObject.SetActive(false);
        actionBtn.transform.GetChild(3).gameObject.SetActive(false);

        shootBtn.gameObject.SetActive(true);
    }

    public void ShowLevelNotice()
    {
        //if (MapManager.instance.CurLevelNumber == Config.FirstLevel) return;
        levelNoticeRect.gameObject.SetActive(false);

        StartCoroutine(ShowLevelNoticeIE());
        IEnumerator ShowLevelNoticeIE()
        {
            var skeletonGraphic = levelNoticeRect.GetChild(0).GetComponent<SkeletonGraphic>();
            var txt = levelNoticeRect.GetChild(1).GetComponent<TextMeshProUGUI>();
            txt.gameObject.SetActive(false);

            yield return new WaitForSeconds(0.2f);

            skeletonGraphic.AnimationState.Complete += HandleAnimComplete;
            skeletonGraphic.AnimationState.SetAnimation(0, "start", false);
            txt.text = "Level " + MapManager.instance.CurLevelNumber;

            StartCoroutine(TextFadeIE(false));
            txt.gameObject.SetActive(true);

            void HandleAnimComplete(TrackEntry trackEntry)
            {
                if (trackEntry.Animation.Name.Equals("start"))
                {
                    skeletonGraphic.AnimationState.SetAnimation(0, "loop", true);

                    _ = StartCoroutine(WattingToEndIE());
                }
                else if (trackEntry.Animation.Name.Equals("end"))
                {
                    levelNoticeRect.gameObject.SetActive(false);
                }
            }
            levelNoticeRect.gameObject.SetActive(true);

            IEnumerator WattingToEndIE()
            {
                float f = 0.6f;
                while (f > 0)
                {
                    yield return new WaitForSeconds(0.05f);
                    f -= 0.1f;
                }
                skeletonGraphic.AnimationState.SetAnimation(0, "end", false);

                StartCoroutine(TextFadeIE(true));
            }

            IEnumerator TextFadeIE(bool fadeIn)
            {
                if (fadeIn)
                {
                    float f = 1;
                    while (f > 0.3f)
                    {
                        txt.transform.localScale = new Vector3(f, f, 1);
                        f -= 0.1f;
                        yield return new WaitForSeconds(0.01f);
                    }
                    txt.transform.localScale = new Vector3(0, 0, 1);
                    //this.PostEvent(EventID.CompleteQuestInGame, (1, EnemyType.None, QuestType.CollectGold));
                    //this.PostEvent(EventID.CompleteQuestInGame, (2, EnemyType.Boo, QuestType.KillEnemy));
                }
                else
                {
                    float f = 0;
                    while (f < 1)
                    {
                        txt.transform.localScale = new Vector3(f, f, 1);
                        f += 0.1f;
                        yield return new WaitForSeconds(0.01f);
                    }
                    txt.transform.localScale = new Vector3(1, 1, 1);
                }

            }
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region Update Info
    public void UpdateAll(int life, int second, int coin, int x2, int nDiedEnemies, int totalEnemies, int angryAmount, int freezeAmount, int invisibleAmount)
    {
        UpdateLife(life);
        UpdateTime(second);
        UpdateCoin(coin);
        UpdateX2(x2);
        UpdateAngry(angryAmount);
        UpdateInvisible(invisibleAmount);
        UpdateFreeze(freezeAmount);
        UpdateDiedMonster(nDiedEnemies, totalEnemies);
    }

    public void UpdateLife(int life)
    {
        lifeAmountTxt.text = "x" + life.ToString();
    }

    public void UpdateTime(int second)
    {
        GeneralHelper.FormatTime(second, out string _, out string m, out string s);
        minuteTxt.text = m;
        secondTxt.text = s;
    }

    public void UpdateCoin(int coin)
    {
        coinTxt.text = coin.ToString();
    }

    public void MoveToCoin(Transform source, Action action)
    {
        StartCoroutine(GeneralHelper.MoveFollowTarget(source, coinIcon, action, itemSpeed));
    }

    public void UpdateX2(int coin)
    {
        x2Txt.text = "x" + coin.ToString();
    }

    public void UpdateAngry(int coin)
    {
        if (coin != 0)
        {
            angryAmountTxt.gameObject.SetActive(true);
            angryAmountTxt.text = "x" + coin.ToString();
            angryAmountTxt.transform.parent.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            angryAmountTxt.gameObject.SetActive(false);
            angryAmountTxt.transform.parent.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void UpdateInvisible(int coin)
    {
        if (coin != 0)
        {
            invisibleAmountTxt.gameObject.SetActive(true);
            invisibleAmountTxt.text = "x" + coin.ToString();
            invisibleAmountTxt.transform.parent.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            invisibleAmountTxt.gameObject.SetActive(false);
            invisibleAmountTxt.transform.parent.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void UpdateFreeze(int coin)
    {
        if (coin != 0)
        {
            freezeAmountTxt.gameObject.SetActive(true);
            freezeAmountTxt.text = "x" + coin.ToString();
            freezeAmountTxt.transform.parent.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            freezeAmountTxt.gameObject.SetActive(false);
            freezeAmountTxt.transform.parent.GetChild(2).gameObject.SetActive(true);
        }
    }

    public void UpdateDiedMonster(int numKilled, int total)
    {
        diedMonsterTxt.text = string.Format("{0}/{1}", numKilled, total);
    }
    #endregion

    public void PlayerEatItem(Vector3 pos, EntityType entityType)
    {
        var go = ObjectPool.instance.GetGameObject(PrefabCache.instance.itemSpritePf, pos, Quaternion.identity);
        go.GetComponent<ItemSprite>().Init(entityType);
        var source = go.transform;

        switch (entityType)
        {
            case EntityType.ItemLife:
                StartCoroutine(GeneralHelper.MoveFollowTarget(source, lifeIcon, () =>
                   {
                       GameManager.instance.PlayerEatItem(entityType);
                   }, itemSpeed));
                break;
            case EntityType.ItemCoin100:
            case EntityType.ItemCoin200:
            case EntityType.ItemCoin500:
            case EntityType.ItemCoin1000:
            case EntityType.ItemCoin2000:
                int coinAdd = 0;
                if (entityType == EntityType.ItemCoin100) coinAdd = 10;
                else if (entityType == EntityType.ItemCoin200) coinAdd = 20;
                else if (entityType == EntityType.ItemCoin500) coinAdd = 50;
                else if (entityType == EntityType.ItemCoin1000) coinAdd = 100;
                else if (entityType == EntityType.ItemCoin2000) coinAdd = 200;
                var go2 = ObjectPool.instance.GetGameObject(PrefabCache.instance.coinFlyUp, pos, Quaternion.identity);
                go2.GetComponent<CoinFlyUp>().Setup(coinAdd);
                StartCoroutine(GeneralHelper.MoveFollowTarget(source, coinIcon, () =>
                {
                    GameManager.instance.PlayerEatItem(entityType);
                }, itemSpeed));
                break;
            case EntityType.ItemTimer5:
            case EntityType.ItemTimer10:
            case EntityType.ItemTimer20:
                StartCoroutine(GeneralHelper.MoveFollowTarget(source, timeIcon, () =>
                {
                    GameManager.instance.PlayerEatItem(entityType);
                }, itemSpeed));
                break;
            case EntityType.ItemCoinRewardEndGameX2:
            case EntityType.ItemCoinRewardEndGameX3:
                StartCoroutine(GeneralHelper.MoveFollowTarget(source, x2Icon, () =>
                {
                    GameManager.instance.PlayerEatItem(entityType);
                }, itemSpeed));
                break;
        }
    }

    public void ClickPause()
    {
        Time.timeScale = 0f;
        panelPause.gameObject.SetActive(true);
    }

    public void EndGame(bool isWin, GameEndReason gameEndReason)
    {
        //endGameTxt.text = isWin ? "You Win!" : "You Loose";

        //MapManager.instance.ResetMovePoints();
        if (isWin)
        {
            panelWin.gameObject.SetActive(true);
        }
        else
        {
            panelLoose.Show(gameEndReason);
        }
    }

    #region Click Control
    public void EnterClickMoveUp()
    {
        PlayerController.instance?.SetDirectionButton(DynamicEntityDirection.Up);
    }

    public void EnterClickMoveDown()
    {
        PlayerController.instance?.SetDirectionButton(DynamicEntityDirection.Down);
    }

    public void EnterClickMoveRight()
    {
        PlayerController.instance?.SetDirectionButton(DynamicEntityDirection.Right);
    }

    public void EnterClickMoveLeft()
    {
        PlayerController.instance?.SetDirectionButton(DynamicEntityDirection.Left);
    }

    public void ExitClickMoveButton()
    {
        PlayerController.instance?.SetDirectionButton(DynamicEntityDirection.None);
    }

    public void ClickFireButton()
    {
        if (shootDateTime > DateTime.Now) return;
        shootDateTime = DateTime.Now.AddMilliseconds(200);

        PlayerController.instance?.Shoot();
    }

    public void ExitFireButton()
    {
        //PlayerController.instance?.EndShoot();
    }

    public void ClickActionButton()
    {
        PlayerController.instance?.StartAction();
       
        PlayerType playerType = Config.SelectPlayerType;
        if (playerType == PlayerType.None) playerType = PlayerHelper.GetCurrentPlayer().playerType;
        if (playerType == PlayerType.Main2 || playerType == PlayerType.Dinosaur 
            || playerType == PlayerType.Main1 || playerType == PlayerType.Mario2)
        {
        }
        else return;

        var img = actionBtn.transform.GetChild(0).gameObject.GetComponent<Image>();
        img.gameObject.SetActive(true);
        actionBtn.transform.GetChild(3).gameObject.SetActive(true);
        actionBtn.enabled = false;

        StartCoroutine(CountDown(10));
        IEnumerator CountDown(float totalTime)
        {
            float time = totalTime;
            img.fillAmount = time  / totalTime;
            while (true)
            {
                yield return new WaitForSeconds(0.1f);
                time -= 0.1f;
                img.fillAmount = time / totalTime;
                if (time <= 0)
                {
                    break;
                }
            }
            img.fillAmount = 0;
            actionBtn.enabled = true;
            img.gameObject.SetActive(false);
            actionBtn.transform.GetChild(3).gameObject.SetActive(false);
        }
    }
    #endregion

    #region Click Item
    public void ClickInvisible()
    {
        var am = invisibleAmount + moreInvisible;
        if (am == 0)
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
                moreInvisible += 1;
                UpdateInvisible(invisibleAmount + moreInvisible);
            }
            IronSourceAd.instance.ShowRewardedVideo("InGameClickInvisible", actionPrepare, actionSuccess, actionFailed);
        }
        else
        {
            if (moreInvisible > 0) moreInvisible -= 1;
            else if (invisibleAmount > 0)
            {
                invisibleAmount -= 1;
                ES3.Save(StringDefine.PlayerInvisibleItemAmount, invisibleAmount);
            }
            UpdateInvisible(moreInvisible + invisibleAmount);

            PlayerController.instance.ProcessActiveItem(EntityType.ItemInvisible, null);
        }
    }

    public void ClickAngry()
    {
        var am = angryAmount + moreAngry;
        if (am == 0)
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
                moreAngry += 1;
                UpdateAngry(angryAmount + moreAngry);
            }
            IronSourceAd.instance.ShowRewardedVideo("InGameClickAngry", actionPrepare, actionSuccess, actionFailed);
        }
        else
        {
            if (moreAngry > 0) moreAngry -= 1;
            else if (angryAmount > 0)
            {
                angryAmount -= 1;
                ES3.Save(StringDefine.PlayerAngryItemAmount, angryAmount);
            }
            UpdateAngry(angryAmount + moreAngry);

            PlayerController.instance.ProcessActiveItem(EntityType.ItemAngry, null);
        }
    }

    public void ClickFreeze()
    {
        var am = moreFreeze + freezeAmount;
        if (am == 0)
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
                moreFreeze += 1;
                UpdateFreeze(moreFreeze + freezeAmount);
            }
            IronSourceAd.instance.ShowRewardedVideo("InGameClickFreeze", actionPrepare, actionSuccess, actionFailed);
        }
        else
        {
            if (moreFreeze > 0) moreFreeze -= 1;
            else if (freezeAmount > 0)
            {
                freezeAmount -= 1;
                ES3.Save(StringDefine.PlayerFreezeItemAmount, freezeAmount);
            }
            UpdateFreeze(moreFreeze + freezeAmount);

            PlayerController.instance.ProcessActiveItem(EntityType.ItemFreeze, null);
        }
    }
    #endregion

    public void ShowHideLeftRect(bool show)
    {
        leftRect.gameObject.SetActive(show);
    }

    public void OnClickReplayBtn()
    {
        IronSourceAd.instance.ShowInterstitialAd("InGameReplay", MapManager.instance.CurLevelNumber, actionSuccess, actionSuccess);

        void actionSuccess()
        {
            MapManager.instance.RestartCurrentMap();
        }       
    }

    public void OnClickSkipBtn()
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
            MapManager.instance.SkipCurrentMap();
        }
        IronSourceAd.instance.ShowRewardedVideo("SkipCurrentMap", actionPrepare, actionSuccess, actionFailed);
    }

    public void ShowGameNotice(string _msg)
    {
        queue.Enqueue(_msg);

        if (gameNoticeCor != null) return;
        Debug.Log("queee:" + queue.Count + " " + (gameNoticeCor != null));

        gameNoticeCor = StartCoroutine(ShowGameNoticeIE());

        IEnumerator ShowGameNoticeIE()
        {
            var msg = queue.Dequeue();
            bool inRun = true;
            while (inRun)
            {
                if (string.IsNullOrEmpty(msg)) yield return new WaitForSeconds(0.2f);
                else
                {
                    var tmpMsg = msg;
                    msg = null;

                    var color = gameNotice.color;
                    var tmpColor = gameNotice.color;
                    tmpColor.a = 0f;
                    gameNotice.color = tmpColor;
                    var txt = gameNotice.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
                    txt.gameObject.SetActive(false);

                    var txtColor = txt.color;
                    txtColor.a = 1;
                    txt.color = txtColor;

                    gameNotice.DOColor(color, 0.5f).OnComplete(() =>
                    {
                        txt.gameObject.SetActive(true);
                        txt.text = tmpMsg;
                        StartCoroutine(WaittingIE());
                    });
                    gameNotice.gameObject.SetActive(true);

                    IEnumerator WaittingIE()
                    {
                        yield return new WaitForSeconds(1f);

                        txt.DOFade(0.5f, 0.2f).OnComplete(() =>
                        {
                            gameNotice.gameObject.SetActive(false);
                            if (queue.Count > 0) msg = queue.Dequeue();
                            else
                            {
                                inRun = false;
                                gameNoticeCor = null;
                            }
                        });
                    }
                }
            }
            Debug.Log("Enddddd");
        }
    }
}
