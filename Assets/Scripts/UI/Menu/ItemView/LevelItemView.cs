using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Assets.Scripts.Common;
using UnityEngine.SceneManagement;

public class LevelItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelTxt;
    [SerializeField] private Image lockIcon;
    [SerializeField] private Image bg;
    [SerializeField] private Button btn;
    [SerializeField] private RectTransform stars;
    [SerializeField] private Sprite starUnlockSp;
    [SerializeField] private Sprite starLockSp;

    [SerializeField] private Sprite unlockSp;
    [SerializeField] private Sprite lockSp;

    private int level;
    public void Setup(int level, bool unlock, int starAmount)
    {
        this.level = level;

        levelTxt.text = level.ToString();
        levelTxt.gameObject.SetActive(unlock);
        stars.gameObject.SetActive(unlock);

        if (unlock)
        {
            lockIcon.gameObject.SetActive(false);
            bg.sprite = unlockSp;

            btn.enabled = true;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(Play);
            for(int k = 0; k < stars.childCount; k++)
            {
                var image = stars.GetChild(k).GetComponent<Image>();
                if (k < starAmount)
                {
                    image.sprite = starUnlockSp;
                }
                else image.sprite = starLockSp;
            }    
        }
        else
        {
            bg.sprite = lockSp;
            lockIcon.gameObject.SetActive(true);
            btn.enabled = false;
        }
    }

    private void Play()
    {
        AudioManager.instance.PlaySound(SoundName.click_btn_common);
        Config.SelectMap = this.level;
        IronSourceAd.instance.HideBanner();
        SceneManager.LoadScene("InGame");
    }
}
