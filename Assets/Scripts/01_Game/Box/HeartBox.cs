using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartBox : Box
{
    protected override void interact()
    {
        base.interact();

        ShowItem();

        Character.Instance.Hp += 4;
        HeartManager.Instance.AddHeart();

        GameData.Hp = Character.Instance.Hp;
    }
}
