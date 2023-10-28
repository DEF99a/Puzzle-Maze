using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CoinFlyUp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI txt;
    [SerializeField] private RectTransform canvas;

    public void Setup(int coin)
    {
        txt.text = string.Format("+{0}", coin);

        transform.DOMoveY(transform.position.y + 2, 1f).OnComplete(() =>
        {
            var colo = txt.color;
            colo.a = 0;
            txt.DOColor(colo, 0.5f).OnComplete(() =>
            {               
                ObjectPool.instance.Disable(gameObject);
                colo.a = 1f;
                txt.color = colo;
            });
        });
    }    
}
