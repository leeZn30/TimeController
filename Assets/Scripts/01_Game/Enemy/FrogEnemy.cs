using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public class FrogEnemy : Enemy
{
    [SerializeField] Bullet PbulletPfb;
    [SerializeField] float interval = 3f;
    float duration = 0f;
    bool isPlayerFound;

    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawWireCube(transform.position, new Vector2(enemyData.SightRange, 1.5f));
    // }

    protected override void Update()
    {
        base.Update();

        if (isPlayerFound)
        {
            if (Character.Instance.transform.position.x <= transform.position.x)
            {
                sprite.flipX = false;
            }
            else
            {
                sprite.flipX = true;
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
                anim.SetTrigger("Attack");
                Instantiate(PbulletPfb, transform.position, transform.rotation);
                duration = 0f;
            }
        }
    }
}
