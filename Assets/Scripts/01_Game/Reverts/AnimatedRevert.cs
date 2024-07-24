using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedRevert : Revertible
{
    Animator anim;

    protected override void Awake()
    {
        anim = GetComponent<Animator>();
        base.Awake();
    }

    override protected void Update()
    {
        base.Update();
    }

    public override void Change()
    {
        base.Change();

        anim.SetTrigger("Change");
    }


    protected override void Rewind()
    {
        anim.SetTrigger("Revert");
        StartCoroutine(returnAfterRewind());
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
}
