using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    [SerializeField] GameObject bubblePfb;

    GameObject bubble;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            bubble = Instantiate(bubblePfb, new Vector3(transform.position.x,
                                                        transform.position.y + 3,
                                                        transform.position.z),
                                                        Quaternion.identity,
                                                        FindObjectOfType<Canvas>().transform);
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
