using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenEnemy : Enemy
{
    bool isPlayerFound;

    protected override void Update()
    {
        base.Update();
    }

    protected override void attack()
    {
        if (isPlayerFound)
        {
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
}
