using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QMG;
using System;

public static class FBInstantSaveData
{
    public static FBInstantSaveDataInfo Info;

    public static void LoadData(System.Action done)
    {
        ContextPlayer context = new ContextPlayer();
        Info = new FBInstantSaveDataInfo();
        Info.Level = 1;
        List<string> keys = new List<string>() { "Info_ID", "Info_LevelGame", "Info_CoinGame" };
        context.getDataAsync(keys, (data) =>
        {
            if (data.ContainsKey("Info_ID")) Info.ID = (Int64)data["Info_ID"];
            if (Info.ID == 0) Info.ID = Int64.Parse(context.getID());
            if (data.ContainsKey("Info_LevelGame"))
            {
                Debug.Log("Info_LevelGame " + (Int64)data["Info_LevelGame"]);
                Info.Level = (Int64)data["Info_LevelGame"];
            }
            if (data.ContainsKey("Info_CoinGame")) Info.Coin = (Int64)data["Info_CoinGame"];
            done?.Invoke();
        });
    }

    public static void SaveData()
    {
        ContextPlayer context = new ContextPlayer();
        Dictionary<string, object> data = new Dictionary<string, object>();
        if (Info == null) Info = new FBInstantSaveDataInfo();
        data.Add("Info_ID", Info.ID);
        data.Add("Info_LevelGame", Info.Level);
        data.Add("Info_CoinGame", Info.Coin);
        context.setDataAsync(data, () =>
        {
            Debug.LogError("Save Done");
        });
    }
}

[System.Serializable]
public class FBInstantSaveDataInfo
{
    public Int64 ID;
    public Int64 Level;
    public Int64 Coin;
}