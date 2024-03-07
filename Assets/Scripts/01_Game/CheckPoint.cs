using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int CheckIdx;

    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (GameData.CheckPoint >= CheckIdx)
            anim.SetBool("Flutter", true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player") && !anim.GetBool("isChecked"))
        {
            anim.SetBool("isChecked", true);

            if (CheckIdx >= GameData.CheckPoint)
                GameData.CheckPoint = CheckIdx;
        }
    }
}
