using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rule0Manager : RuleManager
{
    [SerializeField] Key key;

    public override void Clear()
    {
        if (!isClear)
        {
            isClear = true;
            GameData.ClearDatas.Find(e => e.ID == ruleID).IsClear = true;

            if (SoundManager.Instance != null)
                SoundManager.Instance.PlaySFX(AudioType.Puzzle, "Clear");

            GiveKey();
        }
    }

    public void GiveKey()
    {
        key.transform.position = new Vector2(7.5f, 6.5f);
        StartCoroutine(movekey());
    }

    IEnumerator movekey()
    {
        while (key.transform.position.y > -1f)
        {
            key.transform.position += Vector3.down * 15f * Time.deltaTime;
            yield return null;
        }
    }
}
