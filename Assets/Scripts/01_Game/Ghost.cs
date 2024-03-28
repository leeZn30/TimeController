using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    Transform target;

    public int Score;

    private void Awake()
    {
        target = FindObjectOfType<GhostManager>().transform;
    }

    private void Update()
    {
        if (transform.position != target.position)
            transform.position = Vector3.MoveTowards(transform.position, target.position, Time.unscaledDeltaTime * 100f);
        else
        {
            GhostManager.Instance.AddGhost(Score);
            Destroy(gameObject);
        }
    }
}
