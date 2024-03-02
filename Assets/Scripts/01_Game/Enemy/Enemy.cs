using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyData enemyData;

    public float hp;
    public float atk;

    protected Animator anim;
    protected SpriteRenderer sprite;
    protected Rigidbody2D rigid;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector2(enemyData.SightRange, 1.5f));
    }

    virtual protected void Awake()
    {
        hp = enemyData.Hp;
        atk = enemyData.Damage;
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        rigid = GetComponent<Rigidbody2D>();
    }

    virtual protected void Update()
    {
        detectPlayer();
        attack();
    }

    abstract protected void attack();

    protected void dead()
    {
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("Dead");
        // Destroy(gameObject);
    }

    abstract protected void detectPlayer();

    public void OnDamaged(float damage)
    {
        anim.SetTrigger("Hit");

        // 치명타 계산
        hp -= damage;

        if (hp <= 0)
            dead();
    }

    public void DoDestroy()
    {
        Destroy(gameObject);
    }
}
