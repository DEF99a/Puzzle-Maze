using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StringType
{
    None,
    StoneLock,
    WoodLock,
    GateCheckPoint,
    DoorClose,
    GroundKeyLock
}

[System.Serializable]
public class StringTypeName
{
    public StringType stringType;
    public string name;
}

public class StringTypes : MonoBehaviour
{
    public static StringTypes instance;

    public int h;
    public List<StringTypeName> stringTypeNames;

    public Dictionary<StringType, string> pair;

    private void Awake()
    {
        instance = this;

        pair = new Dictionary<StringType, string>();
        foreach (var stn in stringTypeNames)
            pair.Add(stn.stringType, stn.name);
    }
}
