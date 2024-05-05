using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stage1Manager : StageManager
{
    [Header("Post Clear")]
    [SerializeField] RuntimeAnimatorController flowerAnim;
    [SerializeField] Sprite flowerWall;
    [SerializeField] List<Sprite> BGs;

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
        // Background 바꾸기
        Background background = FindObjectOfType<Background>();
        if (background.BackgroundCnt > 0)
        {
            background.ChangeBackground(BGs);
        }

        // FlowerWall 바꾸기
        FlowerWall[] fWalls = FindObjectsOfType<FlowerWall>();
        foreach (FlowerWall wall in fWalls)
        {
            int cnt = wall.transform.childCount;
            for (int i = 0; i < cnt; i++)
            {
                wall.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = flowerWall;
            }
        }

        // FlowerObject 바꾸기
        GameObject[] gos = FindObjectsOfType<GameObject>();
        foreach (GameObject go in gos)
        {
            if (go.name.Equals("FlowerObject1"))
            {
                go.GetComponent<Animator>().SetInteger("AnimState", 1);
            }

            if (go.name.Equals("FlowerObject2"))
            {
                go.GetComponent<Animator>().SetInteger("AnimState", 1);
            }

        }

        // FlowerMonster 바꾸기
        FlowerEnemy[] flowers = FindObjectsOfType<FlowerEnemy>();
        foreach (FlowerEnemy flower in flowers)
        {
            flower.GetComponent<Animator>().runtimeAnimatorController = flowerAnim;
        }

    }
}
