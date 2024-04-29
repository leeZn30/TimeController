using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : Singleton<BossManager>
{
    BossDoor bossDoor;
    NextStage nextStage;
    SkillBox skillBox;

    private void Awake()
    {
        bossDoor = FindObjectOfType<BossDoor>();
        nextStage = FindObjectOfType<NextStage>();
        skillBox = FindObjectOfType<SkillBox>();

        skillBox.gameObject.SetActive(false);
        nextStage.gameObject.SetActive(false);
    }

    public void BossStart()
    {

    }

    public void Clear()
    {
        bossDoor.Open();
        nextStage.gameObject.SetActive(true);
    }
}