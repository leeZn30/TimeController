using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class StageManager : Singleton<StageManager>
{
    public int StageId;
    [SerializeField] string BGM;
    List<CheckPoint> CheckPoints = new List<CheckPoint>();
    public static List<ItemData> tempItemDatas;

    protected virtual void Awake()
    {
        GameData.Stage = StageId;
        tempItemDatas = new List<ItemData>();
    }

    private void Start()
    {
        // Awake에 하면 Soundmanager에서 먼저 처리 못함
        // Boss가 있는 경우는 거기서 처리
        if (BossManager.Instance == null || BossManager.Instance.isClear)
            SoundManager.Instance.PlayBGM(BGM);
    }

    public void Init()
    {
        if (GameData.CheckPoint > -1)
        {
            CheckPoints = FindObjectsOfType<CheckPoint>().ToList();
            Character.Instance.transform.position = CheckPoints.Find(e => e.CheckIdx == GameData.CheckPoint).transform.position;
        }
    }

    // public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     if (!GameData.Door.Equals(""))
    //     {
    //         Character.Instance.transform.position = GameObject.Find(GameData.Door).transform.position;
    //         GameData.Door = "";
    //     }
    //     else if (GameData.CheckPoint > -1)
    //     {
    //         CheckPoints = FindObjectsOfType<CheckPoint>().ToList();
    //         Character.Instance.transform.position = CheckPoints.Find(e => e.CheckIdx == GameData.CheckPoint).transform.position;
    //     }

    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    public static void SaveStage(int CheckIdx)
    {
        if (CheckIdx >= GameData.CheckPoint)
            GameData.CheckPoint = CheckIdx;
        GameData.Ghosts = GhostManager.Instance.ghostCount;
        GameData.ReviveScene = SceneManager.GetActiveScene().buildIndex;
    }

    public abstract void PostClear();

}
