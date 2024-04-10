using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] float BubbleOffset;

    [SerializeField] GameObject bubblePfb;
    [SerializeField] Canvas canvas;

    Vector3 bubblePosition;
    Collider2D collider;
    GameObject bubble;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();

        canvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        bubblePosition = collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2 + bubblePfb.GetComponent<RectTransform>().rect.size.y / 2 + BubbleOffset, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            bubble = Instantiate(bubblePfb, bubblePosition, Quaternion.identity, canvas.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            if (bubble != null)
                Destroy(bubble);
        }
    }
}
