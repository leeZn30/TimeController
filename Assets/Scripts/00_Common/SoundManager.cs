using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AudioType
{
    Character, Crank, Door, BossDoor, Puzzle, Clock, Enemy
}

public class SoundManager : Singleton<SoundManager>
{
    [Header("AudioSources")]
    [SerializeField] AudioSource CharcterSC;
    [SerializeField] AudioSource ObjectSC;
    [SerializeField] AudioSource ETCSC;
    [SerializeField] AudioSource BGMSC;

    [Header("Character")]
    [SerializeField] List<AudioClip> Character = new List<AudioClip>();

    [Header("Object")]
    [SerializeField] List<AudioClip> Crank = new List<AudioClip>();
    [SerializeField] List<AudioClip> Door = new List<AudioClip>();
    [SerializeField] List<AudioClip> BossDoor = new List<AudioClip>();

    [Header("Puzzle")]
    public List<AudioClip> Puzzle = new List<AudioClip>();

    [Header("Clock")]
    public List<AudioClip> Clock = new List<AudioClip>();

    [Header("BGMs")]
    public List<AudioClip> ETCBGM = new List<AudioClip>();
    public List<AudioClip> Stage0 = new List<AudioClip>();
    public List<AudioClip> Stage1 = new List<AudioClip>();

    Coroutine BGMPitchCoroutine;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        CharcterSC = transform.GetChild(0).GetComponent<AudioSource>();
        ObjectSC = transform.GetChild(1).GetComponent<AudioSource>();
        ETCSC = transform.GetChild(2).GetComponent<AudioSource>();
        BGMSC = transform.GetChild(3).GetComponent<AudioSource>();
    }

    public void PlayBGM(string bgm)
    {
        List<AudioClip> stageList;
        switch (GameData.Stage)
        {
            case 0:
                stageList = Stage0;
                break;

            case 1:
                stageList = Stage1;
                break;

            default:
                stageList = ETCBGM;
                break;
        }

        AudioClip clip = stageList.Find(e => e.name == bgm);

        if (BGMSC.clip == null)
        {
            BGMSC.clip = clip;
            BGMSC.Play();
        }
        else if (clip != BGMSC.clip)
        {
            StartCoroutine(FadeBGM(clip));
        }
    }

    IEnumerator FadeBGM(AudioClip clip)
    {
        float duration = 0.5f;
        float timer = 0.0f;

        // 페이딩 진행
        while (timer < duration)
        {
            // 각 소스의 볼륨 조절
            BGMSC.volume = Mathf.Lerp(1.0f, 0.0f, timer / duration);

            // 시간 증가
            timer += Time.deltaTime;

            // 다음 프레임까지 대기
            yield return null;
        }
        BGMSC.Stop();

        BGMSC.clip = clip;
        BGMSC.volume = 1.0f;

        // 기존 오디오 소스 정지
        BGMSC.Play();
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

            case AudioType.BossDoor:
                ObjectSC.PlayOneShot(BossDoor.Find(e => e.name == sound));
                break;

            case AudioType.Puzzle:
                ETCSC.PlayOneShot(Puzzle.Find(e => e.name == sound));
                break;

            case AudioType.Clock:
                ETCSC.PlayOneShot(Clock.Find(e => e.name == sound));
                break;

            default:
                break;
        }
    }


    public void PlaySFX(AudioType type, AudioClip clip)
    {
        switch (type)
        {
            case AudioType.Character:
                CharcterSC.PlayOneShot(clip);
                break;

            case AudioType.Crank:
                ObjectSC.PlayOneShot(clip);
                break;

            case AudioType.Door:
                ObjectSC.PlayOneShot(clip);
                break;

            case AudioType.BossDoor:
                ObjectSC.PlayOneShot(clip);
                break;

            case AudioType.Puzzle:
                ETCSC.PlayOneShot(clip);
                break;

            case AudioType.Clock:
                ETCSC.PlayOneShot(clip);
                break;

            case AudioType.Enemy:
                ObjectSC.PlayOneShot(clip);
                break;


            default:
                break;
        }
    }

    public void AdjucstBGMPitch(float targetPitch = 1f, float duration = 0.5f)
    {
        if (BGMPitchCoroutine != null)
            StopCoroutine(BGMPitchCoroutine);

        BGMPitchCoroutine = StartCoroutine(BGMPitching(targetPitch, duration));
    }

    IEnumerator BGMPitching(float targetPitch, float duration = 0.5f)
    {
        float currentTime = 0f;
        while (currentTime < duration)
        {
            BGMSC.pitch = Mathf.Lerp(BGMSC.pitch, targetPitch, currentTime / duration);
            currentTime += Time.unscaledDeltaTime;

            yield return null;
        }
    }


}
