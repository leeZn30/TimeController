using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] ParringBullet pb;
    [SerializeField] float Hp;

    // Start is called before the first frame update
    void Start()
    {
        ;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            Instantiate(pb, transform.position, Quaternion.identity);
    }

    public void OnDamaged()
    {
        Hp -= 10f;

        if (Hp <= -0)
        {
            Destroy(gameObject);
        }
    }


}
