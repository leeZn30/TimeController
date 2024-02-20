using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : Box
{
    protected override void interact()
    {
        base.interact();

        GameData.Stage++;

        SceneManager.LoadScene(string.Format("Stage{0}", GameData.Stage));
        FindObjectOfType<Background>().resetMaterial();
    }
}
