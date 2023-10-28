using Assets.Scripts.Define;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public PlayerType playerType;
    public int coinPrice;
    public int gemPrice;
    public int adAmount;

    public float speed = 3f;
    public int life = 3;
    public PlayerRarity playerRarity;

    public bool hasTrial;
}

public class PlayerSaver
{
    public PlayerType playerType;

    public int speedUpPercent;
    public int lifeUpPercent;
    public bool usedTrial;

    public void Copy(PlayerSaver playerSaver)
    {
        this.speedUpPercent = playerSaver.speedUpPercent;
        this.lifeUpPercent = playerSaver.lifeUpPercent;
    }
}

