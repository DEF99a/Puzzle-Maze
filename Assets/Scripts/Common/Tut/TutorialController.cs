using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.SceneManagement;
using Spine.Unity;
using Assets.Scripts.Common;

public class TutorialController : TutBase
{
    public static TutorialController Instance;
    [SerializeField] private SkeletonAnimation lightTut;

    [SerializeField] private RectTransform keyPadTf;
    [SerializeField] private Transform kickBtnTf;
    [SerializeField] private RectTransform processEnemiesRect;
    [SerializeField] private Transform arrowTutTf;

    public Transform firstMoveTargetTut1;
    public Transform moveTarget2Tut1;
    public Transform moveTarget3Tut1;
    public Transform moveTarget4Tut1;

    private bool useFireBtn1Tut1 = false;
    private bool useFireBtn2Tut1 = false;
    private bool wentToGate = false;

    private void Awake()
    {
#if !UNITY_EDITOR
        Instance = this;
#endif
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();
    }

    public void Setup()
    {
        if (MapManager.instance.CurLevelNumber != Config.FirstLevel) return;
        StartCoroutine(UseKeyPadTutIE());
    }

    private void ShowLightTut(Vector3 position)
    {
        lightTut.transform.position = position;
        lightTut.gameObject.SetActive(true);
        arrowTutTf.transform.position = position;
        arrowTutTf.gameObject.SetActive(true);
    }

    private void HideLightTut()
    {
        if (lightTut.gameObject.activeSelf)
        {
            lightTut.gameObject.SetActive(false);
            arrowTutTf.gameObject.SetActive(false);
        }
    }

    public void ShowTutMove(Vector3Int playerCell)
    {
        if (MapManager.instance.CurLevelNumber != Config.FirstLevel) return;

        var moveTarget1Tut1 = MyGraph.instance.GetCellFromPosition(firstMoveTargetTut1.position, out _);
        var _moveTarget2Tut1 = MyGraph.instance.GetCellFromPosition(moveTarget2Tut1.position, out _);
        var _moveTarget4Tut1 = MyGraph.instance.GetCellFromPosition(moveTarget4Tut1.position, out _);

        if (playerCell == moveTarget1Tut1)
        {
            UseFireBtn1Tut();
        }
        else
        {
            if (playerCell == _moveTarget2Tut1)
            {
                UseFireBtn2Tut();
            }
            else if (playerCell == _moveTarget4Tut1)
            {
                //if (wentToGate) return;
                //wentToGate = true;
                ResetAllEffect();
                HideLightTut();
            }
        }
    }

    public void ShowTutKick(Vector3Int playerCell)
    {
        if (MapManager.instance.CurLevelNumber != Config.FirstLevel) return;

        var moveTarget1Tut1 = MyGraph.instance.GetCellFromPosition(firstMoveTargetTut1.position, out _);
        var _moveTarget3Tut1 = MyGraph.instance.GetCellFromPosition(moveTarget3Tut1.position, out _);

        if (playerCell == moveTarget1Tut1)
        {
            GotoTarget2Tut1();
        }
        else if (playerCell == _moveTarget3Tut1)
        {
            UseBombToBreakGift();
        }
    }

    IEnumerator UseKeyPadTutIE()
    {
        yield return new WaitForSeconds(0.5f);

        if (MapManager.instance.CurLevelNumber == 1)
        {
            ShowOnlyHand(keyPadTf, 135, false, 1.5f);
            ShowText("Please use keypad to move player", () =>
            {
                ShowLightTut(firstMoveTargetTut1.position);
                //ShowOnlyHand(firstMoveTargetTut1, 0, true);
                HideHand();

                HideText();
            });
        }
    }

    private void UseFireBtn1Tut()
    {
        if (useFireBtn1Tut1) return;
        useFireBtn1Tut1 = true;

        ShowOnlyHand(kickBtnTf.position + new Vector3(0, 0.5f), 135);
        HideLightTut();
        ShowText("Please use fire button to kick the box", () =>
        {
            HideHand();
            HideText();
        });
    }

    private void UseFireBtn2Tut()
    {
        if (useFireBtn2Tut1) return;
        useFireBtn2Tut1 = true;
        StartCoroutine(UseFireBtn2TutIE());
        ShowText("Please use fire button to break the box", () =>
        {
            HideHand();
            HideText();
        });
        IEnumerator UseFireBtn2TutIE()
        {
            yield return new WaitForSeconds(0.5f);

            ShowOnlyHand(kickBtnTf.position + new Vector3(0, 0.5f), 135);
            HideLightTut();
        }
    }

    private void GotoTarget2Tut1()
    {
        ShowLightTut(moveTarget2Tut1.position);
        //ShowOnlyHand(moveTarget2Tut1, 0, true);
        ShowText("Please move player to the target", () =>
        {
            HideText();
        });
    }

    private void UseBombToBreakGift()
    {
        ShowText("Kick bomb to break gift, some special items will be showed", () =>
        {
            HideText();
            KillAllEnemiesToCompleteGuide();
        });
    }

    private void KillAllEnemiesToCompleteGuide()
    {
        ShowFocus(processEnemiesRect.position + new Vector3(0.9f, 0), -35, null);
        ShowText("Kill all enemies to complete the level", () =>
        {
            HideText();
            HideFocus();
        });
    }

    public void GotoGate()
    {
        if (MapManager.instance.CurLevelNumber != Config.FirstLevel) return;

        //ShowLightTut(moveTarget4Tut1.position);
        //ShowOnlyHand(moveTarget4Tut1, 0, true);
        ShowText("Move to gate to complete the level", () =>
        {
            HideText();
        });
    }
}
