using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    static Stack<GameObject> ESCStack = new Stack<GameObject>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            popESC();
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
}
