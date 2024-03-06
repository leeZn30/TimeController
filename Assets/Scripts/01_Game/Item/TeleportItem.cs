using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TeleportItem : Item
{
    override protected void Awake()
    {
        base.Awake();
        comment = "순간이동 게이지 충전";
    }

    protected override void OperateItem()
    {
        base.OperateItem();
        Character.Instance.FullChargeGauge("Teleport");
    }
}
