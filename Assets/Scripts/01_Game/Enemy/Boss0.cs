using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Boss0 : Enemy
{
    [Header("Boss Data")]
    [SerializeField] float ShortDistance;
    [SerializeField] float LongDistance;
    [SerializeField] float DashPower;
    [SerializeField] float WeaponAtk;
    [SerializeField] float accumulatedDmg;

    [Header("Bullets")]
    [SerializeField] Bullet bulletPfb;
    [SerializeField] Bullet sunPfb;

    [Header("Map Info")]
    [SerializeField] float MapRightLimit;
    [SerializeField] float MapLeftLimit;
    [SerializeField] List<Collider2D> platforms = new List<Collider2D>();

    int bulletCount = 10;
    float circleRadius = 1f; // 원의 반지름
    float fireRate = 0.5f; // 탄막 발사 속도
    float nextFireTime = 1f;
    float originGravityScale;

    bool isOKToDash =>
    currentDistance > LongDistance &&
     !isDash && !anim.GetBool("isCharging") &&
      anim.GetBool("Grounded") &&
       !anim.GetBool("isThrowingSun");
    bool isOKToHit => !anim.GetBool("isUltimated") && !anim.GetBool("isThrowingSun");
    bool isOKToTurn => !anim.GetBool("isCharging") && !anim.GetBool("isCollapsed") && !anim.GetBool("isUltimate");
    bool isOKToAttack =>
    currentDistance < ShortDistance &&
     !anim.GetBool("isAttacking") &&
       !anim.GetBool("isCharging") &&
        !anim.GetBool("isCollapsed") &&
         !anim.GetBool("isUltimate");
    bool isDash = false;
    int hitCount = 0;
    int playerXpose => collider.bounds.center.x - Character.Instance.transform.position.x >= 0 ? -1 : 1;
    // 단순 거리 재기이므로 rigid 사용할 필요 없음
    float currentDistance => Vector3.Distance(collider.bounds.center, Character.Instance.transform.position);
    Vector2 attackRange = new Vector2(2, 3);
    Vector2 attackPosition;
    ChromaticAberration chromatic;

    GameObject sun;
    GameObject shield;
    LineRenderer line;

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(attackPosition, attackRange);
    }

    protected override void Awake()
    {
        base.Awake();

        line = GetComponent<LineRenderer>();
        line.enabled = false;

        shield = transform.GetChild(0).gameObject;
        shield.SetActive(false);

        sun = transform.GetChild(1).gameObject;
        sun.SetActive(false);

        originGravityScale = rigid.gravityScale;

        FindObjectOfType<Volume>().profile.TryGet(out chromatic);

    }

    protected override void Update()
    {
        attackPosition = new Vector2(collider.bounds.center.x + (collider.bounds.size.x / 2) * (sprite.flipX ? 1 : -1), collider.bounds.center.y);

        if (isOKToTurn)
            sprite.flipX = playerXpose < 0 ? false : true;

        attack();

        if (accumulatedDmg > 100f)
            StartCoroutine(Shield());

        Shoot();

        if (isEvasion && anim.GetBool("Grounded"))
        {
            StartCoroutine(throwSun());
            isEvasion = false;
        }

        if (Input.GetKeyDown(KeyCode.G))
            jump();

        land();

    }

    private void FixedUpdate()
    {
        // Dash();
        Evasion();
        // land();
    }

    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator throwSun()
    {
        anim.SetBool("isThrowingSun", true);
        rigid.position += Vector2.up;
        rigid.gravityScale = 0f;

        anim.SetTrigger("ThrowReady");
        sun.SetActive(true);

        yield return StartCoroutine(Delay(1f));

        sun.SetActive(false);
        anim.SetTrigger("ThrowSun");
        BulletManager.TakeOutBullet(sunPfb.name, sun.transform.position);
        rigid.gravityScale = originGravityScale;
        anim.SetBool("isThrowingSun", false);

        Dash();
    }

    IEnumerator Shield()
    {
        // 맵 가운데로 돌진
        if (!anim.GetBool("isCharging"))
        {
            accumulatedDmg = 0f;

            anim.SetTrigger("Charge");
            anim.SetBool("isCharging", true);

            shield.transform.position = rigid.position + Vector2.right * 1.5f * playerXpose;
            shield.SetActive(true);

            // yield return StartCoroutine(Delay(3f));

            float duration = 0f;

            while (duration < 3f)
            {
                duration += Time.deltaTime;
                // float newIntensity = Mathf.Clamp(chromatic.intensity.value + 0.3f * Time.deltaTime, 0f, 1f);
                // chromatic.intensity.value = newIntensity;
                yield return null;
            }

            shield.SetActive(false);
            anim.SetBool("isCharging", false);
        }
    }

    IEnumerator Ultimate()
    {
        yield return Delay(5f);

        anim.SetBool("isUltimate", false);
        OnUltimateEnd();
    }

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
                    float Bangle = Mathf.Atan2(b.targetDirection.y, b.targetDirection.x) * Mathf.Rad2Deg + 180f;
                    b.transform.rotation = Quaternion.AngleAxis(Bangle, Vector3.forward);
                }
            }
        }
    }

    void Dash()
    {
        if (isOKToDash)
        {
            isDash = true;
            StartCoroutine(Dashing());
        }
    }

    IEnumerator Dashing()
    {
        Vector3 targetPosition = Character.Instance.transform.position;
        float dashSpeed = 15f;

        // line.SetPositions(new Vector3[] { rigid.position, targetPosition });
        // line.enabled = true;

        yield return new WaitForSeconds(1f);

        anim.SetFloat("AnimState", 2);
        while (rigid.position != (Vector2)targetPosition)
        {
            rigid.position = Vector2.MoveTowards(rigid.position, targetPosition, dashSpeed * Time.fixedDeltaTime);
            yield return null;
        }

        anim.SetFloat("AnimState", 1);

        // line.enabled = false;
        isDash = false;
    }

    bool isEvasion = false;
    void Evasion()
    {
        if (currentDistance < ShortDistance && hitCount > 3 && anim.GetBool("Grounded") && !isEvasion)
        {
            isEvasion = true;

            anim.SetTrigger("Jump");
            anim.SetBool("Grounded", false);

            Vector2 targetPosition = new Vector2(rigid.position.x + -playerXpose * 3f, rigid.position.y);
            if (targetPosition.x < MapLeftLimit || targetPosition.x > MapRightLimit)
            {
                targetPosition = new Vector2(rigid.position.x + playerXpose * 3f, rigid.position.y);
            }

            // 시작 위치와 목표 위치 사이의 방향 벡터 계산
            Vector2 direction = (targetPosition - rigid.position);

            // 점프에 필요한 초기 속도 계산 (수직 속도 계산)
            float verticalVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * direction.magnitude);

            // 수직 속도와 방향에 맞는 힘 계산
            Vector2 jumpVelocity = new Vector2(direction.x, verticalVelocity).normalized * 30f;

            // Rigidbody2D에 힘을 가해서 점프
            rigid.velocity = jumpVelocity;

            hitCount = 0;
        }
    }

    void jump()
    {
        anim.SetTrigger("Jump");
        // anim.SetBool("Grounded", false);

        rigid.AddForce(Vector2.up * 20f, ForceMode2D.Impulse);
    }

    void land()
    {
        Debug.DrawRay(rigid.position + new Vector2(0, collider.bounds.extents.y), Vector2.down * collider.bounds.extents.y, Color.red);
        int layerMask = (1 << LayerMask.NameToLayer("ThroughMap")) | (1 << LayerMask.NameToLayer("Map"));
        RaycastHit2D hit = Physics2D.Raycast(rigid.position + new Vector2(0, collider.bounds.extents.y), Vector2.down, collider.bounds.extents.y, layerMask);

        if (hit.collider != null && hit.collider.tag.Equals("Ground"))
        {
            anim.SetBool("Grounded", true);
        }
        else
        {
            anim.SetBool("Grounded", false);
        }
    }

    public override void OnDamaged(float damage, DamageType damageType)
    {
        if (isOKToHit)
        {
            accumulatedDmg += damage;

            hp -= damage;
            if (hp <= 0)
                Dead();

            hitCount++;

            if (anim.GetBool("isCharging"))
            {
                if (chromatic.intensity.value != 0f)
                    chromatic.intensity.value = 0f;

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

    void OnUltimateEnd()
    {
        rigid.gravityScale = originGravityScale;
    }

    #region NoNeed
    // 필요없음
    protected override void detectPlayer()
    {
    }
    #endregion
}
