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

    Animator anim;
    Rigidbody2D rigid;
    SpriteRenderer sprite;
    Vector3 hitposition;
    Vector2 hitBox = new Vector2(1, 2);

    public bool isJump = false;

    private void Awake()
    {
        hitposition = new Vector3(transform.position.x + transform.localScale.x / 2, transform.position.y, transform.position.z);

        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        move();
        land();
        attack();

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

        if (Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            isJump = true;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
        }

    }

    void attack()
    {
        if (sprite.flipX)
            hitposition = new Vector3(transform.position.x - transform.localScale.x / 2, transform.position.y, transform.position.z);
        else
            hitposition = new Vector3(transform.position.x + transform.localScale.x / 2, transform.position.y, transform.position.z);

        if (Input.GetMouseButtonDown(0))
        {
            anim.SetTrigger("Attack");

            Collider2D enemy = Physics2D.OverlapBox(hitposition, hitBox, 0, LayerMask.GetMask("Enemy"));
            if (enemy != null)
                Debug.Log(enemy.name);
        }
    }

    void land()
    {
        if (rigid.velocity.y < 0 && isJump) // 내려가고 있음
        {
            Debug.DrawRay(rigid.position, Vector2.down, Color.red);
            RaycastHit2D hit = Physics2D.Raycast(rigid.position, Vector2.down, 1f, LayerMask.GetMask("Map"));

            if (hit.collider != null && hit.collider.tag == "Ground")
            {
                isJump = false;
            }
        }
    }
}
