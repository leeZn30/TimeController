using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogEnemy : Enemy
{
    bool isPlayerFound;
    // [SerializeField] Bullet bulletPfb;
    [SerializeField] Bullet PbulletPfb;
    float interval = 2f;
    float duration = 0f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position, new Vector2(enemyData.SightRange, 2f));
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
        Collider2D player = Physics2D.OverlapBox(transform.position, new Vector2(enemyData.SightRange, 1f), LayerMask.GetMask("Player"));
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
