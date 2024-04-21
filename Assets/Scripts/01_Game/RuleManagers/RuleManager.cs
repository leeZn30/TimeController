using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleManager : Singleton<RuleManager>
{
    public virtual void Clear()
    {
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlaySFX(AudioType.Puzzle, "Clear");
    }
}
