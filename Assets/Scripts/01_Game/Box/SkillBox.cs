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
        ShowItem();
    }
}
