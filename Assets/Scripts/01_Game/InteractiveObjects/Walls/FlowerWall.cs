using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerWall : MonoBehaviour
{
    [SerializeField] Vector2 TargetPose;
    [SerializeField] float moveSpeed;
    bool isOpen = false;

    public void OpenWall()
    {
        if (!isOpen)
        {
            StartCoroutine(Move());
        }
    }

    IEnumerator Move()
    {
        while ((Vector2)transform.position != TargetPose)
        {
            transform.position = Vector3.MoveTowards(transform.position, TargetPose, moveSpeed * Time.deltaTime);
            yield return null;
        }

        GetComponent<Collider2D>().enabled = false;
    }
}
