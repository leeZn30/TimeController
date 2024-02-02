using System.Collections;
using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using System;
using UnityEditor;
using Unity.VisualScripting;

public class Character : MonoBehaviour
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

    // **************** 스킬 상태 ******************
    [Header("스킬")]
    [SerializeField] bool TeleportActive = false;
    [SerializeField] float teleportGauge;
    [SerializeField] float maxTeleportGauge;
    [SerializeField] bool RewindActive = false;
    [SerializeField] float RewindGauge;
    [SerializeField] float maxRewindGauge;
    [SerializeField] bool SlowActive = false;

    // **************** 상태 확인 변수 *************
    bool isTeleport = false;
    bool isLooking = false;
    bool isJumping = false;
    float jumpTime = 0f;
    int jumpCnt = 0;
    float maxJumpTime = 1f;         // 최대 점프 시간
    float jumpTimeMultiplier = 7f;  // 점프 시간에 대한 곱셈 계수


    // **************** 프리팹 ********************
    [Header("Prefabs")]
    [SerializeField] GameObject amingPfb;

    // ************* 컴포넌트&오브젝트 *************
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    CinemachineVirtualCamera camera;
    GameObject teleportPointer;
    GameObject rewindPointer;

    // ************* 그 외 *************
    Vector3 sightRange;
    Vector2 hitposition;
    Vector2 hitBox = new Vector2(1f, 2);

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        camera = FindObjectOfType<CinemachineVirtualCamera>();

        hitposition = new Vector2(rigid.position.x + transform.localScale.x, rigid.position.y);
    }

    private void Update()
    {
        jump();
        attack();
        sightMove();
        extraMove();
        hitposition = new Vector2(rigid.position.x + (sprite.flipX ? -1 : 1) * transform.localScale.x, rigid.position.y);

        // 스킬
        if (TeleportActive)
            teleport();
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
        Gizmos.DrawWireCube(hitposition, hitBox);
    }

    void move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h * speed, ForceMode2D.Impulse);

        // 최대 가속 지정
        if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed * h, rigid.velocity.y);
        }
    }

    void extraMove()
    {
        // 바로 멈추기
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            // anim.SetBool("isRunning", false);
        }

        // 플립
        if (Input.GetButton("Horizontal"))
        {
            sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    void jump()
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

            if (hit.collider != null && hit.collider.tag == "Ground")
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
        if (Input.GetButtonDown("Vertical"))
        {
            if (Input.GetAxisRaw("Vertical") == 1)
                sightRange = new Vector3(camera.transform.position.x, transform.position.y + maxSightRange, camera.transform.position.z);
            else
                sightRange = new Vector3(camera.transform.position.x, transform.position.y - maxSightRange, camera.transform.position.z);
            isLooking = true;
            camera.Follow = null;
        }

        if (Input.GetButtonUp("Vertical"))
        {
            sightRange = Vector3.zero;
            isLooking = false;
            camera.Follow = transform;
        }

        if (isLooking)
        {
            float v = Input.GetAxisRaw("Vertical");
            if (v == 1)
            {
                if (camera.transform.position.y > sightRange.y)
                {
                    camera.transform.position = sightRange;
                }
                camera.transform.Translate(Vector3.up * 15 * Time.unscaledDeltaTime);
            }
            else if (v == -1)
            {
                if (camera.transform.position.y < sightRange.y)
                {
                    camera.transform.position = sightRange;
                }
                camera.transform.Translate(Vector3.down * 15 * Time.unscaledDeltaTime);
            }
        }
    }

    void attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // attack 하면 안되는 조건 
            if (!anim.GetBool("isAttacking") && !anim.GetBool("isJumpping") && !isTeleport)
            {
                rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

                anim.SetBool("isAttacking", true);
                StartCoroutine(detectCombo());

                Collider2D enemy = Physics2D.OverlapBox(hitposition, hitBox, 0, LayerMask.GetMask("Enemy"));
                if (enemy != null)
                {
                    if (enemy.tag == "Enemy")
                    {
                        enemy.GetComponent<Enemy>().OnDamaged(Atk);
                    }
                    else if (enemy.tag == "Bullet")
                    {
                        enemy.GetComponent<ParringBullet>().parried();
                    }
                }
            }
        }
    }

    void comboAttack()
    {
        anim.SetTrigger("Combo1");

        Collider2D enemy = Physics2D.OverlapBox(hitposition, hitBox, 0, LayerMask.GetMask("Enemy"));
        if (enemy != null)
        {
            if (enemy.tag == "Enemy")
            {
                enemy.GetComponent<Enemy>().OnDamaged(Atk);
            }
            else if (enemy.tag == "Bullet")
            {
                enemy.GetComponent<ParringBullet>().parried();
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
    void OnDamaged(Vector2 targetPos)
    {
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

            default:
                break;
        }
    }


    #region 캐릭터 스킬
    void teleport()
    {
        // 텔레포트 시작
        if (Input.GetMouseButtonDown(1) && !isTeleport)
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

    #endregion

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Bullet")
        {
            OnDamaged(other.transform.position);
        }
    }

    // 충돌 감지 + 물리적 영향X
    private void OnTriggerEnter2D(Collider2D other)
    {
    }

    private void OnTriggerExit2D(Collider2D other)
    {
    }


    #region 더미 메서드
    // bool isJumpping = false;
    /* 이하 체공점프인데 좀 이상하게 되는 것 같아서 일단 삭제
    // void jump()
    // {
    //     if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumpping"))
    //     {
    //         anim.SetBool("isJumpping", true);
    //         isJumpping = true;
    //         rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    //     }
    // }

    // public float maxJump;
    // // fixed
    // void jumpping()
    // {
    //     if (Input.GetButton("Jump") && isJumpping)
    //     {
    //         rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

    //         if (rigid.velocity.y > maxJump)
    //         {
    //             rigid.velocity = new Vector2(rigid.velocity.x, maxJump);
    //         }
    //         jumpTimer += Time.fixedDeltaTime;
    //     }

    //     if (isJumpping && (Input.GetButtonUp("Jump") || jumpTimer > 1f)) // 1초 이상 점프하면
    //     {
    //         jumpTimer = 0f;
    //         isJumpping = false;
    //     }
    // }
    */

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
