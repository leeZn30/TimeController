using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : Singleton<BossManager>
{
    BossDoor bossDoor;
    NextStage nextStage;

    private void Awake()
    {
        bossDoor = FindObjectOfType<BossDoor>();
        nextStage = FindObjectOfType<NextStage>();

        nextStage.gameObject.SetActive(false);
    }

    public void Clear()
    {
        bossDoor.Open();
        nextStage.gameObject.SetActive(true);
    }
}
