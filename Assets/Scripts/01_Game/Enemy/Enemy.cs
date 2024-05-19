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

    protected bool isDead = false;
    protected bool isPlayerFound = false;
    public float hp;
    public float atk;
    [SerializeField] SigthType sigthType;


    protected Animator anim;
    protected SpriteRenderer sprite;
    protected Rigidbody2D rigid;
    protected Collider2D collider;

    virtual protected void OnDrawGizmos()
    {
        collider = GetComponent<Collider2D>();
        Gizmos.color = Color.magenta;

        switch (sigthType)
        {
            case SigthType.Rectangle:
                Gizmos.DrawWireCube(collider.bounds.center, new Vector2(enemyData.SightRange, collider.bounds.size.y));
                break;

            case SigthType.Circle:
                Gizmos.DrawWireSphere(collider.bounds.center, enemyData.SightRange);
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
        collider = GetComponent<Collider2D>();
    }

    virtual protected void Update()
    {
        detectPlayer();
        attack();
    }

    abstract protected void attack();

    protected virtual void Dead()
    {
        rigid.velocity = Vector2.zero;
        anim.SetTrigger("Dead");
        gameObject.layer = LayerMask.NameToLayer("DeadEnemy");
        isDead = true;

        if (GhostManager.Instance != null && enemyData.Ghost > 0)
            GhostManager.Instance.CreateGhost(enemyData.Ghost, transform.localPosition);
    }

    abstract protected void detectPlayer();

    virtual public void OnDamaged(float damage, DamageType damageType)
    {
        if (!isDead)
        {
            rigid.velocity = Vector2.zero;
            bool isCritical = false;

            if (damageType == DamageType.Player)
            {
                if (Random.value >= 0.95f)
                {
                    isCritical = true;
                    damage *= Random.Range(1.1f, 2.0f);
                }
            }
            else
            {
                isCritical = true;
                damage *= Random.Range(2.0f, 3.0f);
            }
            damage = Mathf.Round(damage);

            if (damage > 0)
                anim.SetTrigger("Hit");
            FixedUIManager.Instance.ShowDamage((int)damage, collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2), isCritical);

            hp -= damage;

            if (hp <= 0)
                Dead();
        }
    }

    public void DoDestroy()
    {
        Destroy(gameObject);
    }

    virtual protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Trap" && !isDead)
        {
            Dead();
        }
    }

}
