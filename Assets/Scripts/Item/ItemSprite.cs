using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Text valueTxt;
    [SerializeField] private RectTransform canvas;

    public void Init(EntityType entityType)
    {
        canvas.gameObject.SetActive(false);
        string name = entityType.ToString();
        if (name.Contains("ItemTimer"))
        {
            spriteRenderer.sprite = EntitySprites.instance.GetSprite(EntityType.ItemTimer);
            valueTxt.text = name.Replace("ItemTimer", "");
            canvas.gameObject.SetActive(true);
        }
        else
        {
            spriteRenderer.sprite = EntitySprites.instance.GetSprite(entityType);
        }
    }
}
