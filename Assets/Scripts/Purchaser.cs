
using Assets.Scripts.Data;
using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Assets.Scripts.Data.ShopData;

public class Purchaser : MonoBehaviour
{
    static Purchaser _instance;

    public static Purchaser Instance
    {
        get
        {
            if(_instance == null)
            {
                var go = new GameObject("Purchaser");
                _instance = go.AddComponent<Purchaser>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }    
    

}