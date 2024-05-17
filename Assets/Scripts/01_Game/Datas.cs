using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

[System.Serializable]
public class ItemData
{
    string id;
    public string ID { get { return id; } set { id = value; } }

    bool isGain;
    public bool IsGain { get { return isGain; } set { isGain = value; } }

    public ItemData(string id, bool isGain)
    {
        ID = id;
        IsGain = isGain;
    }
}

[System.Serializable]
public class ObjectData
{
    string id;
    public string ID { get { return id; } set { id = value; } }

    bool isExist;
    public bool IsExist { get { return isExist; } set { isExist = value; } }

    public ObjectData(string id, bool isExist)
    {
        ID = id;
        IsExist = isExist;
    }
}

[System.Serializable]
public class ClearData
{
    string id;
    public string ID { get { return id; } set { id = value; } }

    bool isClear;
    public bool IsClear { get { return isClear; } set { isClear = value; } }

    public ClearData(string id, bool isClear)
    {
        ID = id;
        IsClear = isClear;
    }
}

public class SceneHandOverData
{
    public string door = "";
    public int nowGhosts = 0;
    public float nowTeleportGauge = 0;
    public float nowSlowGauge = 0;

    public SceneHandOverData(int nowGhosts, float nowTeleportGauge, float nowSlowGauge, string door)
    {
        this.door = door;
        this.nowGhosts = nowGhosts;
        this.nowTeleportGauge = nowTeleportGauge;
        this.nowSlowGauge = nowSlowGauge;
    }

    public SceneHandOverData() { }
}

