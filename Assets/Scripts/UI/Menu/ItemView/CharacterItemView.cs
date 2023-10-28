using Assets.Scripts.Define;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterItemView : MonoBehaviour
{
    [SerializeField] private Image bgSelect;

    [SerializeField] private Image characterIcon;
    [SerializeField] private Image mask;
    //[SerializeField] private Image lockIcon;
    [SerializeField] private Image pickIcon;

    PlayerType curPt;

    private void Awake()
    {
        this.RegisterListener(EventID.SelectCharInList, (pt) => Select(pt));
    }

    public void Setup(PlayerData playerData, PlayerSaver playerSaver, bool current)
    {
        pickIcon.gameObject.SetActive(current);
        if (playerData != null)
        {
            curPt = playerData.playerType;

            mask.gameObject.SetActive(playerSaver == null);
            //lockIcon.gameObject.SetActive(playerSaver == null);
        }
        else
        {
            mask.gameObject.SetActive(playerSaver == null);
            //lockIcon.gameObject.SetActive(playerSaver == null);
        }
        characterIcon.sprite = EntitySprites.instance.GetPlayerSprite(playerData.playerType);
        bgSelect.gameObject.SetActive(false);
    }

    public void Select(object sel)
    {
        var pt = (PlayerType)sel;

        bgSelect.gameObject.SetActive(curPt == pt);
    }
}
