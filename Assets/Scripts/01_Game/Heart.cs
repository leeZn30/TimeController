using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    [Header("하트 상태")]
    [SerializeField] int stat = 0;

    [SerializeField] Sprite[] HeartSprites = new Sprite[4];
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ChangeHeart(int remainder)
    {
        stat += remainder;
        image.sprite = HeartSprites[stat];

        if (stat > 4)
        {
        }
    }
}
