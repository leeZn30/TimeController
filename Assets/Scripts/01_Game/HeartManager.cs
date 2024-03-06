using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HeartManager : Singleton<HeartManager>
{
    [Header("하트 상태")]
    [SerializeField] List<GameObject> hearts = new List<GameObject>();
    [SerializeField] List<int> heartHps = new List<int>();
    [SerializeField] List<Sprite> heartSprites = new List<Sprite>();

    [SerializeField] GameObject heartPfb;
    [SerializeField] int HeartCount => hearts.Count;
    [SerializeField] float MaxHeart;

    private void Awake()
    {
        MaxHeart = Character.Instance.Hp / 4;
        for (int i = 0; i < Character.Instance.Hp / 4; i++)
        {
            hearts.Add(Instantiate(heartPfb, transform));
            heartHps.Add(4);
        }
    }

    public void AddHeart()
    {
        RecoverHeart();
        hearts.Add(Instantiate(heartPfb, transform));
        heartHps.Add(4);
    }

    public void calculateHeart(float damage)
    {
        while (damage > 0)
        {
            heartHps[heartHps.Count - 1]--;
            if (heartHps[heartHps.Count - 1] == 0)
            {
                heartHps.RemoveAt(heartHps.Count - 1);
                Destroy(hearts[hearts.Count - 1].gameObject);
                hearts.RemoveAt(hearts.Count - 1);
            }
            else
            {
                hearts[hearts.Count - 1].GetComponent<Image>().sprite = heartSprites[heartHps[heartHps.Count - 1] - 1];
            }
            damage--;
        }
    }

    public void RecoverHeart()
    {
        if (heartHps[HeartCount - 1] < 4)
        {
            heartHps[HeartCount - 1] = 4;
            hearts[HeartCount - 1].GetComponent<Image>().sprite = heartSprites[3];
        }
        else
        {
            if (HeartCount < MaxHeart)
            {
                heartHps.Add(4);
                hearts.Add(Instantiate(heartPfb, transform));
            }
        }
    }

}
