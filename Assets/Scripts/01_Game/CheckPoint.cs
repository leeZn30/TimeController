using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    public int CheckIdx;
    [SerializeField] int needGhost;
    [SerializeField] GameObject needGhostUIPfb;

    GameObject needGhostUI;
    Canvas canvas;
    Animator anim;

    private void Awake()
    {
        canvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        anim = GetComponent<Animator>();

        if (GameData.CheckPoint >= CheckIdx)
            anim.SetBool("Flutter", true);
        else
        {
            needGhostUI = Instantiate(needGhostUIPfb, transform.position + Vector3.up * 1.5f, Quaternion.identity, canvas.transform);
            needGhostUI.GetComponentInChildren<TextMeshProUGUI>().SetText(needGhost.ToString());
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player") && !anim.GetBool("isChecked") && GhostManager.Instance.ghostCount >= needGhost)
        {
            GhostManager.Instance.UseGhost(needGhost);
            anim.SetBool("isChecked", true);

            StageManager.SaveStage(CheckIdx: CheckIdx);

            Destroy(needGhostUI);
        }
    }


}
