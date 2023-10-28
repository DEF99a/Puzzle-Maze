using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreLineItemView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amountTxt;
    [SerializeField] private TextMeshProUGUI coinTxt;
    [SerializeField] private TextMeshProUGUI gameX2Txt;

    private int curCoin;
    private int curAmount, gameX2;
    private Coroutine runCo;
    private bool isGameX2;

    public void Setup(int amount, int coin, int gameX2, bool isGameX2)
    {
        gameObject.SetActive(true);
        curCoin = coin;
        curAmount = amount;
        this.gameX2 = gameX2;
        this.isGameX2 = isGameX2;

        runCo = StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        int stepCoin = curCoin / 5;
        int stepAmount = curAmount / 5;
        int stepGameX2 = gameX2 / 5;

        int coinTmp = 0;
        int amountTmp = 0;
        int gameX2Tmp = 0;

        coinTxt.text = coinTmp.ToString();
        amountTxt.text = amountTmp.ToString();
        if (this.isGameX2) gameX2Txt.text = gameX2Tmp.ToString();

        for (int k = 0; k < 5; k++)
        {
            yield return new WaitForSeconds(0.05f);

            coinTmp += stepCoin;
            amountTmp += stepAmount;
            gameX2Tmp += stepGameX2;

            coinTxt.text = coinTmp.ToString();
            amountTxt.text = amountTmp.ToString();
            if (this.isGameX2) gameX2Txt.text = gameX2Tmp.ToString();
        }
        coinTxt.text = curCoin.ToString();
        amountTxt.text = curAmount.ToString();
        if (this.isGameX2) gameX2Txt.text = gameX2.ToString();

        this.PostEvent(EventID.EndScoreLine, null);
    }

    public void Stop()
    {
        if (runCo != null) StopCoroutine(runCo);
        coinTxt.text = curCoin.ToString();
        amountTxt.text = curAmount.ToString();
        this.PostEvent(EventID.EndScoreLine, null);
    }
}
