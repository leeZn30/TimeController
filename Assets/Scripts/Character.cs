using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Character : MonoBehaviour
{
    [Header("Inspector")]
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpPower = 5f;

    [Header("Prefabs")]
    [SerializeField] GameObject amingPfb;

    GameObject point;
    bool isTeleport = false;
    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Vector2 hitposition;
    Vector2 hitBox = new Vector2(2, 2);

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        hitposition = new Vector2(rigid.position.x + transform.localScale.x / 2, rigid.position.y);
    }

    private void Update()
    {
        move();
        attack();
        teleport();
    }

    private void FixedUpdate()
    {
        land();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(hitposition, hitBox);
    }

    void move()
    {
        if (Input.GetKey(KeyCode.D))
        {
            sprite.flipX = false;
            transform.Translate(new Vector3(1.0f, 0, 0) * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            sprite.flipX = true;
            transform.Translate(new Vector3(-1.0f, 0, 0) * speed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("isJumpping"))
        {
            // isJump = true;
            anim.SetBool("isJumpping", true);
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
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

        float moveDistance = 15f;
        if (sprite.flipX)
            transform.Translate(Vector2.left * speed * moveDistance * Time.deltaTime);
        else
            transform.Translate(Vector2.right * speed * moveDistance * Time.deltaTime);

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


    void land()
    {
        if (rigid.velocity.y < 0 && anim.GetBool("isJumpping")) // 내려가고 있음
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
}
