using System.Collections;
using System.Collections.Generic;
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
