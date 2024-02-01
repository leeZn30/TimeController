using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParringBullet pb;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate(pb, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnDamaged()
    {
        Debug.Log("Hurt!");
    }
}
