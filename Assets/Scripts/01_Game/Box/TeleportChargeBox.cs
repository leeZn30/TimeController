using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportChargeBox : Box
{
    protected override void interact()
    {
        base.interact();

        ShowItem();

        GameData.TeleportChargeSpeed += 20f;
    }
}
