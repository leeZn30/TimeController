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

    protected override void Awake()
    {
        base.Awake();
        director = FindObjectOfType<PlayableDirector>();

    }

    public override void Clear()
    {
        base.Clear();

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
