using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

enum SigthType
{
    Rectangle, Circle
}

public enum DamageType
{
    Player, ParriedBullet
}

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyData enemyData;

    public float hp;
    public float atk;
    [SerializeField] SigthType sigthType;


    protected Animator anim;
    protected SpriteRenderer sprite;
    protected Rigidbody2D rigid;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;

        switch (sigthType)
        {
            case SigthType.Rectangle:
                Gizmos.DrawWireCube(transform.position, new Vector2(enemyData.SightRange, 1.5f));
                break;

            case SigthType.Circle:
                Gizmos.DrawWireSphere(transform.position, enemyData.SightRange);
                break;

            default:
                break;
        }
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
    }

    abstract protected void detectPlayer();

    virtual public void OnDamaged(float damage, DamageType damageType)
    {
        anim.SetTrigger("Hit");
        damage *= Random.Range(0.9f, 1.1f);
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

        DamageUI.Instance.ShowDamage((int)damage, transform.position, isCritical);

        hp -= damage;

        if (hp <= 0)
            dead();
    }


    public void DoDestroy()
    {
        Destroy(gameObject);
    }
}
