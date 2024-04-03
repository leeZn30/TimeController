using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportPointer : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    CircleCollider2D collider;
    Collider2D otherCollider;

    private float overlapArea = 0f;
    private float percentageOfA = 0f;

    float area;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        otherCollider = GameObject.Find("Ground").GetComponent<Collider2D>();

        area = collider.radius * collider.radius * Mathf.PI;
        // Debug.Log("Teleport Area: " + area);
    }

    private void Update()
    {
        ComputeOverlap();
    }

    void ComputeOverlap()
    {
        // 겹치는 부분 계산
        if (collider.IsTouching(otherCollider))
        {
            overlapArea = OverlapArea();

            // 겹치는 부분이 objectA의 전체 면적의 몇 %를 차지하는지 계산
            percentageOfA = (overlapArea / area) * 100f;
        }
        else
        {
            overlapArea = 0f;
            percentageOfA = 0f;
        }

        // Debug.Log("Overlap Area: " + overlapArea);
        // Debug.Log("Percentage of A: " + percentageOfA + "%");

        if (percentageOfA < 30)
        {
            spriteRenderer.color = Color.white;
        }
        else
        {
            spriteRenderer.color = Color.magenta;
        }
    }

    float OverlapArea()
    {
        Bounds boundsA = collider.bounds;
        Bounds boundsB = otherCollider.bounds;

        // Debug.LogFormat("x: min {0} max {1}\n y: min {2} max {3}", boundsB.min.x, boundsB.max.x, boundsB.min.y, boundsB.max.y);

        float minX = Mathf.Max(boundsA.min.x, boundsB.min.x);
        float minY = Mathf.Max(boundsA.min.y, boundsB.min.y);
        float maxX = Mathf.Min(boundsA.max.x, boundsB.max.x);
        float maxY = Mathf.Min(boundsA.max.y, boundsB.max.y);

        // 겹치는 영역 계산
        float width = Mathf.Max(0, maxX - minX);
        float height = Mathf.Max(0, maxY - minY);

        return width * height;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Equals("Ground"))
        {
            ComputeOverlap();
        }
    }

}


