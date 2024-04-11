using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Rule3Manager : Singleton<Rule3Manager>
{
    bool isClear = false;

    [SerializeField] GameObject rock;
    [SerializeField] GameObject door;
    [SerializeField] GameObject key;
    [SerializeField] GameObject CCTV;

    private void Update()
    {
        if (CCTV == null && !isClear)
        {
            Clear();
        }
    }

    public void Clear()
    {
        isClear = true;
        Destroy(rock.gameObject);
        key.SetActive(true);
        StartCoroutine(movekey());
    }


    IEnumerator movekey()
    {
        while (key.transform.position.y > -5f)
        {
            key.transform.position += Vector3.down * 15f * Time.deltaTime;
            yield return null;
        }
    }
}
