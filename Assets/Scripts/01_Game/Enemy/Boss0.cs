using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Boss0 : Enemy
{
    [Header("Boss Data")]
    [SerializeField] float ShortDistance;
    [SerializeField] float LongDistance;
    [SerializeField] float WeaponAtk;
    [SerializeField] float accumulatedDmg;

    [Header("Prefabs")]
    [SerializeField] Bullet bulletPfb;
    [SerializeField] Bullet sunPfb;

    [Header("Map Info")]
    [SerializeField] float MapRightLimit;
    [SerializeField] float MapLeftLimit;

    int bulletCount = 10;
    float circleRadius = 1f; // 원의 반지름
    float fireRate = 0.5f; // 탄막 발사 속도
    float nextFireTime = 1f;
    float originGravityScale;

    bool isOKToEvasion =>
    !isEvasioning &&
    hitCount >= 5;

    bool isOKToHit = true;

    bool isOKToTurn => !anim.GetBool("isAttacking") && !isDash;

    bool isOKToAttack =>
    currentDistance < ShortDistance &&
    !anim.GetBool("isAttacking");

    bool isDash = false;
    bool isEvasioning = false;
    int hitCount = 0;
    int playerXpose => collider.bounds.center.x - Character.Instance.transform.position.x >= 0 ? -1 : 1;
    // 단순 거리 재기이므로 rigid 사용할 필요 없음
    float currentDistance => Vector3.Distance(collider.bounds.center, Character.Instance.transform.position);
    Vector2 attackRange = new Vector2(2, 3);
    Vector2 attackPosition;
    ChromaticAberration chromatic;

    GameObject sun;
    GameObject shield;
    TrailRenderer trail;
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

        trail = transform.GetChild(2).gameObject.GetComponent<TrailRenderer>();
        trail.enabled = false;

        originGravityScale = rigid.gravityScale;

        FindObjectOfType<Volume>().profile.TryGet(out chromatic);
    }

    protected override void Update()
    {
        attackPosition = new Vector2(collider.bounds.center.x + collider.bounds.extents.x * (sprite.flipX ? 1 : -1), collider.bounds.center.y);

        // if (isOKToTurn)
        //     sprite.flipX = playerXpose <= 0 ? false : true;
        land();

        attack();
        // Shoot();
        // if (accumulatedDmg > 100f)
        //     StartCoroutine(Shield());
        // if (isEvasion && anim.GetBool("Grounded"))
        // {
        //     StartCoroutine(throwSun());
        //     isEvasion = false;
        // }

    }

    private void FixedUpdate()
    {
        Chase();
        Evasion();
    }

    IEnumerator Delay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }

    void turn()
    {
        if (isOKToTurn)
        {
            sprite.flipX = playerXpose <= 0 ? false : true;
        }
    }

    IEnumerator turnDelay(bool newFlip)
    {
        yield return Delay(0.2f);
        sprite.flipX = newFlip;
        rigid.AddForce(Vector2.right * playerXpose * 7f, ForceMode2D.Impulse);
    }

    void Chase()
    {
        /*
        조건: 
        1. 플레이어와의 격차가 많이 남 > 플랫폼 위인지 확인 후 플랫폼 떨어트리기 or 해던지기 or 대쉬 or 파트공격
        2. 플레이어와의 격차가 얼마 나지 않고, shrot보다 멀면 > 달려가기
        */
        if (currentDistance > LongDistance && !anim.GetBool("isAttacking") && !isEvasioning)
        {
            turn();
            Dash();
        }
        else
        {
            if (currentDistance > ShortDistance && !anim.GetBool("isAttacking") && !isEvasioning && !anim.GetBool("isThrowingSun") && !isDash)
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
                if (!isEvasioning && !isDash)
                {
                    anim.SetInteger("AnimState", 1);
                    rigid.velocity = new Vector2(0, rigid.velocity.y);
                }
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
        isDash = false;
    }

    void Evasion()
    {
        if (isOKToEvasion)
        {
            anim.SetBool("isAttacking", false);

            hitCount = 0;

            float dst = Random.Range(1, 3);
            Vector2 targetPosition = new Vector2(rigid.position.x + -playerXpose * dst, rigid.position.y);
            if (targetPosition.x < MapLeftLimit || targetPosition.x > MapRightLimit)
            {
                targetPosition = new Vector2(rigid.position.x + playerXpose * dst, rigid.position.y);
            }
            Jump(targetPosition);

            isEvasioning = true;
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
                throwSun();
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

    IEnumerator Hit()
    {
        Texture2D originTexture = sprite.sprite.texture;

        Texture2D texture = new Texture2D(sprite.sprite.texture.width, sprite.sprite.texture.height);

        // Set all pixels to white
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                texture.SetPixel(x, y, Color.white);
            }
        }
        texture.Apply();

        // Apply the modified texture to the sprite
        sprite.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);

        yield return Delay(0.5f);

        // sprite.sprite.texture = originTexture
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
