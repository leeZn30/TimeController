using System.Collections;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using CC = CinemachineCamera;

public class Character : Singleton<Character>
{
    // **************** 캐릭터 데이터 *****************
    [Header("Character Data")]
    [SerializeField] float hp;
    [SerializeField] float speed;
    [SerializeField] float maxSpeed;
    [SerializeField] float jumpPower;          // 점프 힘
    [SerializeField] float Atk = 10f;
    float damageTime = 0.3f;
    float maxSightRange = 5f;
    float slowDelay = 1f;
    float trailSpawnTime = 0.05f; // 스프라이트 생성 간격
    float trailTimer = 0f;

    // **************** 스킬 상태 ******************
    [Header("스킬")]
    [SerializeField] bool TeleportActive = false;
    [SerializeField] float teleportGauge;
    [SerializeField] float maxTeleportGauge;
    [SerializeField] bool RewindActive = false;
    [SerializeField] float RewindGauge;
    [SerializeField] float maxRewindGauge;
    [SerializeField] bool SlowActive = false;

    // **************** 변수 *************
    // bool isSlowable = true;
    bool isSlow = false;
    bool isTeleport = false;
    bool isLooking = false;
    bool isJumping = false;
    float jumpTime = 0f;
    int jumpCnt = 0;
    float maxJumpTime = 1f;         // 최대 점프 시간
    float jumpTimeMultiplier = 7f;  // 점프 시간에 대한 곱셈 계수
    Vector3 sightRange;

    // *************** 상호작용 박스 *************
    Vector2 hitPosition;
    Vector2 hitBox = new Vector2(1f, 2);
    Vector2 parryingPosition;
    Vector2 parryingBox = new Vector2(0.3f, 1); // default = 0.3f

    // **************** 프리팹 ********************
    [Header("Prefabs")]
    [SerializeField] GameObject amingPfb;
    [SerializeField] Trail trailPrefab; // 사용할 스프라이트 프리팹

    // ************* 컴포넌트&오브젝트 *************
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    GameObject teleportPointer;
    GameObject rewindPointer;
    Background background;
    Parriable bullet;

    // ************* 그 외 *************

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        background = FindObjectOfType<Background>();

        hitPosition = new Vector2(rigid.position.x + transform.localScale.x, rigid.position.y);
        parryingPosition = new Vector2(rigid.position.x + transform.localScale.x * 0.65f, rigid.position.y);
    }

    private void Update()
    {
        jump();
        attack();
        sightMove();
        extraMove();
        parry();
        hitPosition = new Vector2(rigid.position.x + (sprite.flipX ? -1 : 1) * transform.localScale.x, rigid.position.y);
        parryingPosition = new Vector2(rigid.position.x + transform.localScale.x * 0.65f, rigid.position.y);

        // 스킬
        teleport();
        OnSlow();
        SlowRun();
    }

    private void FixedUpdate()
    {
        move();
        jumping();
        land();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitPosition, hitBox);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(parryingPosition, parryingBox);
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
            }
        }
    }

    void extraMove()
    {
        // 바로 멈추기
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            anim.SetBool("isRunning", false);
        }
    }

    void jump()
    {
        if (!isSlow)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
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
            Debug.DrawRay(rigid.position, Vector2.down, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Map"));

            if (hit.collider != null && hit.collider.tag.Equals("Ground"))
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
                if (Input.GetAxisRaw("Vertical") == 1)
                    sightRange = new Vector3(CC.Instance.transform.position.x, transform.position.y + maxSightRange, CC.Instance.transform.position.z);
                else
                    sightRange = new Vector3(CC.Instance.transform.position.x, transform.position.y - maxSightRange, CC.Instance.transform.position.z);
                isLooking = true;
                CC.Instance.Follow = null;
            }

            if (Input.GetButtonUp("Vertical"))
            {
                sightRange = Vector3.zero;
                isLooking = false;
                CC.Instance.Follow = transform;
            }

            if (isLooking)
            {
                float v = Input.GetAxisRaw("Vertical");
                if (v == 1)
                {
                    CC.Instance.transform.Translate(Vector3.up * 15 * Time.unscaledDeltaTime);
                    if (CC.Instance.transform.position.y >= sightRange.y)
                    {
                        CC.Instance.transform.position = sightRange;
                    }
                }
                else if (v == -1)
                {
                    CC.Instance.transform.Translate(Vector3.down * 15 * Time.unscaledDeltaTime);
                    if (CC.Instance.transform.position.y <= sightRange.y)
                    {
                        CC.Instance.transform.position = sightRange;
                    }
                }
            }
        }
    }

    void attack()
    {
        if (!isSlow)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // attack 하면 안되는 조건 
                if (!anim.GetBool("isAttacking") && !anim.GetBool("isJumpping") && !isTeleport)
                {
                    rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

                    anim.SetBool("isAttacking", true);
                    StartCoroutine(detectCombo());

                    Collider2D enemy = Physics2D.OverlapBox(hitPosition, hitBox, 0, LayerMask.GetMask("Enemy"));
                    if (enemy != null)
                    {
                        if (enemy.tag == "Enemy")
                        {
                            enemy.GetComponent<Enemy>().OnDamaged(Atk);
                        }
                    }
                }
            }
        }
    }

    void comboAttack()
    {
        anim.SetTrigger("Combo1");

        Collider2D enemy = Physics2D.OverlapBox(hitPosition, hitBox, 0, LayerMask.GetMask("Enemy"));
        if (enemy != null)
        {
            if (enemy.tag == "Enemy")
            {
                enemy.GetComponent<Enemy>().OnDamaged(Atk);
            }
        }
    }

    IEnumerator detectCombo()
    {
        // 앞에 다른 애니메이션이 들어오지 않게 한 프레임 쉬기
        yield return null;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        // Debug.LogFormat("[{0}]: {1}", info.IsName("Attack1") ? "Attack1" : info.shortNameHash.ToString(), info.length);

        float limit = info.length; //animation 길이로 측정
        float duration = 0f;
        float comboStart = limit * 0.7f; // 비율

        while (duration <= limit)
        {
            duration += Time.deltaTime;

            if (comboStart <= duration && duration <= limit)
            {
                if (Input.GetMouseButtonDown(0) && anim.GetBool("isAttacking"))
                {
                    // Debug.Log("Combo Enter");
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
                    if (bullet.tag == "Bullet")
                    {
                        Time.timeScale = 0f;
                        this.bullet = bullet.GetComponent<Parriable>();
                        this.bullet.stopBullet();
                        StartCoroutine(CharacterZoom());
                    }
                }
            }
        }
    }

    IEnumerator CharacterZoom()
    {
        yield return null;

        Vignette vignette;
        FindObjectOfType<Volume>().profile.TryGet(out vignette);
        CC.Instance.ChangeSoftZone(new Vector2(0, 0));

        while (CC.Instance.OrthographicSize > 1.5f)
        {
            Debug.Log(CC.Instance.OrthographicSize);
            CC.Instance.OrthographicSize -= Time.unscaledDeltaTime * 20f;
            float newIntensity = Mathf.Clamp(vignette.intensity.value + 3 * Time.unscaledDeltaTime, 0f, 0.5f);
            vignette.intensity.value = newIntensity;

            yield return null;
        }

        anim.updateMode = AnimatorUpdateMode.UnscaledTime;
        anim.SetTrigger("Parry");
    }

    public void ParryAnimationEvent()
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

        // 무적
        gameObject.layer = 9;
        sprite.color = new Color(1, 1, 1, 0.5f);

        // 넉백
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        Invoke("OffDamaged", damageTime);
    }

    // 무적 해제
    void OffDamaged()
    {
        // isSlowable = true;
        gameObject.layer = 8;
        sprite.color = Color.white;
    }

    public void activeSkill(string skill)
    {
        switch (skill)
        {
            case "Teleport":
                TeleportActive = true;
                break;

            case "Rewind":
                RewindActive = true;
                break;

            case "Slow":
                SlowActive = true;
                break;

            default:
                break;
        }
    }


    #region 캐릭터 스킬
    void teleport()
    {
        if (TeleportActive)
        {
            // 텔레포트 시작
            if (Input.GetKeyDown(KeyCode.Q) && !isTeleport)
            {
                isTeleport = true;
                Time.timeScale = 0.05f;
                teleportPointer = Instantiate(amingPfb);
            }
            // 텔레포트 중단
            else if (Input.GetMouseButtonDown(1) && isTeleport)
            {
                isTeleport = false;
                Time.timeScale = 1f;
                Destroy(teleportPointer);
            }
            // 텔레포트 완료
            else if (Input.GetMouseButtonDown(0) && isTeleport)
            {
                isTeleport = false;
                transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Destroy(teleportPointer);
                Time.timeScale = 1f;
            }

            if (isTeleport)
            {
                teleportPointer.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }

        }
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
            else
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
        if (isSlow)
        {
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
            Trail trail = Instantiate<Trail>(trailPrefab, transform.position, Quaternion.identity);
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
            OnDamaged(other.transform.position, other.gameObject.GetComponent<Enemy>().atk);
        }
    }

    // 충돌 감지 + 물리적 영향X
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Trap")
        {
            OnDamaged(other.transform.position, 0);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
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
