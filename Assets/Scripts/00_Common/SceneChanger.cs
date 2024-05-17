using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : Singleton<SceneChanger>
{
    static SceneHandOverData sceneHandOverData;

    public static void MakeSceneHandOverData(string doorName = "", int ghostCount = -1)
    {
        sceneHandOverData = new SceneHandOverData
        (
            nowGhosts: ghostCount != -1 ? ghostCount : GhostManager.Instance != null ? GhostManager.Instance.ghostCount : 0,
            nowTeleportGauge: GameObject.Find("TeleportGauge") != null ? GameObject.Find("TeleportGauge").GetComponent<Slider>().value : 0,
            nowSlowGauge: GameObject.Find("SlowGauge") != null ? GameObject.Find("SlowGauge").GetComponent<Slider>().value : 0,
            door: doorName
        );
    }

    public static void LoadScene(int sceneNum)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(sceneNum);
        Background.Instance.resetMaterial();
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(sceneName);
        Background.Instance.resetMaterial();
    }

    public static void LoadSceneByDoor(string sceneName)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(sceneName);
        Background.Instance.resetMaterial();
    }


    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (StageManager.Instance != null)
        {
            StageManager.Instance.Init();
        }

        if (Character.Instance != null)
        {
            if (!sceneHandOverData.door.Equals(""))
            {
                Character.Instance.transform.position = GameObject.Find(sceneHandOverData.door).transform.position;
            }
        }

        if (GhostManager.Instance != null)
        {
            GhostManager.Instance.SetGhost(sceneHandOverData.nowGhosts);
        }

        if (GameData.TeleportActive)
        {
            GameObject.Find("TeleportGauge").GetComponent<Slider>().value = sceneHandOverData.nowTeleportGauge;
        }

        if (GameData.SlowActive)
        {
            GameObject.Find("SlowGauge").GetComponent<Slider>().value = sceneHandOverData.nowSlowGauge;
        }

        sceneHandOverData = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

}
