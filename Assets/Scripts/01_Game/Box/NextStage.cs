using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextStage : Box
{
    override protected void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    protected override void interact()
    {
        isInteractable = false;

        GameData.Stage++;

        SceneManager.LoadScene(string.Format("Stage{0}", GameData.Stage));
        FindObjectOfType<Background>().resetMaterial();
    }

}
