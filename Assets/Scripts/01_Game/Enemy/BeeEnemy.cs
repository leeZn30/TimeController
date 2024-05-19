using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeeEnemy : Enemy
{

    protected override void Update()
    {
        detectPlayer();
    }

    private void FixedUpdate()
    {
        attack();
    }

    protected override void attack()
    {
        if (isPlayerFound)
        {
            // 목표 방향 계산
            Vector3 direction = (Character.Instance.transform.position - transform.position).normalized;

            // Rigidbody에 힘을 가해 목표 방향으로 이동
            rigid.MovePosition(rigid.position + (Vector2)(direction * enemyData.MoveSpeed * Time.fixedDeltaTime));
        }
    }

    protected override void detectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, enemyData.SightRange, LayerMask.GetMask("Player"));
        if (player != null)
        {
            isPlayerFound = true;
        }
        else
        {
            isPlayerFound = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag.Equals("Ground") || other.collider.tag.Equals("Player"))
        {
            Dead();
        }
    }
}
