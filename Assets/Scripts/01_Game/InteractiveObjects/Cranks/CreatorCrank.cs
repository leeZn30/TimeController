using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class CreatorCrank : Crank
{
    [SerializeField] GameObject CreatedObj;
    [SerializeField] Vector2 CreatedPositon;

    protected override void interact()
    {
        base.interact();

        Instantiate(CreatedObj, CreatedPositon, quaternion.identity);
    }
}
