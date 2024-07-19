using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEvent : MonoBehaviour
{
    public event Action callback;

    public void callMove()
    {
        StartCoroutine(MoveSequence());
    }

    IEnumerator MoveSequence()
    {
        yield return StartCoroutine(Move());

        callback.Invoke();
    }

    protected virtual IEnumerator Move()
    {
        yield return null;
    }
}
