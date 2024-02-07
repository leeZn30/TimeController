using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

    [SerializeField] GameObject ESCMenuPfb;

    GameObject ESCMenu;
    Canvas canvas;

    static Stack<GameObject> ESCStack = new Stack<GameObject>();

    private void Awake()
    {
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (ESCStack.Count > 0)
                popESC();
            else
            {
                // ESCMenu.SetActive(true);
                ESCMenu = Instantiate(ESCMenuPfb, canvas.transform);
                pushESC(ESCMenu);
            }
        }
    }

    void popESC()
    {
        if (ESCStack.Count > 0)
            Destroy(ESCStack.Pop());
    }

    static public void pushESC(GameObject go)
    {
        ESCStack.Push(go);
    }

    public void Retry()
    {

        popESC();
    }

    public void Exit()
    {

        popESC();
    }
}
