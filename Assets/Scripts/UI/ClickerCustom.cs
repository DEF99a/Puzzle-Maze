using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClickerCustom : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SoundName soundName;
    public void OnPointerClick(PointerEventData eventData)
    {
        var btn = gameObject.GetComponent<Button>();
        if (btn == null || (btn != null && btn.IsActive() && btn.interactable))
            AudioManager.instance.PlaySound(soundName);
    }
}
