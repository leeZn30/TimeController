using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    Character, Crank, Door, Puzzle
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("AudioSources")]
    [SerializeField] AudioSource CharcterSC;
    [SerializeField] AudioSource ObjectSC;
    [SerializeField] AudioSource ETCSC;

    [Header("Character")]
    [SerializeField] List<AudioClip> Character = new List<AudioClip>();


    [Header("Object")]
    [SerializeField] List<AudioClip> Crank = new List<AudioClip>();
    [SerializeField] List<AudioClip> Door = new List<AudioClip>();

    [Header("Puzzle")]
    public List<AudioClip> Puzzle = new List<AudioClip>();

    private void Awake()
    {
        DontDestroyOnLoad(this);

        CharcterSC = transform.GetChild(0).GetComponent<AudioSource>();
        ObjectSC = transform.GetChild(1).GetComponent<AudioSource>();
        ETCSC = transform.GetChild(2).GetComponent<AudioSource>();
    }

    public void PlaySFX(AudioType type, string sound)
    {
        switch (type)
        {
            case AudioType.Character:
                CharcterSC.PlayOneShot(Character.Find(e => e.name == sound));
                break;

            case AudioType.Crank:
                ObjectSC.PlayOneShot(Crank.Find(e => e.name == sound));
                break;

            case AudioType.Door:
                ObjectSC.PlayOneShot(Door.Find(e => e.name == sound));
                break;

            case AudioType.Puzzle:
                ETCSC.PlayOneShot(Puzzle.Find(e => e.name == sound));
                break;

            default:
                break;
        }
    }

}
