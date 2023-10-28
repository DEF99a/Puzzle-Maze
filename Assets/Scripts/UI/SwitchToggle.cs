using Assets.Scripts.Define;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public enum SwitchToggleType
{
    None,
    Sound,
    Music,
    Controller
}

public class SwitchToggle : MonoBehaviour
{
    static Dictionary<SwitchToggleType, string> switchToggleTypeString = new Dictionary<SwitchToggleType, string>()
    {
        {SwitchToggleType.Music, StringDefine.Music }, {SwitchToggleType.Sound, StringDefine.Sound },
        {SwitchToggleType.Controller, StringDefine.IsJoystick },
    };

    [SerializeField] private SwitchToggleType switchToggleType;
    [SerializeField] private RectTransform uiHandleRectTransfrom;
    [SerializeField] private Sprite backgroundOnImage;
    [SerializeField] private Sprite backgroundOffImage;

    private Image backgroundImage;
    private Toggle toggle;

    private Vector2 handlePosition;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(OnSwitch);
        backgroundImage = uiHandleRectTransfrom.parent.GetComponent<Image>();
        handlePosition = uiHandleRectTransfrom.anchoredPosition;
    }

    private void OnEnable()
    {
        var savedValue = ES3.Load(switchToggleTypeString[switchToggleType], 1);
        if (switchToggleType == SwitchToggleType.Controller)
            savedValue = ES3.Load(switchToggleTypeString[switchToggleType], 0);

        Debug.Log("switchToggleTypeString[switchToggleType]:" + switchToggleTypeString[switchToggleType] + " savedValue:" + savedValue);

        toggle.isOn = (savedValue == 1 ? true : false);

        uiHandleRectTransfrom.anchoredPosition = (toggle.isOn ? (handlePosition * -1) : handlePosition);
        backgroundImage.sprite = toggle.isOn ? backgroundOnImage : backgroundOffImage;
        if (switchToggleType == SwitchToggleType.Controller)
        {
            backgroundImage.sprite = backgroundOnImage;
        }
    }

    private void OnSwitch(bool on)
    {
        AudioManager.instance.PlaySound(SoundName.click_btn_common);

        uiHandleRectTransfrom.anchoredPosition = (toggle.isOn ? (handlePosition * -1) : handlePosition);

        Debug.Log("OnSwitch:" + switchToggleTypeString[switchToggleType]);

        ES3.Save(switchToggleTypeString[switchToggleType], on ? 1 : 0);

        Debug.Log("OnSwitch 2:" + ES3.Load(switchToggleTypeString[switchToggleType], 0));

        backgroundImage.sprite = on ? backgroundOnImage : backgroundOffImage;

        if (switchToggleType == SwitchToggleType.Music)
        {
            if (!on)
            {
                if (MenuUIManager.instance != null)
                    AudioManager.instance.StopMusic(SoundName.ground_music);
                else if(MapManager.instance != null) AudioManager.instance.StopMusic(MapManager.instance.GameMusicStr);
            }
            else
            {
                if (MenuUIManager.instance != null)
                    AudioManager.instance.PlayMusic(SoundName.ground_music);
                else if (MapManager.instance != null) AudioManager.instance.PlayMusic(MapManager.instance.GameMusicStr);
            }
        }
        else if (switchToggleType == SwitchToggleType.Controller)
        {
            backgroundImage.sprite = backgroundOnImage;
            this.PostEvent(EventID.UpdateController, null);
        }
    }

    private void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
