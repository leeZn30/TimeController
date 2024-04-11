using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CCTVBullet : Bullet
{
    float currentTime = 0f;
    [SerializeField] float duration;

    protected override void move()
    {
        // 유도탄
        targetDirection = (Character.Instance.transform.position - transform.position).normalized;
        transform.position += targetDirection * bulletData.MoveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 180f, Vector3.forward);
    }

    protected override void EraseBullet()
    {
        if (currentTime >= duration || (Character.Instance.gameObject.layer == 9 && Vector2.Distance(transform.position, Character.Instance.transform.position) < 0.5f))
        {
            BulletManager.InsertBullet(this);
        }
        else
        {
            currentTime += Time.deltaTime;
        }
    }
}
