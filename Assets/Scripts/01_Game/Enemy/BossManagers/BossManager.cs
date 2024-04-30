using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : Singleton<BossManager>
{
    protected bool isClear = false;
    [SerializeField] string StageId;

    BossDoor bossDoor;
    NextStage nextStage;
    SkillBox skillBox;
    [SerializeField] protected Enemy Boss;

    protected virtual void Awake()
    {
        ClearData data = GameData.ClearDatas.Find(e => e.ID == StageId);
        if (data == null)
        {
            GameData.ClearDatas.Add(new ClearData(StageId, false));
        }
        else
        {
            isClear = data.IsClear;
        }

        bossDoor = FindObjectOfType<BossDoor>();
        nextStage = FindObjectOfType<NextStage>();
        skillBox = FindObjectOfType<SkillBox>();

        skillBox.gameObject.SetActive(false);
        nextStage.gameObject.SetActive(false);
    }

    public virtual void Clear()
    {
        ClearData data = GameData.ClearDatas.Find(e => e.ID == StageId);
        data.IsClear = true;

        bossDoor.Open();
        nextStage.gameObject.SetActive(true);
        skillBox.gameObject.SetActive(true);
    }
}
