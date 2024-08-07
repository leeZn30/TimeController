using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

using CC = CinemachineCamera;
using System.Linq;
using Unity.VisualScripting;

public class Character : Singleton<Character>
{

    // **************** 캐릭터 데이터 *****************
    [Header("Character Data")]
    public float Hp;
    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float jumpPower;          // 점프 힘
    public float Atk = 10f;
    float invincibilityTime = 0.5f;
    float maxSightRange = 5f;
    float trailSpawnTime = 0.05f; // 스프라이트 생성 간격
    float trailTimer = 0f;

    // **************** 스킬 상태 ******************
    bool TeleportActive => GameData.TeleportActive;
    Slider TeleportGauge;
    float teleportChargeSpeed => GameData.TeleportChargeSpeed;
    bool RewindActive => GameData.RewindActive;
    bool SlowActive => GameData.SlowActive;
    Slider SlowGauge;
    [SerializeField] float slowChargeSpeed => GameData.SlowChargeSpeed;

    // **************** 변수 *************
    public bool isMovable = true;
    bool isSlow = false;
    bool isTeleport = false;
    bool isRewind = false;
    bool isLooking = false;
    bool isJumping = false;
    float jumpTime = 0f;
    int jumpCnt = 0;
    float maxJumpTime = 1f;         // 최대 점프 시간
    float jumpTimeMultiplier = 7f;  // 점프 시간에 대한 곱셈 계수
    public Queue<Trail> TrailQueue;
    Vector3 platformDistance;

    // *************** 상호작용 박스 *************
    Vector2 hitPosition;
    Vector2 hitBox = new Vector2(1f, 2);
    Vector2 parryingPosition;
    Vector2 parryingBox = new Vector2(1.3f, 2.2f); //1.3f, 2.2f

    // **************** 프리팹 ********************
    [Header("Prefabs")]
    [SerializeField] GameObject amingPfb;
    [SerializeField] GameObject draggingPfb;
    [SerializeField] Trail trailPrefab;

    // ************* 컴포넌트&오브젝트 *************
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Background background;
    Parriable bullet;
    [SerializeField] GameObject platform;
    // ************* 그 외 *************

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        background = FindObjectOfType<Background>();
        TeleportGauge = GameObject.Find("TeleportGauge").GetComponent<Slider>();
        SlowGauge = GameObject.Find("SlowGauge").GetComponent<Slider>();

        hitPosition = new Vector2(rigid.position.x + transform.localScale.x, rigid.position.y);
        parryingPosition = rigid.position;

        TrailQueue = ObjectPool.CreateQueue<Trail>(5, trailPrefab);

        Init();
    }

    private void Update()
    {
        if (isMovable)
        {
            jump();
            land();
            attack();
            sightMove();
            extraMove();
            parry();
            hitPosition = new Vector2(rigid.position.x + (sprite.flipX ? -1 : 1) * transform.localScale.x, rigid.position.y);
            parryingPosition = rigid.position;

            // 스킬
            chargeSkill();
            teleport();
            Rewind();
            OnSlow();
            SlowRun();
        }
    }

    private void FixedUpdate()
    {
        if (isMovable)
        {
            move();
            jumping();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitPosition, hitBox);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryingPosition, parryingBox);
    }

    void Init()
    {
        Hp = GameData.Hp;

        if (!TeleportActive)
            TeleportGauge.gameObject.SetActive(false);
        if (!SlowActive)
            SlowGauge.gameObject.SetActive(false);
    }

    void move()
    {
        if (!isSlow)
        {
            if (Input.GetButton("Horizontal"))
            {
                sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
                anim.SetBool("isRunning", true);

                float h = Input.GetAxisRaw("Horizontal");
                rigid.AddForce(Vector2.right * h * speed, ForceMode2D.Impulse);

                background.BackgroundScroll(h);

                // 최대 가속 지정
                if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
                {
                    rigid.velocity = new Vector2(maxSpeed * h, rigid.velocity.y);
                }

                if (platform != null)
                    platformDistance = platform.transform.position - transform.position;
            }
            else
            {
                if (platform != null)
                    rigid.position = platform.transform.position - platformDistance;
            }
        }
    }

    void extraMove()
    {
        // 바로 멈추기
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0f, rigid.velocity.y);
            anim.SetBool("isRunning", false);
        }
    }

    void jump()
    {
        if (!isSlow)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SoundManager.Instance.PlaySFX(AudioType.Character, "Jump");
                if (!anim.GetBool("isJumpping"))
                {
                    isJumping = true;
                    anim.SetBool("isJumpping", true);
                    jumpCnt++;
                    rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                }
                else
                {
                    if (jumpCnt < 2)
                    {
                        jumpCnt++;
                        rigid.velocity = new Vector2(rigid.velocity.x, 0f);
                        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
                    }
                }
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                jumpTime = 0f;
            }
        }
    }

    void jumping()
    {
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTime < maxJumpTime)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, jumpPower);
                jumpTime += Time.deltaTime * jumpTimeMultiplier;
            }
            else
            {
                isJumping = false;
            }
        }
    }

    void land()
    {
        if (rigid.velocity.normalized.y <= 0 && anim.GetBool("isJumpping")) // 내려가고 있음
        {
            if (Mathf.Abs(rigid.velocity.y) > 20)
                rigid.velocity = new Vector2(rigid.velocity.x, -30);

            Debug.DrawRay(rigid.position, Vector2.down, Color.red);
            List<RaycastHit2D> hit = Physics2D.RaycastAll(rigid.position, Vector2.down, 1f).ToList();

            if (hit.Count > 0 && hit.Find(e => e.collider.CompareTag("Ground") || e.collider.CompareTag("Platform")))
            {
                jumpTime = 0f;
                isJumping = false;
                jumpCnt = 0;

                anim.SetBool("isJumpping", false);
            }
        }
    }

    void sightMove()
    {
        if (!isSlow)
        {
            if (Input.GetButtonDown("Vertical"))
            {
                CC.Instance.Follow = null;
                isLooking = true;
            }

            if (Input.GetButtonUp("Vertical"))
            {
                isLooking = false;
                CC.Instance.Follow = transform;
            }

            if (isLooking)
            {
                float v = Input.GetAxisRaw("Vertical");
                if (v == 1)
                {
                    CC.Instance.transform.Translate(Vector3.up * 15f * Time.unscaledDeltaTime);

                    if (CC.Instance.transform.position.y > transform.position.y + maxSightRange)
                    {
                        CC.Instance.transform.position = new Vector3(CC.Instance.transform.position.x, transform.position.y + maxSightRange, CC.Instance.transform.position.z);
                    }
                }
                else if (v == -1)
                {
                    CC.Instance.transform.Translate(Vector3.down * 15 * Time.unscaledDeltaTime);

                    if (CC.Instance.transform.position.y < transform.position.y - maxSightRange)
                    {
                        CC.Instance.transform.position = new Vector3(CC.Instance.transform.position.x, transform.position.y - maxSightRange, CC.Instance.transform.position.z);
                    }
                }
            }
        }
    }

    void attack()
    {
        if (!isRewind && !isSlow && gameObject.layer != LayerMask.NameToLayer("DamagedPlayer"))
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!anim.GetBool("isAttacking") && !anim.GetBool("isJumpping") && !isTeleport)
                {
                    SoundManager.Instance.PlaySFX(AudioType.Character, "Attack1");

                    rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

                    anim.SetBool("isAttacking", true);
                    StartCoroutine(detectCombo());

                    int layerMask = 1 << LayerMask.NameToLayer("Enemy");
                    Collider2D[] enemies = Physics2D.OverlapBoxAll(hitPosition, hitBox, 0, layerMask);
                    if (enemies.Length > 0)
                    {
                        foreach (Collider2D enemy in enemies)
                        {
                            if (enemy.tag == "Enemy")
                            {
                                enemy.GetComponent<Enemy>().OnDamaged(Atk, DamageType.Player);
                            }
                            else
                            {
                                Destroy(enemy.gameObject);
                            }
                        }
                    }
                }
            }
        }
    }

    void comboAttack()
    {
        anim.SetTrigger("Combo1");
        SoundManager.Instance.PlaySFX(AudioType.Character, "Attack2");

        Collider2D enemy = Physics2D.OverlapBox(hitPosition, hitBox, 0, LayerMask.GetMask("Enemy"));
        if (enemy != null)
        {
            if (enemy.tag == "Enemy")
            {
                enemy.GetComponent<Enemy>().OnDamaged(Atk, DamageType.Player);
            }
        }
    }

    IEnumerator detectCombo()
    {
        // 앞에 다른 애니메이션이 들어오지 않게 한 프레임 쉬기
        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        float limit = info.length; //animation 길이로 측정
        float duration = 0f;
        float comboStart = limit * 0.6f; // 비율

        while (duration <= limit)
        {
            duration += Time.deltaTime;

            if (comboStart <= duration && duration <= limit)
            {
                if (Input.GetMouseButtonDown(0) && anim.GetBool("isAttacking"))
                {
                    comboAttack();
                }
            }
            yield return null;
        }

        yield return new WaitForEndOfFrame();
        anim.SetBool("isAttacking", false);
    }

    void parry()
    {
        if (!isTeleport && !isSlow)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Collider2D bullet = Physics2D.OverlapBox(parryingPosition, parryingBox, 0, LayerMask.GetMask("Bullet"));

                if (bullet != null)
                {
                    Parriable p = bullet.GetComponent<Parriable>();
                    if (bullet.tag == "Bullet" && p.enabled)
                    {
                        Time.timeScale = 0f;
                        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                        this.bullet = p;
                        PostPrecessingController.Instance.CallParryStartEffect();
                        CC.Instance.Zoom(1.5f, 0.8f);
                        SoundManager.Instance.PlaySFX(AudioType.Character, "Parrying");
                        anim.SetTrigger("Parry");
                    }
                }
            }
        }
    }

    void ParryAnimationEvent()
    {
        anim.updateMode = AnimatorUpdateMode.Normal;
        bullet.parried();
    }

    public void OnDamaged(Vector2 targetPos, float Damage)
    {
        // 다시 슬로우 걸 수 있을때까지 딜레이 줘야함
        // isSlowable = false;
        // isSlow = false;
        // Time.timeScale = 1f;
        // anim.updateMode = AnimatorUpdateMode.Normal; // 슬로우한 상태로 패링하면 풀릴 수 있음
        // rigid.gravityScale = 7f;

        HeartManager.Instance.calculateHeart(Damage);
        Hp -= Damage;
        if (Hp <= 0)
            Dead();

        // 넉백
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        // 무적
        gameObject.layer = LayerMask.NameToLayer("DamagedPlayer");
        sprite.color = new Color(1, 1, 1, 0.5f);

        Invoke("OffDamaged", invincibilityTime);
    }

    // 무적 해제
    void OffDamaged()
    {
        // isSlowable = true;
        gameObject.layer = LayerMask.NameToLayer("Player");
        sprite.color = Color.white;
    }

    void Dead()
    {
        anim.SetTrigger("Dead");
    }

    void DeadAnimationEvent()
    {
        GameManager.Instance.Failed();
        Destroy(gameObject);
    }

    public void activeSkill(string skill)
    {
        switch (skill)
        {
            case "Teleport":
                TeleportGauge.gameObject.SetActive(true);
                break;

            case "Slow":
                SlowGauge.gameObject.SetActive(true);
                break;

            default:
                break;
        }
    }


    #region 캐릭터 스킬
    void chargeSkill()
    {
        if (TeleportGauge.value < TeleportGauge.maxValue)
            TeleportGauge.value += Time.unscaledDeltaTime * teleportChargeSpeed;

        if (!isSlow && SlowGauge.value < SlowGauge.maxValue)
            SlowGauge.value += Time.unscaledDeltaTime * slowChargeSpeed;
    }

    public void FullChargeGauge(string skill)
    {
        switch (skill)
        {
            case "Teleport":
                TeleportGauge.value = TeleportGauge.maxValue;
                break;

            default:
                break;
        }
    }

    void teleport()
    {
        if (TeleportActive && TeleportGauge.value == TeleportGauge.maxValue)
        {
            // 텔레포트 시작
            if (Input.GetKeyDown(KeyCode.Q) && !isTeleport && !isRewind)
            {
                isTeleport = true;
                Instantiate(amingPfb, (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition), Quaternion.identity);
            }
        }
    }

    public void StopTeleport()
    {
        isTeleport = false;
        Time.timeScale = 1f;
    }

    public void FinishTeleport()
    {
        isTeleport = false;
        Time.timeScale = 1f;

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 가속도 쌓이는 거 방지
        rigid.velocity = new Vector2(0, 0);

        TeleportGauge.value = 0f;
    }


    void Rewind()
    {
        if (RewindActive)
        {
            if (Input.GetMouseButtonDown(1) && !isRewind && !isTeleport)
            {
                isRewind = true;
                Instantiate(draggingPfb);
                // Instantiate(draggingPfb, GameObject.Find("Canvas").transform);
            }
        }
    }

    public void FinishRewind()
    {
        isRewind = false;
    }

    void OnSlow()
    {
        if (SlowActive)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isSlow = true;
                anim.SetBool("isSlow", true);
                Time.timeScale = 0.1f;
                // 애니메이션
                anim.updateMode = AnimatorUpdateMode.UnscaledTime;
                rigid.gravityScale = 0f;
                rigid.velocity = Vector2.zero;
                isJumping = false;
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift) && anim.GetBool("isSlow"))
            {
                isSlow = false;
                anim.SetBool("isSlow", false);
                Time.timeScale = 1f;
                anim.updateMode = AnimatorUpdateMode.Normal; // 슬로우한 상태로 패링하면 풀릴 수 있음
                rigid.gravityScale = 7f;
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && !anim.GetBool("isSlow"))
            {
                anim.SetBool("isSlow", true);
            }
        }
    }

    void SlowRun()
    {
        if (isSlow && SlowGauge.value > 0)
        {
            SlowGauge.value -= Time.unscaledDeltaTime * slowChargeSpeed;
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {

                sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;

                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                transform.Translate(Vector2.right * h * speed * Time.unscaledDeltaTime);
                transform.position += Vector3.up * v * speed * Time.unscaledDeltaTime;

                background.BackgroundScroll(h);

                afterImage();
            }
        }
    }

    void afterImage()
    {
        trailTimer += Time.unscaledDeltaTime;

        // 일정 간격으로 스프라이트 생성
        if (trailTimer >= trailSpawnTime)
        {
            // Trail trail = Instantiate<Trail>(trailPrefab, transform.position, Quaternion.identity);
            Trail trail = TrailQueue.Dequeue();
            trail.transform.position = transform.position;
            trail.gameObject.SetActive(true);
            trail.sprite.sprite = sprite.sprite;
            trail.sprite.flipX = sprite.flipX;
            trailTimer = 0f;
        }
    }


    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            Enemy e = other.gameObject.GetComponent<Enemy>();
            if (e != null)
                OnDamaged(other.transform.position, e.atk);
        }

        if (other.gameObject.CompareTag("Platform"))
        {
            platform = other.gameObject;
            platformDistance = platform.transform.position - transform.position;
        }
    }

    void OnCollisionExit2D(Collider2D other)
    {
        if (other.CompareTag("Platform"))
        {
            platform = null;
            platformDistance = Vector3.zero;
        }
    }

    // 충돌 감지 + 물리적 영향X
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Trap"))
        {
            Dead();
        }

        if (other.CompareTag("Platform"))
        {
            platform = other.gameObject;
            platformDistance = platform.transform.position - transform.position;
        }
    }


    #region 더미 메서드
    // 슬로우시 모든 행동은 그대로 유지하는 것 일단 남겨는 둠(미완성)
    // void SlowMove()
    // {
    //     if (isSlow)
    //     {
    //         if (Input.GetButton("Horizontal"))
    //         {
    //             sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
    //             anim.SetBool("isRunning", true);

    //             float h = Input.GetAxisRaw("Horizontal");
    //             // rigid.AddForce(Vector2.right * h * speed, ForceMode2D.Impulse);
    //             transform.Translate(Vector2.right * h * speed * Time.unscaledDeltaTime);

    //             background.BackgroundScroll(h);

    //             // 최대 가속 지정
    //             // if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
    //             // {
    //             //     rigid.velocity = new Vector2(maxSpeed * h, rigid.velocity.y);
    //             // }
    //         }
    //     }
    // }

    // void SlowJump()
    // {
    //     if (isSlow)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Space))
    //         {
    //             if (!anim.GetBool("isJumpping"))
    //             {
    //                 Debug.Log("~~~");
    //                 isJumping = true;
    //                 anim.SetBool("isJumpping", true);
    //                 jumpCnt++;
    //                 // rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    //                 transform.position += Vector3.up * jumpPower * 10f * Time.unscaledDeltaTime;
    //             }
    //             else
    //             {
    //                 if (jumpCnt < 2)
    //                 {
    //                     jumpCnt++;
    //                     rigid.velocity = new Vector2(rigid.velocity.x, 0f);
    //                     // rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    //                     transform.position += Vector3.up * jumpPower * 10f * Time.unscaledDeltaTime;
    //                 }
    //             }
    //         }
    //         if (Input.GetKeyUp(KeyCode.Space))
    //         {
    //             isJumping = false;
    //             jumpTime = 0f;
    //         }
    //     }
    // }

    // 사다리 너무 어려워서 일단 제외
    /*
    /* bool isLabber = false;
    bool isClimbing = false;
    void climb()
    {
        if (isLabber && Input.GetButtonDown("Vertical"))
        {
            isClimbing = true;
        }
    }

    void climbimg()
    {
        if (isClimbing)
        {
            Physics2D.IgnoreCollision(collider, groundCollider, true);
            rigid.gravityScale = 0f;
            rigid.velocity = new Vector2(rigid.velocity.x, speed * Input.GetAxisRaw("Vertical"));

            if (rigid.velocity.normalized.y < 0) // 내려가고 있음
            {
                Debug.DrawRay(rigid.position, Vector2.down, Color.red);
                RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Map"));

                if (hit.collider != null && hit.collider.tag == "Ground")
                {
                    Physics2D.IgnoreCollision(collider, groundCollider, false);
                    Debug.Log("Ground");
                }
            }
        }
    }
    */
    #endregion

}
