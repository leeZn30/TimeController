using System.Collections;
using UnityEngine;

public class Boss0 : Enemy
{
    [Header("Boss Data")]
    [SerializeField] float ShortDistance;
    [SerializeField] float LongDistance;
    [SerializeField] float WeaponAtk;
    float accumulatedDmg = 0f;
    float sheildDmg = 100f;
    float shootTime = 0f;

    [Header("Prefabs")]
    [SerializeField] Bullet bulletPfb;
    [SerializeField] Bullet sunPfb;
    [SerializeField] Bullet windBulletPfb;
    [SerializeField] Bullet lastWindBulletPfb;

    [Header("Map Info")]
    [SerializeField] float MapRightLimit;
    [SerializeField] float MapLeftLimit;

    int sunAttackCnt = 0;
    int windAttackCnt = 0;
    int bulletCount = 10;
    float circleRadius = 0f; // 원의 반지름
    float fireRate = 0.5f; // 탄막 발사 속도
    float nextFireTime = 1f;
    float originGravityScale;

    bool BossActive = false;
    bool manualEvasion = true;
    bool isOKToEvasion =>
    manualEvasion &&
    !isEvasioning &&
    hitCount >= 5 &&
    !anim.GetBool("isCharging") &&
    accumulatedDmg < sheildDmg &&
    !anim.GetBool("isUltimate") &&
    !isWindAttacking;

    bool manualChase = true;
    bool isOkToChase =>
    manualChase &&
    currentDistance > ShortDistance &&
    !anim.GetBool("isAttacking") &&
    !isEvasioning &&
    !anim.GetBool("isThrowingSun") &&
    !isDash &&
    !anim.GetBool("isCharging") &&
    !anim.GetBool("isUltimate") &&
    !isWindAttacking;

    bool manualHit = true;
    bool isOKToHit => manualHit;

    bool isOKToTurn => !anim.GetBool("isAttacking") && !isDash && !anim.GetBool("isCollapsed");

    bool manualAttack = true;
    bool isOKToAttack =>
    manualAttack &&
    currentDistance < ShortDistance &&
    !anim.GetBool("isAttacking");

    bool isWindAttacking = false;
    bool isDash = false;
    bool isEvasioning = false;
    int hitCount = 0;
    int playerXpose => collider.bounds.center.x - Character.Instance.transform.position.x >= 0 ? -1 : 1;
    // 단순 거리 재기이므로 rigid 사용할 필요 없음
    float currentDistance => Vector3.Distance(collider.bounds.center, Character.Instance.transform.position);
    Vector2 attackRange = new Vector2(2, 3);
    Vector2 attackPosition;
    Coroutine shieldCoroutine;

    GameObject sun;
    GameObject shield;
    TrailRenderer trail;
    LineRenderer line;

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
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

        trail = transform.GetChild(2).gameObject.GetComponent<TrailRenderer>();
        trail.enabled = false;

        originGravityScale = rigid.gravityScale;

    }

    protected override void Update()
    {
        detectPlayer();

        if (BossActive)
        {
            attackPosition = new Vector2(collider.bounds.center.x + collider.bounds.extents.x * (sprite.flipX ? 1 : -1), collider.bounds.center.y);

            land();

            attack();
            Shoot();

            // Evasion과 겹칠때 우선순위 둬야 함
            if (accumulatedDmg > sheildDmg && !anim.GetBool("isCharging") && !isEvasioning)
                shieldCoroutine = StartCoroutine(Shield());
        }
    }

    private void FixedUpdate()
    {
        if (BossActive)
        {
            Chase();
            Evasion();
        }
    }

    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    IEnumerator turnDelay(bool newFlip)
    {
        yield return Delay(0.2f);
        sprite.flipX = newFlip;
        rigid.AddForce(Vector2.right * playerXpose * 7f, ForceMode2D.Impulse);
    }

    void Chase()
    {
        if (isOkToChase)
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


            if (Mathf.Abs(rigid.velocity.x) > 7f)
            {
                rigid.velocity = new Vector2(7f * playerXpose, rigid.velocity.y);
            }
        }
        else
        {
            if (!isEvasioning && !isDash && shieldCoroutine == null)
            {
                anim.SetInteger("AnimState", 1);
                rigid.velocity = new Vector2(0, rigid.velocity.y);
            }
        }
    }

    protected override void attack()
    {
        if (isOKToAttack)
        {
            anim.SetTrigger("Attack");
        }
    }

    void OnAttackStart()
    {
        anim.SetBool("isAttacking", true);
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

    void Dash(float speed = 15f)
    {
        if (!isDash && !anim.GetBool("isThrowingSun"))
        {
            // sprite 바꿔야 함
            trail.enabled = true;
            isDash = true;

            Vector2 target = (Vector2)Character.Instance.transform.position;
            Vector2 direction = (target - rigid.position).normalized;

            rigid.AddForce(direction * speed, ForceMode2D.Impulse);

            StartCoroutine(DashDelay());
        }
    }

    IEnumerator DashDelay()
    {
        yield return Delay(0.5f);

        trail.enabled = false;
        if (rigid.gravityScale == 0f)
            rigid.gravityScale = originGravityScale;

        if (!manualEvasion)
            manualEvasion = true;

        isDash = false;
    }

    void Evasion()
    {
        if (isOKToEvasion)
        {
            isEvasioning = true;
            if (anim.GetBool("isCharging"))
                return;

            anim.SetBool("isAttacking", false);

            hitCount = 0;

            float dst = Random.Range(1, 3);
            Vector2 targetPosition = new Vector2(rigid.position.x + -playerXpose * dst, rigid.position.y);
            if (targetPosition.x < MapLeftLimit || targetPosition.x > MapRightLimit)
            {
                targetPosition = new Vector2(rigid.position.x + playerXpose * dst, rigid.position.y);
            }
            Jump(targetPosition);
        }
    }

    void Jump(Vector2 targetPosition)
    {
        anim.SetTrigger("Jump");

        // 시작 위치와 목표 위치 사이의 방향 벡터 계산
        Vector2 direction = targetPosition - rigid.position;

        // 점프에 필요한 초기 속도 계산 (수직 속도 계산)
        float verticalVelocity = Mathf.Sqrt(2 * Mathf.Abs(Physics2D.gravity.y) * direction.magnitude);

        // 수직 속도와 방향에 맞는 힘 계산
        Vector2 jumpVelocity = new Vector2(direction.x, verticalVelocity).normalized * 30f;

        // Rigidbody2D에 힘을 가해서 점프
        rigid.velocity = jumpVelocity;
    }

    void throwSun()
    {
        sunAttackCnt++;
        anim.SetTrigger("ThrowReady");
    }

    void OnThrowReady()
    {
        anim.SetBool("isThrowingSun", true);
        anim.SetBool("isAttacking", false);
        StartCoroutine(throwSunReady());
    }

    IEnumerator throwSunReady()
    {
        rigid.gravityScale = 0f;
        sun.SetActive(true);

        float yPosition = Random.Range(-1, 3);
        while (rigid.position.y < yPosition)
        {
            rigid.position += Vector2.up * 15f * Time.deltaTime;
            yield return null;
        }

        anim.SetTrigger("ThrowSun");
    }

    void OnThrowSunStart()
    {
        sun.SetActive(false);
        BulletManager.TakeOutBullet(sunPfb.name, sun.transform.position);
    }

    void OnThrowSunEnd()
    {
        anim.SetBool("isThrowingSun", false);
        Dash(30f);
    }

    void windAttack()
    {
        windAttackCnt++;
        isWindAttacking = true;
        anim.SetInteger("AnimState", 1);
        anim.SetBool("isAttacking", false);
        StartCoroutine(knifeWind());
    }

    IEnumerator knifeWind()
    {
        float duration = 0f;
        float fireRate = 1.7f;
        float spawnTimer = 0f;
        float windCnt = 0;
        while (duration < 6f)
        {
            duration += Time.deltaTime;
            spawnTimer -= Time.deltaTime;
            while (spawnTimer <= 0f)
            {
                windCnt++;

                if (windCnt <= 3)
                {
                    Bullet w = BulletManager.TakeOutBullet(windBulletPfb.name, rigid.position + Vector2.right * playerXpose);
                    w.targetDirection = (Vector3.right * playerXpose).normalized;
                    w.transform.rotation = Quaternion.identity;
                }
                else
                {
                    Bullet w = BulletManager.TakeOutBullet(lastWindBulletPfb.name, rigid.position + Vector2.right * playerXpose);
                    w.targetDirection = (Vector3.right * playerXpose).normalized;
                    w.transform.rotation = Quaternion.identity;
                }

                spawnTimer = fireRate;
                yield return null;
            }
            yield return null;
        }

        isWindAttacking = false;
    }

    IEnumerator Shield()
    {
        Character.Instance.KnockBack(rigid.position, 10f);

        manualChase = false;
        manualEvasion = false;
        manualAttack = false;

        anim.SetTrigger("Charge");
        anim.SetBool("isCharging", true);
        anim.SetBool("isAttacking", false);
        accumulatedDmg = 0f;

        shield.transform.position = rigid.position + Vector2.right * 1.5f * playerXpose;
        shield.SetActive(true);

        yield return Delay(3f);

        anim.SetBool("isCharging", false);

        rigid.gravityScale = 0f;
        while (rigid.position.y < 1)
        {
            rigid.position += Vector2.up * 15f * Time.deltaTime;
            yield return null;
        }

        shield.SetActive(false);
        anim.SetTrigger("Ultimate");
        anim.SetBool("isUltimate", true);
    }

    void Shoot()
    {
        if (anim.GetBool("isUltimate") && shootTime < 5f)
        {
            shootTime += Time.deltaTime;
            if (Time.time > nextFireTime)
            {
                nextFireTime = Time.time + fireRate;

                float radiusOffset = Random.Range(0, 360);
                // 원형으로 탄막 생성
                for (int i = 0; i < bulletCount; i++)
                {
                    float angle = i * (360f / bulletCount) + radiusOffset; // 원형을 구성하는 각도 계산
                    Vector2 bulletDirection = Quaternion.Euler(0, 0, angle) * Vector3.right; // 각도에 따른 방향 계산
                    Vector2 bulletPosition = (Vector2)collider.bounds.center + bulletDirection * circleRadius; // 원 주위의 위치 계산

                    // 탄막 생성
                    Bullet b = BulletManager.TakeOutBullet(bulletPfb.name, bulletPosition);
                    b.targetDirection = bulletDirection;
                    float Bangle = Mathf.Atan2(b.targetDirection.y, b.targetDirection.x) * Mathf.Rad2Deg + 180f;
                    b.transform.rotation = Quaternion.AngleAxis(Bangle, Vector3.forward);
                }
            }
        }
        else if (shootTime > 5f)
        {
            shootTime = 0f;
            hitCount = 0;
            anim.SetBool("isUltimate", false);

            manualChase = true;
            manualAttack = true;

            Dash(30f);
        }
    }

    void land()
    {
        Debug.DrawRay(rigid.position + new Vector2(0, collider.bounds.extents.y), Vector2.down * collider.bounds.extents.y, Color.red);
        int layerMask = (1 << LayerMask.NameToLayer("ThroughMap")) | (1 << LayerMask.NameToLayer("Map"));
        RaycastHit2D hit = Physics2D.Raycast(rigid.position + new Vector2(0, collider.bounds.extents.y), Vector2.down, collider.bounds.extents.y, layerMask);

        if (hit.collider != null && hit.collider.tag.Equals("Ground"))
        {
            anim.SetBool("Grounded", true);

            if (isEvasioning)
            {
                if (Mathf.Abs(sunAttackCnt - windAttackCnt) < 2)
                {
                    int randomValue = Random.Range(0, 2);
                    switch (randomValue)
                    {
                        case 0:
                            throwSun();
                            break;

                        case 1:
                            windAttack();
                            break;

                        default:
                            break;
                    }
                }
                else
                {
                    if (sunAttackCnt > windAttackCnt)
                    {
                        windAttack();

                    }
                    else
                    {
                        throwSun();
                    }
                }
                isEvasioning = false;
            }
        }
        else
        {
            anim.SetBool("Grounded", false);

            // rigid.gravityScale = originGravityScale;
        }
    }

    public override void OnDamaged(float damage, DamageType damageType)
    {
        if (isOKToHit)
        {
            bool isCritical = false;
            if (damageType == DamageType.Player)
            {
                if (Random.value >= 0.95f)
                {
                    isCritical = true;
                    damage *= Random.Range(1.5f, 2.0f);
                }
            }
            else
            {
                isCritical = true;
                damage *= Random.Range(2.0f, 3.0f);
            }
            damage = Mathf.Round(damage);

            FixedUIManager.Instance.ShowDamage((int)damage, collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2), isCritical);

            if (anim.GetBool("isCharging"))
            {
                if (shieldCoroutine != null)
                    StopCoroutine(shieldCoroutine);
                accumulatedDmg = 0f;
                gameObject.layer = 16;

                anim.SetBool("isCharging", false);
                shield.SetActive(false);
                anim.SetTrigger("Collapse");
                anim.SetBool("isCollapsed", true);
                StartCoroutine(CollapseWaiting());
            }
            else if (!anim.GetBool("isCollapsed"))
            {
                accumulatedDmg += damage;
                hitCount++;
            }

            hp -= damage;
            if (hp <= 0)
                Dead();

        }
    }

    IEnumerator CollapseWaiting()
    {
        yield return Delay(3f);

        gameObject.layer = 7;
        anim.SetBool("isCollapsed", false);
    }

    void OnRecoverStart()
    {
        manualHit = false;
    }

    void OnRecoverEnd()
    {
        manualHit = true;

        manualChase = true;
        manualEvasion = true;
        manualAttack = true;
    }

    protected override void detectPlayer()
    {
        if (!BossActive)
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, enemyData.SightRange, LayerMask.GetMask("Player"));
            if (player != null)
            {
                BossActive = true;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

}
