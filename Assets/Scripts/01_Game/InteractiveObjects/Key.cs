using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] Door PairDoor;
    Animator anim;

    protected void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            anim.SetTrigger("Collected");
            PairDoor.Open();
        }
    }

    void DoDestroy()
    {
        Destroy(gameObject);
    }

}
