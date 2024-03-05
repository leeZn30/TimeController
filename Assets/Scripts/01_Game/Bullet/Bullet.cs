using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected BulletData bulletData;
    public string BulletName => bulletData.Name;

    protected Vector3 targetPose;

    SpriteRenderer sprite;
    Material DefaultMaterial;

    virtual protected void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        DefaultMaterial = sprite.material;
    }

    public virtual void Init()
    {
        // 패링 불렛으로의 전환
        if (Random.value <= bulletData.ParryPercent)
        {
            sprite.material = bulletData.Material;

            Parriable p = gameObject.GetComponent<Parriable>();
            p.enabled = true;
            p.Init();
        }
        else
        {
            sprite.material = DefaultMaterial;
        }

        targetPose = Character.Instance.transform.position;

        Vector3 targetDirection = (targetPose - transform.position).normalized;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    virtual protected void Update()
    {
        move();
        CheckDistance();
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
            BulletManager.InsertBullet(BulletName, this);
        }
    }

    protected void CheckDistance()
    {
        if (Vector3.Distance(transform.position, Character.Instance.transform.position) > bulletData.MaxDistance)
            BulletManager.InsertBullet(BulletName, this);
    }
}
