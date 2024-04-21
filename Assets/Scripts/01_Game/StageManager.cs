using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : Singleton<StageManager>
{
    public int StageId;
    int currentCheckPoint;
    List<CheckPoint> CheckPoints = new List<CheckPoint>();

    private void Awake()
    {
        GameData.Stage = StageId;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!GameData.Door.Equals(""))
        {
            Character.Instance.transform.position = GameObject.Find(GameData.Door).transform.position;
            GameData.Door = "";
        }
        else if (GameData.CheckPoint > -1)
        {
            CheckPoints = FindObjectsOfType<CheckPoint>().ToList();
            Character.Instance.transform.position = CheckPoints.Find(e => e.CheckIdx == GameData.CheckPoint).transform.position;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }



    public static void SaveStage(int CheckIdx)
    {
        if (CheckIdx >= GameData.CheckPoint)
            GameData.CheckPoint = CheckIdx;
        GameData.Ghosts = GhostManager.Instance.ghostCount;
        GameData.ReviveScene = SceneManager.GetActiveScene().buildIndex;
    }

}
