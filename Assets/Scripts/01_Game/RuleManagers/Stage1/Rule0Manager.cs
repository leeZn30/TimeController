using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule0Manager : Singleton<Rule0Manager>
{
    [SerializeField] Key key;

    public void GiveKey()
    {
        key.transform.position = new Vector2(7.5f, 6.5f);
        StartCoroutine(movekey());
    }

    IEnumerator movekey()
    {
        while (key.transform.position.y > -1.5f)
        {
            key.transform.position += Vector3.down * 15f * Time.deltaTime;
            yield return null;
        }
    }
}
