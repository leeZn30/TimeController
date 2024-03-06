using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            anim.SetTrigger("Collected");

            HeartManager.Instance.RecoverHeart();
        }
    }

    void DoDestroy()
    {
        Destroy(gameObject);
    }
}
