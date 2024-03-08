using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPointer : MonoBehaviour
{
    CircleCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        // CalculateOverlapArea();
    }

    public Vector2 CalculateOverlapArea(Collider2D other)
    {
        Bounds thisBounds = collider.bounds;
        Bounds otherBounds = other.bounds;

        if (!thisBounds.Intersects(otherBounds))
            return Vector2.zero;


        Debug.Log("InterSect!");

        Vector2 minThisBounds = (Vector2)thisBounds.min;
        Vector2 maxThisBounds = (Vector2)thisBounds.max;

        Vector2 minOtherBounds = (Vector2)otherBounds.min;
        Vector2 maxOtherBounds = (Vector2)otherBounds.max;

        Vector2 lowerMax = Vector2.Min(maxThisBounds, maxOtherBounds);
        Vector2 higherMin = Vector2.Max(minThisBounds, minOtherBounds);

        return lowerMax - higherMin;
    }


    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag.Equals("Ground"))
        {
            CalculateOverlapArea(other);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Ground"))
        {
            CalculateOverlapArea(other);
        }
    }
}


