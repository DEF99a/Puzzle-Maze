using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonInit : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    // Start is called before the first frame update
    void Start()
    {

        InitComponent();
    }

    void InitComponent()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
