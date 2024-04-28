using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBox : Box
{
    [Header("SKill")]
    [SerializeField] string Skill;

    protected override void interact()
    {
        base.interact();

        Character.Instance.activeSkill(Skill);
        switch (Skill)
        {
            case "Teleport":
                GameData.TeleportActive = true;
                break;
            case "Rewind":
                GameData.RewindActive = true;
                break;
            case "Slow":
                GameData.SlowActive = true;
                break;
        }
        ShowItem();
    }
}
