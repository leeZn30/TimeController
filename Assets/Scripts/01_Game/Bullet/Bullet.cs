using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected BulletData bulletData;
    protected Vector3 targetPose;

    SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        // 패링 불렛으로의 전환
        if (Random.value <= bulletData.ParryPercent)
        {
            sprite.material = bulletData.Material;
            gameObject.AddComponent<Parriable>();
        }

        targetPose = Character.Instance.transform.position;

        Vector3 targetDirection = (targetPose - transform.position).normalized;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void Update()
    {
        move();
    }

    // 일직선탄
    virtual protected void move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPose, bulletData.MoveSpeed * Time.deltaTime);
        if (bulletData.RotateSpeed > 0)
            transform.Rotate(0, 0, 360 * bulletData.RotateSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            Character.Instance.OnDamaged(transform.position, bulletData.Damage);
            Destroy(gameObject);
        }
    }
}
