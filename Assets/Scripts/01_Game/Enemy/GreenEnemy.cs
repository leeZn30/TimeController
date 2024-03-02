using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GreenEnemy : Enemy
{
    bool isPlayerFound;
    bool isMovable = true;

    protected override void Awake()
    {
        base.Awake();
    }

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
        if (isPlayerFound && isMovable)
        {
            Vector2 direction;
            if (Character.Instance.gameObject.transform.position.x - transform.position.x >= 0)
            {
                direction = Vector2.right;
                sprite.flipX = true;
            }
            else
            {
                direction = Vector2.left;
                sprite.flipX = false;
            }

            rigid.AddForce(direction * enemyData.MoveSpeed * Time.deltaTime, ForceMode2D.Impulse);
        }

    }

    protected override void detectPlayer()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(enemyData.SightRange, 1.5f), 0, LayerMask.GetMask("Player"));
        if (player != null)
        {
            isPlayerFound = true;
            if (!anim.GetBool("isRun"))
                anim.SetBool("isRun", true);
        }
        else
        {
            if (anim.GetBool("isRun"))
                anim.SetBool("isRun", false);
            isPlayerFound = false;
        }
    }

    void OnMovable()
    {
        isMovable = true;
        anim.SetBool("isStun", false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag.Equals("Player"))
        {
            isMovable = false;

            // 플레이어랑 닿았음
            anim.SetBool("isStun", true);
            int dirc = transform.position.x - Character.Instance.gameObject.transform.position.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 0) * 5, ForceMode2D.Impulse);

            Invoke("OnMovable", 2f);
        }
    }

}
