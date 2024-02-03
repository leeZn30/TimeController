using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : Enemy
{
    bool isPlayerFound;
    // [SerializeField] Bullet bulletPfb;
    [SerializeField] ParringBullet PbulletPfb;
    float interval = 2f;
    float duration = 0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyData.SightRange);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        detectPlayer();
        attack();
    }

    protected override void detectPlayer()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, enemyData.SightRange, LayerMask.GetMask("Player"));
        if (player != null)
        {
            isPlayerFound = true;
        }
        else
        {
            isPlayerFound = false;
        }

    }

    protected override void attack()
    {
        if (isPlayerFound)
        {
            duration += Time.deltaTime;

            if (duration > interval)
            {
                Instantiate(PbulletPfb, transform.position, Quaternion.identity);
                duration = 0f;
            }
        }
    }
}
