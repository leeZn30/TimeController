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
    [SerializeField] float WeaponAtk;

    [Header("Bullets")]
    [SerializeField] Bullet bulletPfb;
    [SerializeField] Bullet sunPfb;

    float originGravityScale;
    bool isOKToHit => !anim.GetBool("isUltimated");
    bool isOKToTurn => !anim.GetBool("isCharging") && !anim.GetBool("isCollapsed") && !anim.GetBool("isUltimate");
    bool isOKToAttack => currentDistance < ShortDistance && !anim.GetBool("isAttacking") && !anim.GetBool("isCharging") && !anim.GetBool("isCollapsed") && !anim.GetBool("isUltimate");
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

        originGravityScale = rigid.gravityScale;
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(attackPosition, attackRange);
    }

    protected override void Update()
    {
        attackPosition = new Vector2(collider.bounds.center.x + (collider.bounds.size.x / 2) * (sprite.flipX ? 1 : -1), collider.bounds.center.y);

        if (isOKToTurn)
            sprite.flipX = playerXpose < 0 ? false : true;

        attack();

        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(Shield());

        Shoot();
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

    void OnUlimateStart()
    {
        rigid.gravityScale = 0f;
        rigid.position += Vector2.up;

        anim.SetBool("isUltimate", true);
    }

    void OnUltimate()
    {
        StartCoroutine(Ultimate());
    }

    IEnumerator Ultimate()
    {
        yield return Delay(5f);

        anim.SetBool("isUltimate", false);
        OnUltimateEnd();
    }

    int bulletCount = 10;
    float circleRadius = 1f; // 원의 반지름
    float fireRate = 0.5f; // 탄막 발사 속도
    float nextFireTime = 1f;

    void Shoot()
    {
        if (anim.GetBool("isUltimate"))
        {
            if (Time.time > nextFireTime)
            {
                nextFireTime = Time.time + fireRate;

                float radiusOffset = Random.Range(0, 360);
                // 원형으로 탄막 생성
                for (int i = 0; i < bulletCount; i++)
                {
                    float angle = i * (360f / bulletCount) + radiusOffset; // 원형을 구성하는 각도 계산
                    Vector2 bulletDirection = Quaternion.Euler(0, 0, angle) * Vector3.right; // 각도에 따른 방향 계산
                    Vector2 bulletPosition = rigid.position + bulletDirection * circleRadius; // 원 주위의 위치 계산

                    // 탄막 생성
                    Bullet b = BulletManager.TakeOutBullet(bulletPfb.name, bulletPosition);
                    b.targetDirection = bulletDirection;
                }
            }
        }
    }

    void OnUltimateEnd()
    {
        rigid.gravityScale = originGravityScale;
    }

    void Run()
    {

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
        if (isOKToHit)
        {
            hitCount++;

            if (anim.GetBool("isCharging"))
            {
                shield.SetActive(false);
                anim.SetTrigger("Collapse");
                anim.SetBool("isCollapsed", true);
            }
        }
    }

    protected override void attack()
    {
        if (isOKToAttack)
        {
            anim.SetBool("isAttacking", true);
            anim.SetTrigger("Attack");
        }
    }

    void OnAttack()
    {
        Collider2D player = Physics2D.OverlapBox(attackPosition, attackRange, 0, LayerMask.GetMask("Player"));
        if (player != null)
        {
            Character.Instance.OnDamaged(transform.position, WeaponAtk);
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
