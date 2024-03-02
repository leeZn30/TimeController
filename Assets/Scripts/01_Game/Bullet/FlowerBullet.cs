using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBullet : Bullet
{
    [SerializeField] float power;
    Rigidbody2D rigid;

    protected override void Awake()
    {
        base.Awake();

        rigid = GetComponent<Rigidbody2D>();
    }

    protected override void Update()
    {
    }

    private void Start()
    {
        move();
    }

    protected override void move()
    {
        rigid.AddForce(new Vector2(Random.Range(-0.3f, 0.3f), 1) * power, ForceMode2D.Impulse);
    }
}
