using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpenClose : MonoBehaviour
{
    static DoorOpenClose _instance;

    public static DoorOpenClose Instance
    {
        get
        {
            if (_instance == null)
            {
                var go = new GameObject("DoorOpenClose");
                _instance = go.AddComponent<DoorOpenClose>();
            }
            return _instance;
        }
    }


}
