using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditEnemy : Enemy
{
    [SerializeField] float maxSpeed;
    int playerXpose => collider.bounds.center.x - Character.Instance.transform.position.x >= 0 ? -1 : 1;
    bool isOKToTurn => !anim.GetBool("isAttacking");

    [SerializeField] bool isMovable = true;

    Vector2 hitPosition = new Vector2(0, 0);
    Vector2 hitBox = new Vector2(1f, 2.5f);


    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector2(enemyData.SightRange, 1.5f));

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
        hitPosition = new Vector2(rigid.position.x + (sprite.flipX ? 1 : -1) * transform.localScale.x, rigid.position.y);
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
            Character.Instance.OnDamaged(transform.position, enemyData.WeaponDmg);
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
        if (isPlayerFound && isMovable && !anim.GetBool("isAttacking"))
        {
            if (Vector2.Distance(Character.Instance.transform.position, transform.position) > 1.5f)
            {
                if (anim.GetInteger("AnimState") != 2)
                    anim.SetInteger("AnimState", 2);

                if (isOKToTurn)
                {
                    bool nowFlip = sprite.flipX;
                    bool newFlip = playerXpose <= 0 ? false : true;

                    if (nowFlip != newFlip)
                    {
                        StartCoroutine(turnDelay(newFlip));
                    }
                    else
                    {
                        anim.SetInteger("AnimState", 2);
                        rigid.AddForce(Vector2.right * playerXpose * 7f, ForceMode2D.Impulse);
                    }
                }

                if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
                {
                    rigid.velocity = new Vector2(maxSpeed * playerXpose, rigid.velocity.y);
                }

            }
            else
            {
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                attack();
            }
        }
    }

    IEnumerator turnDelay(bool newFlip)
    {
        yield return new WaitForSeconds(0.2f);
        sprite.flipX = newFlip;
        rigid.AddForce(Vector2.right * playerXpose * 7f, ForceMode2D.Impulse);
    }

    void AttackDelayEvent()
    {
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        anim.SetInteger("AnimState", 1);
        yield return new WaitForSeconds(0.5f);

        anim.SetBool("isAttacking", false);
    }

    void OnMovable()
    {
        isMovable = true;
    }

    void OffMovable()
    {
        anim.SetBool("isAttacking", false);
        isMovable = false;
    }

    public override void OnDamaged(float damage, DamageType damageType)
    {
        rigid.velocity = Vector2.zero;
        bool isCritical = false;

        if (damageType == DamageType.Player)
        {
            if (Random.value >= 0.95f)
            {
                isCritical = true;
                damage *= Random.Range(1.1f, 2.0f);
            }
        }
        else
        {
            isCritical = true;
            damage *= Random.Range(2.0f, 3.0f);
        }
        damage = Mathf.Round(damage);

        FixedUIManager.Instance.ShowDamage((int)damage, collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2), isCritical);

        hp -= damage;

        if (hp <= 0)
            Dead();
    }

}
