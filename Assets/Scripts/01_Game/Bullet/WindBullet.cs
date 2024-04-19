using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindBullet : Bullet
{
    protected override void move()
    {
        transform.position += targetDirection * bulletData.MoveSpeed * Time.deltaTime;
    }
}
