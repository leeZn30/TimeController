using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogBullet : Bullet
{
    protected override void move()
    {
        base.move();

        if (transform.position == targetPose)
            Destroy(gameObject);
    }
}
