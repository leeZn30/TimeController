using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] protected BulletData bulletData;
    SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();

        if (Random.value <= bulletData.ParryPercent)
        {
            sprite.material = bulletData.Material;
            gameObject.AddComponent<Parriable>();
        }
    }

    // 일직선탄
    virtual protected void move(Vector2 targetPose)
    {
        transform.Translate(Vector2.left * bulletData.MoveSpeed * Time.deltaTime);
    }
}
