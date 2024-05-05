using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : Singleton<BossManager>
{
    public bool isClear = false;
    [SerializeField] string StageId;

    BossDoor bossDoor;
    NextStage nextStage;
    SkillBox skillBox;
    [SerializeField] protected Enemy Boss;

    protected virtual void Awake()
    {
        bossDoor = FindObjectOfType<BossDoor>();
        nextStage = FindObjectOfType<NextStage>();
        skillBox = FindObjectOfType<SkillBox>();

        skillBox.gameObject.SetActive(false);
        nextStage.gameObject.SetActive(false);

        ClearData data = GameData.ClearDatas.Find(e => e.ID == StageId);
        if (data == null)
        {
            GameData.ClearDatas.Add(new ClearData(StageId, false));
        }
        else
        {
            isClear = data.IsClear;
        }

        if (!isClear)
        {
            GameData.BossTryCnt++;
            SoundManager.Instance.PlayBGM("Boss0");
        }
        else
        {
            skillBox.gameObject.SetActive(true);
            nextStage.gameObject.SetActive(true);
        }
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
