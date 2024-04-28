using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    [SerializeField] int ruleID;
    protected bool isClear = false;

    private void Awake()
    {
        RuleData ruleData = GameData.RuleDatas.Find(e => e.ID == ruleID);
        if (ruleData == null)
        {
            GameData.RuleDatas.Add(new RuleData(ruleID, false));
        }
        else { isClear = ruleData.IsClear; }
    }


    public virtual void Clear()
    {
        if (!isClear)
        {
            isClear = true;
            GameData.RuleDatas.Find(e => e.ID == ruleID).IsClear = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(AudioType.Puzzle, "Clear");
        }
    }
}
