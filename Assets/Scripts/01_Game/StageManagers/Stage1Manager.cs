using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage1Manager : StageManager
{
    protected override void Awake()
    {
        base.Awake();

        string stageId = "Stage" + StageId;
        ClearData data = GameData.ClearDatas.Find(e => e.ID == stageId);
        if (data != null && data.IsClear)
        {
            PostClear();
        }
    }

    public override void PostClear()
    {
        Debug.Log("Post Clear");
        // Background 바꾸기

        // FlowerWall 바꾸기
        // FlowerObject 바꾸기
        // FlowerMonster 바꾸기
    }
}
