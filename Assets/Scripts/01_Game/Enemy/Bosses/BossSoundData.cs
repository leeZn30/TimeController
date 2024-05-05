using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSoundData : Singleton<BossSoundData>
{
    [Header("Sounds")]
    [SerializeField] List<AudioClip> sounds = new List<AudioClip>();

    public AudioClip GetSound(string sound)
    {
        return sounds.Find(x => x.name == sound);
    }
}
