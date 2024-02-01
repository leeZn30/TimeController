using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    [Header("Inspector")]
    [SerializeField] float speed = 10f;
    [SerializeField] float maxSpeed = 30f;
    [SerializeField] float jumpPower = 5f;
    float damageTime = 0.3f;
    float jumpTimer = 0f;

    [Header("Prefabs")]
    [SerializeField] GameObject amingPfb;


    GameObject point;
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Vector2 hitposition;
    Vector2 hitBox = new Vector2(1f, 2);
    bool isSlow = false;
    bool isTeleport = false;
    bool isJumpping = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hitposition = new Vector2(rigid.position.x + transform.localScale.x, rigid.position.y);
    }

    private void Update()
    {
        movingSupport();
        attack();
        teleport();
        jump();
        hitboxFollowing();
    }

    private void FixedUpdate()
    {
        move();
        jumpping();
        land();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitposition, hitBox);
    }

    void move()
    {
        if (!isSlow)
        {
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h * speed, ForceMode2D.Impulse);

            // 최대 가속 지정
            if (Mathf.Abs(rigid.velocity.normalized.x) >= maxSpeed)
            {
                rigid.velocity = new Vector2(maxSpeed * h, rigid.velocity.y);
            }
        }
    }

    void movingSupport()
    {
        // 바로 멈추기
        if (Input.GetButtonUp("Horizontal"))
        {
            // rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
            rigid.velocity = new Vector2(0f, rigid.velocity.y);
        }

        // 플립
        if (Input.GetButton("Horizontal"))
        {
            sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Mathf.Abs(rigid.velocity.normalized.x) > 0) // 움직이고 있음
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("isJumpping"))
        {
            anim.SetBool("isJumpping", true);
            isJumpping = true;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }
    }

    // fixed
    void jumpping()
    {
        if (Input.GetButton("Jump") && isJumpping)
        {
            rigid.AddForce(Vector2.up, ForceMode2D.Impulse);
            jumpTimer += Time.fixedDeltaTime;
        }

        if (isJumpping && (Input.GetButtonUp("Jump") || jumpTimer > 1f)) // 1초 이상 점프하면
        {
            jumpTimer = 0f;
            isJumpping = false;
        }
    }

    void land()
    {
        if (rigid.velocity.normalized.y < 0 && anim.GetBool("isJumpping")) // 내려가고 있음
        {
            Debug.DrawRay(rigid.position, Vector2.down, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Map"));

            if (hit.collider != null && hit.collider.tag == "Ground")
            {
                jumpTimer = 0f;
                isJumpping = false;
                anim.SetBool("isJumpping", false);
            }
        }
    }

    void hitboxFollowing()
    {
        float direction;
        if (sprite.flipX)
            direction = -1f;
        else direction = 1f;

        hitposition = new Vector2(rigid.position.x + direction * transform.localScale.x, rigid.position.y);
    }

    void attack()
    {
        if (Input.GetMouseButtonDown(0) && !anim.GetBool("isAttacking") && !isTeleport && !anim.GetBool("isJumpping"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);

            StartCoroutine(detectCombo());
            anim.SetBool("isAttacking", true);

            Collider2D enemy = Physics2D.OverlapBox(hitposition, hitBox, 0, LayerMask.GetMask("Enemy"));
            if (enemy != null)
            {
                if (enemy.tag == "Enemy")
                {

                }
                else if (enemy.tag == "Bullet")
                {
                    enemy.GetComponent<ParringBullet>().parried();
                }
            }
        }
    }

    void comboAttack()
    {
        anim.SetTrigger("Combo1");

        if (sprite.flipX)
            transform.position += Vector3.left * 2;
        else
            transform.position += Vector3.right * 2;

        Collider2D enemy = Physics2D.OverlapBox(hitposition, hitBox, 0, LayerMask.GetMask("Enemy"));
        if (enemy != null)
        {
            if (enemy.tag == "Enemy")
            {

            }
            else if (enemy.tag == "Bullet")
            {
                enemy.GetComponent<ParringBullet>().parried();
            }
        }
    }

    IEnumerator detectCombo()
    {
        float limit = anim.GetCurrentAnimatorStateInfo(0).length; //animation 길이로 측정
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

    void teleport()
    {
        // 텔레포트 시작
        if (Input.GetMouseButtonDown(1) && !isTeleport)
        {
            isTeleport = true;
            Time.timeScale = 0.05f;
            point = Instantiate(amingPfb);
        }
        // 텔레포트 중단
        else if (Input.GetMouseButtonDown(1) && isTeleport)
        {
            isTeleport = false;
            Time.timeScale = 1f;
            Destroy(point);
        }
        // 텔레포트 완료
        else if (Input.GetMouseButtonDown(0) && isTeleport)
        {
            isTeleport = false;
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 점프계속 누르고 있는거 방지 (근데 굳이 필요 없을듯?)
            // rigid.velocity = Vector2.zero;
            // isJumpping = false;
            // anim.SetBool("isJumpping", false);

            Destroy(point);
            Time.timeScale = 1f;
        }

        if (isTeleport)
        {
            point.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Bullet")
        {
            OnDamaged(other.transform.position);
        }
    }


}
