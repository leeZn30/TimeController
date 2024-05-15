using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class Boss0Manager : BossManager
{
    PlayableDirector director;
    [SerializeField] TimelineAsset BossEndTimeline;
    [SerializeField] GameObject Sun;
    [SerializeField] GameObject Stones;

    protected override void Awake()
    {
        base.Awake();
        if (!isClear)
        {
            Stones.SetActive(false);
        }
        else
        {
            Stones.SetActive(true);
        }
        director = FindObjectOfType<PlayableDirector>();

        if (!isClear && GameData.BossTryCnt == 1)
            director.Play();
    }

    public override void Clear()
    {
        base.Clear();

        Stones.SetActive(true);
        StartCoroutine(SunGoUP());
    }

    IEnumerator SunGoUP()
    {
        yield return new WaitForSeconds(1f);

        Sun.transform.position = Boss.transform.position;
        Sun.gameObject.SetActive(true);

        director.playableAsset = BossEndTimeline;

        float duration = 0f;
        while (duration < 1.5f)
        {
            duration += Time.deltaTime;

            Sun.transform.position += Vector3.up * Time.deltaTime * 7.5f;
            yield return null;
        }

        director.Play();
    }
}
