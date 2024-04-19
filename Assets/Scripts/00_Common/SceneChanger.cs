using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : Singleton<SceneChanger>
{
    static Vector3 lastPlayerPosition;
    public static Vector3 LastPlayerPosition
    {
        get { return lastPlayerPosition; }
        set { LastPlayerPosition = value; }
    }

    public static void LoadScene(int sceneNum)
    {
        // if (Character.Instance != null)
        //     LastPlayerPosition = Character.Instance.transform.position;

        SceneManager.LoadScene(sceneNum);
        Background.Instance.resetMaterial();
    }
    public static void LoadScene(string sceneName)
    {
        // if (Character.Instance != null)
        //     LastPlayerPosition = Character.Instance.transform.position;

        SceneManager.LoadScene(sceneName);
        Background.Instance.resetMaterial();
    }
}
