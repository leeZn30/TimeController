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

    protected bool isPlayerFound = false;
    public float hp;
    public float atk;
    [SerializeField] SigthType sigthType;


    protected Animator anim;
    protected SpriteRenderer sprite;
    protected Rigidbody2D rigid;

    virtual protected void OnDrawGizmos()
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

    protected void Dead()
    {
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("Dead");
        gameObject.layer = 11;

        if (GhostManager.Instance != null)
            GhostManager.Instance.CreateGhost(enemyData.Ghost, transform.localPosition);
    }

    abstract protected void detectPlayer();

    virtual public void OnDamaged(float damage, DamageType damageType)
    {
        rigid.velocity = Vector2.zero;

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

        FixedUIManager.Instance.ShowDamage((int)damage, new Vector2(transform.localPosition.x, transform.localPosition.y + transform.localScale.y * 0.8f), isCritical);

        hp -= damage;

        if (hp <= 0)
            Dead();
    }

    public void DoDestroy()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Trap" && gameObject.layer != 11)
        {
            Dead();
        }
    }

}
