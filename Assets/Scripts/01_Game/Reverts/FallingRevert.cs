using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRevert : Revertible
{
    Animator animator;

    protected override void Awake()
    {
        animator = GetComponent<Animator>();
        base.Awake();
    }

    override protected void Update()
    {
        base.Update();
    }

    protected override void checkChangeCondition()
    {
        if (Character.Instance != null)
        {
            if (Vector3.Distance(Character.Instance.transform.position, collider.bounds.center) < 5f && !isRevertible)
            {
                Change();
            }
        }
    }


    public override void Change()
    {
        base.Change();

        isRevertible = true;
        animator.SetTrigger("Fall");
    }

    protected override void Rewind()
    {
        animator.SetTrigger("Climb");
    }
}
