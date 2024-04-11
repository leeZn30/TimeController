using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCTVEnemy : Enemy
{
    [SerializeField] Bullet PbulletPfb;
    [SerializeField] Bullet SpecialPfb;
    [SerializeField] float interval = 0.05f;
    float duration = 2f;

    bool isReadyToSpecialAttack = false;
    public bool isSightVewing = true;

    LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.enabled = false;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void CallSpecailAttack()
    {
        isReadyToSpecialAttack = true;
        StartCoroutine(SpecialAttack());
    }

    IEnumerator SpecialAttack()
    {
        float settingTime = 0f;
        lineRenderer.enabled = true;
        while (settingTime < 2f)
        {
            settingTime += Time.deltaTime;

            lineRenderer.SetPosition(1, Character.Instance.transform.position);
            Vector3 targetPose = lineRenderer.GetPosition(1);
            Vector2 direction = (targetPose - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            yield return null;
        }

        lineRenderer.enabled = false;
        BulletManager.TakeOutBullet(SpecialPfb.BulletName, transform.position);
        isReadyToSpecialAttack = false;
    }

    protected override void attack()
    {
        if (!isReadyToSpecialAttack)
        {
            if (isPlayerFound)
            {
                // head 돌리기
                Vector2 direction = (Character.Instance.transform.position - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 180f;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                duration += Time.deltaTime;

                if (duration > interval)
                {
                    BulletManager.TakeOutBullet(PbulletPfb.BulletName, transform.position);
                    duration = 0f;
                }
            }
            else
            {
                float targetRotation = 0f;
                float currentRotation = Mathf.MoveTowardsAngle(transform.rotation.eulerAngles.z, targetRotation, 100f * Time.deltaTime);
                transform.rotation = Quaternion.Euler(0f, 0f, currentRotation);
            }
        }
    }

    protected override void detectPlayer()
    {
        if (isSightVewing)
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
}
