using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditEnemy : Enemy
{
    [SerializeField] float maxSpeed;
    [SerializeField] float WeaponAtk;

    bool isMovable = true;

    Vector2 hitPosition = new Vector2(0, 0);
    Vector2 hitBox = new Vector2(1f, 2.5f);


    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + Vector3.up, new Vector2(enemyData.SightRange, 1.5f));

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitPosition, hitBox);
    }

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        detectPlayer();
        hitPosition = new Vector2(rigid.position.x + (sprite.flipX ? 1 : -1) * transform.localScale.x, rigid.position.y + 1.5f);
    }

    private void FixedUpdate()
    {
        if (gameObject.layer != 11)
            run();
    }

    protected override void attack()
    {
        if (!anim.GetBool("isAttacking"))
        {
            isMovable = false;
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("Attack");
        }
    }

    // 애니메이션에서 호출
    void AttackEvent()
    {
        Collider2D player = Physics2D.OverlapBox(hitPosition, hitBox, 0, LayerMask.GetMask("Player"));
        if (player != null)
        {
            Character.Instance.OnDamaged(transform.position, WeaponAtk);
        }
    }

    protected override void detectPlayer()
    {
        if (!isPlayerFound)
        {
            Collider2D player = Physics2D.OverlapBox(transform.position + Vector3.up, new Vector2(enemyData.SightRange, 1.3f), 0, LayerMask.GetMask("Player"));
            if (player != null)
            {
                isPlayerFound = true;
                anim.SetInteger("AnimState", 1);
            }
        }
    }

    void run()
    {
        if (isPlayerFound && isMovable)
        {
            if (Vector2.Distance(Character.Instance.transform.position, transform.position) > 2f)
            {
                if (anim.GetInteger("AnimState") != 2)
                    anim.SetInteger("AnimState", 2);

                float direction;
                if (Character.Instance.gameObject.transform.position.x - transform.position.x >= 0)
                {
                    direction = 1;
                    sprite.flipX = true;
                }
                else
                {
                    direction = -1;
                    sprite.flipX = false;
                }

                rigid.AddForce(Vector2.right * direction * enemyData.MoveSpeed, ForceMode2D.Impulse);

                if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
                {
                    rigid.velocity = new Vector2(maxSpeed * direction, rigid.velocity.y);
                }

            }
            else
            {
                rigid.velocity = new Vector2(0, rigid.velocity.y);

                attack();
            }
        }
    }

    void AttackDelayEvent()
    {
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        anim.SetInteger("AnimState", 1);
        yield return new WaitForSeconds(0.5f);

        isMovable = true;
        anim.SetBool("isAttacking", false);
    }

    void OnMovable()
    {
        isMovable = true;
        anim.SetBool("isAttacking", false);
    }

    void OffMovable()
    {
        isMovable = false;
    }

}
