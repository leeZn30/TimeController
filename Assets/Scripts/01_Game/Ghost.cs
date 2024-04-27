using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Animator anim;
    Transform target;

    public int Score;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        target = FindObjectOfType<GhostManager>().transform;
    }

    private void Update()
    {
        if (transform.position != target.position)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.unscaledDeltaTime * 100f);
        }
        else
        {
            anim.SetBool("Collected", true);
        }
    }

    void DoAddGhost()
    {
        Debug.Log("Do addGhost");
        GhostManager.Instance.AddGhost(Score);
    }

    void DoDestroy()
    {
        Destroy(gameObject);
    }
}
