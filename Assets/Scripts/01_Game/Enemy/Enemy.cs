using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyData enemyData;
    public float hp;
    public float atk;

    Animator anim;

    protected void Awake()
    {
        hp = enemyData.Hp;
        atk = enemyData.Damage;
        anim = GetComponent<Animator>();
    }

    abstract protected void attack();

    protected void dead()
    {
        // anim.SetTrigger("Dead");
        Destroy(gameObject);
    }

    abstract protected void detectPlayer();

    public void OnDamaged(float damage)
    {
        anim.SetTrigger("Hit");

        // 치명타 계산
        hp -= damage;

        if (hp <= 0)
            dead();
    }

    public void DoDestroy()
    {
        Destroy(gameObject);
    }
}
