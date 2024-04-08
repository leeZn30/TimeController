using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected BulletData bulletData;
    public string BulletName => bulletData.Name;

    protected Vector3 createdPose;
    protected Vector3 targetPose;
    Vector3 targetDirection;

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

        createdPose = transform.position;
        targetPose = Character.Instance.transform.position;

        targetDirection = (targetPose - transform.position).normalized;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg + 180f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    virtual protected void Update()
    {
        move();
        EraseBullet();
    }

    // 일직선탄
    virtual protected void move()
    {
        transform.position += targetDirection * bulletData.MoveSpeed * Time.deltaTime;
        if (bulletData.RotateSpeed > 0)
            transform.Rotate(0, 0, 360 * bulletData.RotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            Character.Instance.OnDamaged(transform.position, bulletData.Damage);
            BulletManager.InsertBullet(this);
        }
    }

    protected virtual void EraseBullet()
    {
        /*
            기본: 처음 총알 위치에서 특정 거리 이상 떨어졌으며, 화면에서 보이지 않으면
            유도탄: 일정 시간 후
        */
        Vector3 nowPose = transform.position;
        Vector3 screenPose = Camera.main.WorldToScreenPoint(nowPose);

        if (Vector3.Distance(transform.position, createdPose) > 20f && isOutOfScreen(screenPose))
        {
            BulletManager.InsertBullet(this);
        }
    }

    bool isOutOfScreen(Vector3 position)
    {
        return position.x > Screen.width || position.x < 0 || position.y > Screen.height || position.y < 0;
    }
}
