using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoEnemy : FrogEnemy
{
    public override void OnDamaged(float damage, DamageType damageType)
    {
        if (damageType == DamageType.ParriedBullet)
            base.OnDamaged(damage, damageType);
        else
            base.OnDamaged(0, damageType);

    }
}
