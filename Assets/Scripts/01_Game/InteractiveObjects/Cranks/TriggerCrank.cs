using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCrank : Crank
{
    [SerializeField] Trigger TriggerObj;

    protected override void interact()
    {
        base.interact();

        TriggerObj.TriggerEvent();
    }
}
