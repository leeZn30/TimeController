using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    Transform leftArm;
    Transform rightArm;
    HingeJoint2D barJoint;
    DistanceJoint2D leftJoint;
    DistanceJoint2D rightJoint;

    bool isCleared = false;

    bool isBalanced = false;
    float balanceTime = 0;
    [SerializeField] float forceLimit = 1f;
    [SerializeField] float distanceLimit = 1f;
    [SerializeField] float checkDuration = 3f;

    private void Awake()
    {
        leftArm = transform.Find("LeftArm");
        rightArm = transform.Find("RightArm");

        barJoint = transform.Find("Bar").GetComponent<HingeJoint2D>();
        leftJoint = leftArm.GetComponent<DistanceJoint2D>();
        rightJoint = rightArm.GetComponent<DistanceJoint2D>();
    }

    private void Update()
    {
        if (!isCleared)
        {
            CheckClear();
        }
    }

    // 클리어 조건 다 다름
    // defualt: 단순 확인
    protected virtual void CheckClear()
    {
        float yDifference = Mathf.Abs(leftArm.position.y - rightArm.position.y);
        float yForceDiffernce = Mathf.Abs(leftJoint.reactionForce.y - rightJoint.reactionForce.y);

        if (yDifference <= distanceLimit)
        {
            if (!isBalanced)
            {
                balanceTime = 0.0f;
                isBalanced = true;
            }

            balanceTime += Time.deltaTime;

            if (balanceTime >= checkDuration)
            {
                isCleared = true;
                Clear();
            }
        }
        else
        {
            balanceTime = 0.0f;
            isBalanced = false;
        }
    }

    // 클리어 때는 다 다름
    public virtual void Clear()
    {
        RuleManager.Instance.Clear();
    }
}
