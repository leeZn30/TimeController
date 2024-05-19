using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Boss0 : Enemy
{
    [Header("Boss Data")]
    [SerializeField] float ShortDistance;
    float accumulatedDmg = 0f;
    float shieldDmg = 100f;
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
    accumulatedDmg < shieldDmg &&
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

    bool isOKToHit = true;

    bool isOKToTurn => !anim.GetBool("isAttacking") && !isDash && !anim.GetBool("isCollapsed");

    bool manualAttack = true;
    bool isOKToAttack =>
    manualAttack &&
    currentDistance < ShortDistance &&
    !anim.GetBool("isAttacking");

    bool readyToShield = false;
    bool isWindAttacking = false;
    bool isDash = false;
    bool isEvasioning = false;
    int hitCount = 0;
    int playerXpose => collider.bounds.center.x - Character.Instance.transform.position.x >= 0 ? -1 : 1;
    float currentDistance
    {
        get
        {
            if (Character.Instance != null) return Vector3.Distance(collider.bounds.center, Character.Instance.transform.position);
            else return 0f;
        }
    }

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
        ObjectData data = GameData.ObjectDatas.Find(e => e.ID == gameObject.name);
        if (data == null)
        {
            GameData.ObjectDatas.Add(new ObjectData(gameObject.name, true));
        }
        else
        {
            if (!data.IsExist)
            {
                Destroy(gameObject);
            }
        }

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
        }
    }


    private void FixedUpdate()
    {
        if (BossActive)
        {
            Chase();
            Evasion();

            // Evasion과 겹칠때 우선순위 둬야 함
            if (accumulatedDmg > shieldDmg && !anim.GetBool("isCharging") && !isEvasioning)
            {
                jumpToShield();
            }
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
            if (!isEvasioning && !isDash && !readyToShield)
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
        SoundManager.Instance.PlaySFX(AudioType.Enemy, BossSoundData.Instance.GetSound("Attack"));
        Collider2D player = Physics2D.OverlapBox(attackPosition, attackRange, 0, LayerMask.GetMask("Player"));
        if (player != null)
        {
            Character.Instance.OnDamaged(transform.position, enemyData.WeaponDmg);
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
                SoundManager.Instance.PlaySFX(AudioType.Enemy, BossSoundData.Instance.GetSound("Wind"));
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

    void jumpToShield()
    {
        if (!readyToShield)
        {
            readyToShield = true;

            manualChase = false;
            manualEvasion = false;
            manualAttack = false;

            Vector2 targetPosition = new Vector2(rigid.position.x + -playerXpose, rigid.position.y);
            if (targetPosition.x < MapLeftLimit || targetPosition.x > MapRightLimit)
            {
                targetPosition = new Vector2(rigid.position.x + playerXpose, rigid.position.y);
            }
            Jump(targetPosition);
        }
    }

    IEnumerator Shield()
    {
        readyToShield = false;

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
        SoundManager.Instance.PlaySFX(AudioType.Enemy, BossSoundData.Instance.GetSound("Roar"));
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
        List<RaycastHit2D> hits = Physics2D.RaycastAll(rigid.position + new Vector2(0, collider.bounds.extents.y), Vector2.down, collider.bounds.extents.y).ToList();

        if (hits.Count > 0 && hits.Find(e => e.collider.CompareTag("Ground")))
        {
            anim.SetBool("Grounded", true);

            if (readyToShield)
            {
                shieldCoroutine = StartCoroutine(Shield());
            }

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
            {
                Dead();
            }
        }
        else
        {
            FixedUIManager.Instance.ShowDamage(0, collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2), false);
        }
    }

    IEnumerator CollapseWaiting()
    {
        collider.isTrigger = true;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;

        yield return Delay(3f);

        gameObject.layer = LayerMask.NameToLayer("Enemy");
        anim.SetBool("isCollapsed", false);

        collider.isTrigger = false;
        rigid.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
    }

    void OnRecoverStart()
    {
        isOKToHit = false;
    }

    void OnRecoverEnd()
    {
        isOKToHit = true;

        manualChase = true;
        manualEvasion = true;
        manualAttack = true;
    }

    protected override void detectPlayer()
    {
        if (!BossActive)
        {
            Collider2D player = Physics2D.OverlapCircle(transform.position, enemyData.SightRange, LayerMask.GetMask("Player"));
            if (player != null && !isDead)
            {
                BossActive = true;
            }
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
    }

    protected override void Dead()
    {
        SoundManager.Instance.PlaySFX(AudioType.Enemy, BossSoundData.Instance.GetSound("Dead"));

        isDead = true;
        BossActive = false;
        anim.SetBool("isDead", true);

        rigid.gravityScale = originGravityScale;
        StopAllCoroutines();
        BossManager.Instance.Clear();
        ObjectData data = GameData.ObjectDatas.Find(e => e.ID == gameObject.name);
        data.IsExist = false;

        SoundManager.Instance.PlayBGM("Stage");

        base.Dead();
    }

    void OnDead()
    {
        anim.SetBool("isAttacking", false);
        anim.SetBool("isCharging", false);
        anim.SetBool("isUltimate", false);
        anim.SetBool("isCollapsed", false);
        anim.SetBool("isThrowingsun", false);
    }

    void OnChromatic()
    {
        ChromaticAberration chromatic = null;
        FindObjectOfType<Volume>().profile.TryGet(out chromatic);

        StartCoroutine(ShimmerWindow(chromatic));
    }

    IEnumerator ShimmerWindow(ChromaticAberration chromatic)
    {
        SoundManager.Instance.PlaySFX(AudioType.Enemy, BossSoundData.Instance.GetSound("Roar"));

        float duration = 0;
        while (duration < 2.5f)
        {
            duration += Time.deltaTime;

            if (chromatic.intensity.value < 1f)
            {
                chromatic.intensity.value += 0.1f;
            }
            else
            {
                chromatic.intensity.value -= 0.1f;
            }

            if (chromatic.intensity.value < 0)
            {
                chromatic.intensity.value = 0f;
            }

            yield return null;
        }

        chromatic.intensity.value = 0f;
    }

}
