using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{
    public static void LoadScene(int sceneNum)
    {
        SceneManager.sceneLoaded += StageManager.Instance.OnSceneLoaded;

        SceneManager.LoadScene(sceneNum);
        Background.Instance.resetMaterial();
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Background.Instance.resetMaterial();

        SceneManager.sceneLoaded += StageManager.Instance.OnSceneLoaded;
    }

    public static void LoadSceneByDoor(string sceneName, string dName)
    {
        SceneManager.sceneLoaded += StageManager.Instance.OnSceneLoaded;

        GameData.Door = dName;

        SceneManager.LoadScene(sceneName);
        Background.Instance.resetMaterial();
    }

}
