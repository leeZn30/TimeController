using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HeartManager : Singleton<HeartManager>
{
    [Header("하트 상태")]
    [SerializeField] List<Heart> hearts = new List<Heart>();

    [SerializeField] Heart heartPfb;

    private void Awake()
    {
        for (int i = 0; i < Character.Instance.Hp / 4; i++)
        {
            hearts.Add(Instantiate(heartPfb, transform));
        }
    }

    public void calculateHeart(float damage)
    {
        // int quotient = (int)damage / 4;
        // int remainder = (int)damage % 4;

        // hearts[hearts.Count - 1].ChangeHeart(remainder);
    }

}
