using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingRevert : Revertible
{
    [SerializeField] float movingSpeed;
    [SerializeField] float movingTime;

    bool isMoving = false;

    LineRenderer lineRenderer;

    protected override void Awake()
    {
        base.Awake();

        lineRenderer = GetComponent<LineRenderer>();
    }

    protected override void checkChangeCondition()
    {

    }

    protected override void Update()
    {
        base.Update();
    }

    public override void Change()
    {
        base.Change();
    }

    // IEnumerator Move()
    // {
    //     float duration = 0f;

    //     while(duration < movingTime)
    //     {
    //         Vector3.MoveTowards(transform.position, )

    //         duration += Time.deltaTime;
    //         yield return null;
    //     }

}

protected override void Rewind()
{
}

}
