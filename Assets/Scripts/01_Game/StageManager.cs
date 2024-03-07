using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StageManager : Singleton<StageManager>
{
    public int StageId;
    int currentCheckPoint;
    List<CheckPoint> CheckPoints = new List<CheckPoint>();

    private void Awake()
    {
        GameData.Stage = StageId;
        CheckPoints = FindObjectsOfType<CheckPoint>().ToList();

        // 체크포인트로 로드
        if (GameData.CheckPoint > -1)
        {
            Character.Instance.transform.position = CheckPoints.Find(e => e.CheckIdx == GameData.CheckPoint).transform.position;
        }
    }
}
