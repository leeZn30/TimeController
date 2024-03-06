using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeartItem : Item
{
    override protected void Awake()
    {
        base.Awake();
        comment = "하트 회복";
    }

    protected override void OperateItem()
    {
        base.OperateItem();
        HeartManager.Instance.RecoverHeart();
    }
}
