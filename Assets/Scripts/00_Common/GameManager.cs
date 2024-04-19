using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] GameObject ESCMenu;

    static Stack<GameObject> ESCStack = new Stack<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCStack.Count > 0)
                popESC();
            else
            {
                OpenMenu();
            }
        }
    }

    void popESC()
    {
        if (Character.Instance != null)
            Character.Instance.isMovable = true;

        Time.timeScale = 1f;
        GameObject go = ESCStack.Pop();
        if (go != ESCMenu)
            Destroy(go);
        else
            ESCMenu.SetActive(false);
    }

    static public void pushESC(GameObject go)
    {
        ESCStack.Push(go);
    }

    public void OpenMenu()
    {
        if (Character.Instance != null)
            Character.Instance.isMovable = false;

        Time.timeScale = 0f;
        ESCMenu.SetActive(true);

        pushESC(ESCMenu);
    }

    public void Retry()
    {
        if (GameData.ReviveScene != -1)
            SceneChanger.LoadScene(GameData.ReviveScene);
        else
        {
            SceneChanger.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        GameData.nowGhosts = GameData.Ghosts;
        popESC();
    }

    public void Exit()
    {
        popESC();
    }

    public void Clear()
    {

    }

    public void Failed()
    {
        OpenMenu();
    }
}
