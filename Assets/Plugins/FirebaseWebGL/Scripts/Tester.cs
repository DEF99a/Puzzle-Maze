using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FirebaseWebGL.Scripts.FirebaseAnalytics;
using FirebaseWebGL.Scripts.FirebaseBridge;

public class Tester : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Text txt;
    void Start()
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            FirebaseAnalytics.LogEvent("Tester");
            FirebaseDatabase.GetJSON("example", gameObject.name, "OnRequestSuccess", "OnRequestFall");
        });
    }

    private void OnRequestSuccess(string data)
    {
        txt.text = data;
    }

    private void OnRequestFall(string error)
    {
        txt.text = error;
    }
}
