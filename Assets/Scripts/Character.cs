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

    [Header("Prefabs")]
    [SerializeField] GameObject amingPfb;


    GameObject point;
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Vector2 hitposition;
    Vector2 hitBox = new Vector2(2, 2);
    bool isSlow = false;
    bool isTeleport = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hitposition = new Vector2(rigid.position.x + transform.localScale.x / 2, rigid.position.y);
    }

    private void Update()
    {
        movingSupport();
        attack();
        teleport();
        jump();
    }

    private void FixedUpdate()
    {
        move();
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
            if (Mathf.Abs(rigid.velocity.normalized.x) > maxSpeed)
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
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        // 플립
        if (Input.GetButton("Horizontal"))
            sprite.flipX = Input.GetAxisRaw("Horizontal") == -1;

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
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
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
                // isJump = false;
                anim.SetBool("isJumpping", false);
            }
        }
    }

    void attack()
    {
        if (sprite.flipX)
            hitposition = new Vector2(rigid.position.x - transform.localScale.x / 2, rigid.position.y);
        else
            hitposition = new Vector2(rigid.position.x + transform.localScale.x / 2, rigid.position.y);


        if (Input.GetMouseButtonDown(0) && !anim.GetBool("isAttacking") && !isTeleport)
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
        if (Input.GetMouseButtonDown(1) && !isTeleport)
        {
            isTeleport = true;
            point = Instantiate(amingPfb);
            // Debug.Log("Teleport Start!");
        }
        else if (Input.GetMouseButtonDown(1) && isTeleport)
        {
            isTeleport = false;
            Destroy(point);
            // Debug.Log("Teleport Cancel!");
        }
        else if (Input.GetMouseButtonDown(0) && isTeleport)
        {
            isTeleport = false;
            transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Destroy(point);
            // Debug.Log("Teleport complete!");
        }

        if (isTeleport)
        {
            point.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }


}
