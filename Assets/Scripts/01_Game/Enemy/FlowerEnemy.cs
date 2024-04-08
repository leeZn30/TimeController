using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerEnemy : Enemy
{
    [SerializeField] Bullet BulletPfb;
    [SerializeField] float interval = 1f;
    [SerializeField] FlowerWall PairWall;
    float duration = 1f;

    protected override void attack()
    {
        if (isPlayerFound)
        {
            duration += Time.deltaTime;

            if (duration > interval)
            {
                BulletManager.TakeOutBullet(BulletPfb.BulletName, transform.position);
                duration = 0f;
            }
        }
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

    public override void OnDamaged(float damage, DamageType damageType)
    {
        if (damageType == DamageType.ParriedBullet)
            base.OnDamaged(damage, damageType);
    }

    private void OnDestroy()
    {
        if (PairWall != null)
            PairWall.OpenWall();
    }

}
