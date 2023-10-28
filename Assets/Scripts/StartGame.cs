using Assets.Scripts.Common;
using Assets.Scripts.Controller;
using Assets.Scripts.Data;
using Assets.Scripts.Define;
using Assets.Scripts.Hellper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class StartGame : MonoBehaviour
{
    void Start()
    {
        var player = PlayerHelper.GetPlayers();
        if(player.Count == 0)
        {
            PlayerHelper.UpdatePlayer(new PlayerSaver() { playerType = Config.playerTypeDefault, lifeUpPercent = 1 });
        }    

        var curLevel = ES3.Load(StringDefine.CurrentLevel, Config.FirstLevel);
        Debug.Log("curLevel:" + curLevel);

        if (curLevel == 1)
        {
            SceneManager.LoadScene("InGame");
        }
        else SceneManager.LoadScene("Menu");
    }
}
