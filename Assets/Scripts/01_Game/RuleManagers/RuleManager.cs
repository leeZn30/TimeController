using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    [SerializeField] string ruleID;
    protected bool isClear = false;

    private void Awake()
    {
        ClearData ClearData = GameData.ClearDatas.Find(e => e.ID == ruleID);
        if (ClearData == null)
        {
            GameData.ClearDatas.Add(new ClearData(ruleID, false));
        }
        else { isClear = ClearData.IsClear; }
    }


    public virtual void Clear()
    {
        if (!isClear)
        {
            isClear = true;
            GameData.ClearDatas.Find(e => e.ID == ruleID).IsClear = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(AudioType.Puzzle, "Clear");
        }
    }
}
