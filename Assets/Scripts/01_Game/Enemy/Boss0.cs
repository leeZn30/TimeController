using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Boss0 : Enemy
{
    [Header("Boss Data")]
    [SerializeField] float ShortDistance;
    [SerializeField] float LongDistance;
    [SerializeField] float DashPower;

    bool isDash = false;
    int hitCount = 0;
    int playerXpose => collider.bounds.center.x - Character.Instance.transform.position.x >= 0 ? -1 : 1;
    // 단순 거리 재기이므로 rigid 사용할 필요 없음
    float currentDistance => Vector3.Distance(collider.bounds.center, Character.Instance.transform.position);
    Vector2 attackRange = new Vector2(2, 3);
    Vector2 attackPosition;

    GameObject shield;
    LineRenderer line;

    protected override void Awake()
    {
        base.Awake();

        line = GetComponent<LineRenderer>();
        line.enabled = false;

        shield = transform.GetChild(0).gameObject;
        shield.SetActive(false);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(attackPosition, attackRange);
    }

    protected override void Update()
    {
        attackPosition = new Vector2(collider.bounds.center.x + (collider.bounds.size.x / 2) * (sprite.flipX ? 1 : -1), collider.bounds.center.y);

        if (!anim.GetBool("isCharging") && !anim.GetBool("isCollapsed"))
            sprite.flipX = playerXpose < 0 ? false : true;

        attack();

        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(Shield());
    }

    private void FixedUpdate()
    {
        // Dash();
    }

    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator Shield()
    {
        // 맵 가운데로 돌진
        if (!anim.GetBool("isCharging"))
        {
            anim.SetTrigger("Charge");
            anim.SetBool("isCharging", true);

            shield.transform.position = rigid.position + Vector2.right * 1.5f * playerXpose;
            shield.SetActive(true);

            yield return StartCoroutine(Delay(3f));

            shield.SetActive(false);
            anim.SetBool("isCharging", false);
        }
    }

    void Dash()
    {
        if (!isDash)
        {
            Vector3 startPose = rigid.position;
            // 플레이어가 너무 멀리 떨어져 있거나
            if (currentDistance > LongDistance)
            {
                line.SetPositions(new Vector3[] { new Vector2(rigid.position.x, rigid.position.y), new Vector2(Character.Instance.transform.position.x, Character.Instance.transform.position.y) });
                line.enabled = true;

                StartCoroutine(Delay(2f));

                // isDash = true;
                // 돌진 방향 계산
                Vector2 dashDirection = (Character.Instance.transform.position - transform.position).normalized;
                // Rigidbody2D에 속도를 설정하여 돌진
                rigid.AddForce(dashDirection * DashPower, ForceMode2D.Impulse);
            }
            // 플레이어가 가까이 있는데, 몇 대 맞은 경우
            else if (currentDistance < ShortDistance && hitCount > 5)
            {
                hitCount = 0;
            }

            // if (Vector3.Distance(rigid.position, startPose) > 10f)
            // {
            //     line.enabled = false;
            //     rigid.velocity = Vector2.zero;
            //     isDash = false;
            // }
        }

    }

    public override void OnDamaged(float damage, DamageType damageType)
    {
        hitCount++;

        if (anim.GetBool("isCharging"))
        {
            shield.SetActive(false);
            anim.SetTrigger("Collapse");
            anim.SetBool("isCollapsed", true);
        }
    }

    protected override void attack()
    {
        if (currentDistance < ShortDistance && !anim.GetBool("isAttacking"))
        {
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("Attack");
        }
    }


    void OnAttackEnd()
    {
        anim.SetBool("isAttacking", false);
    }

    void OnCollapseEnd()
    {
        anim.SetBool("isCollapsed", false);
    }



    #region NoNeed
    // 필요없음
    protected override void detectPlayer()
    {
    }
    #endregion
}
