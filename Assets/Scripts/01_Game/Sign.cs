using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] GameObject bubblePfb;
    [SerializeField] Canvas canvas;
    Vector3 pose;

    GameObject bubble;

    private void Awake()
    {
        canvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            bubble = Instantiate(bubblePfb, transform.position + new Vector3(0, 3, 0), Quaternion.identity, canvas.transform);
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
