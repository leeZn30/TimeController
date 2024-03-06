using UnityEngine;

public class GreenEnemy : Enemy
{
    [SerializeField] float maxSpeed;
    bool isMovable = true;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        detectPlayer();
    }

    private void FixedUpdate()
    {
        attack();
    }

    protected override void attack()
    {
        if (isPlayerFound && isMovable)
        {
            if (!anim.GetBool("isRun"))
                anim.SetBool("isRun", true);

            float direction;
            if (Character.Instance.gameObject.transform.position.x - transform.position.x >= 0)
            {
                direction = 1;
                sprite.flipX = true;
            }
            else
            {
                direction = -1;
                sprite.flipX = false;
            }

            rigid.AddForce(direction * enemyData.MoveSpeed * Vector2.right, ForceMode2D.Impulse);
            if (Mathf.Abs(rigid.velocity.x) > maxSpeed)
            {
                rigid.velocity = new Vector2(maxSpeed * direction, rigid.velocity.y);
            }
        }

    }

    protected override void detectPlayer()
    {
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(enemyData.SightRange, 1.5f), 0, LayerMask.GetMask("Player"));
        if (player != null)
        {
            isPlayerFound = true;
        }
        else
        {
            if (anim.GetBool("isRun"))
                anim.SetBool("isRun", false);
            isPlayerFound = false;
        }
    }

    void OnMovable()
    {
        isMovable = true;
        anim.SetBool("isStun", false);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag.Equals("Player"))
        {
            isMovable = false;

            // 플레이어랑 닿았음
            anim.SetBool("isStun", true);
            int dirc = transform.position.x - Character.Instance.gameObject.transform.position.x > 0 ? 1 : -1;
            rigid.AddForce(new Vector2(dirc, 0) * 5, ForceMode2D.Impulse);

            Invoke("OnMovable", 2f);
        }
    }

}
