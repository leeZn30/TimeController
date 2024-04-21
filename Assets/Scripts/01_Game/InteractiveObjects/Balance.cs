using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balance : MonoBehaviour
{
    [SerializeField] float LeftMass => ComputeArmMass(leftBar);
    [SerializeField] float RightMass => ComputeArmMass(rightBar);

    GameObject LeftArm;
    GameObject RightArm;
    [SerializeField] GameObject leftBar;
    [SerializeField] GameObject rightBar;

    [SerializeField] float Heavy;
    [SerializeField] float Middle;
    [SerializeField] float Light;
    [SerializeField] float MoveSpeed;

    bool isClear = false;

    private void Awake()
    {
        LeftArm = transform.GetChild(0).gameObject;
        RightArm = transform.GetChild(1).gameObject;
    }

    private void Update()
    {
        CheckClear();

        if (LeftMass < RightMass)
        {
            if (LeftArm.transform.position.y != Light)
            {
                LeftArm.transform.position = Vector3.MoveTowards(LeftArm.transform.position, new Vector2(LeftArm.transform.position.x, Light), MoveSpeed * Time.deltaTime);
            }

            if (RightArm.transform.position.y != Heavy)
            {
                RightArm.transform.position = Vector3.MoveTowards(RightArm.transform.position, new Vector2(RightArm.transform.position.x, Heavy), MoveSpeed * Time.deltaTime);
            }
        }
        else if (LeftMass > RightMass)
        {
            if (LeftArm.transform.position.y != Heavy)
            {
                LeftArm.transform.position = Vector3.MoveTowards(LeftArm.transform.position, new Vector2(LeftArm.transform.position.x, Heavy), MoveSpeed * Time.deltaTime);
            }

            if (RightArm.transform.position.y != Light)
            {
                RightArm.transform.position = Vector3.MoveTowards(RightArm.transform.position, new Vector2(RightArm.transform.position.x, Light), MoveSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (LeftArm.transform.position.y != Middle)
            {
                LeftArm.transform.position = Vector3.MoveTowards(LeftArm.transform.position, new Vector2(LeftArm.transform.position.x, Middle), MoveSpeed * Time.deltaTime);
            }

            if (RightArm.transform.position.y != Middle)
            {
                RightArm.transform.position = Vector3.MoveTowards(RightArm.transform.position, new Vector2(RightArm.transform.position.x, Middle), MoveSpeed * Time.deltaTime);
            }
        }
    }

    float ComputeArmMass(GameObject arm)
    {
        float sum = 0;

        for (int i = 0; i < arm.transform.childCount; i++)
        {
            Rigidbody2D rigid = arm.transform.GetChild(i).GetComponent<Rigidbody2D>();
            if (rigid != null)
            {
                sum += rigid.mass;
            }
        }

        Debug.DrawRay(arm.transform.position, Vector2.right, Color.red);
        RaycastHit2D[] hits = Physics2D.RaycastAll(arm.transform.position, Vector2.right, 2f);
        for (int i = 0; i < hits.Length; i++)
        {
            Rigidbody2D rigid = hits[i].transform.GetComponent<Rigidbody2D>();
            if (rigid != null)
            {
                sum += rigid.mass;
            }
        }
        return sum;
    }

    // Balance마다 다름
    virtual protected void CheckClear()
    {
        if (!isClear)
        {
            if (LeftArm.transform.position.y == RightArm.transform.position.y)
            {
                RaycastHit2D leftHit = Physics2D.Raycast(leftBar.transform.position, Vector2.right, 2f, LayerMask.GetMask("Player"));
                if (leftHit.collider != null && leftHit.collider.tag.Equals("Player"))
                {
                    isClear = true;
                    RuleManager.Instance.Clear();
                }
            }

        }
    }
}
