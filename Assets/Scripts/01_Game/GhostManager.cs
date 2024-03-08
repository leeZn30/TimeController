using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GhostManager : Singleton<GhostManager>
{
    public int ghostCount;
    [SerializeField] GameObject GhostPfb;

    Image ghostImg;
    TextMeshProUGUI text;

    private void Awake()
    {
        ghostImg = GetComponentInChildren<Image>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    IEnumerator MoveGhost(Vector3 Point)
    {
        GameObject ghost = Instantiate(GhostPfb, Point, Quaternion.identity, GameObject.Find("Canvas").transform);

        // Vector3 targetPose = Camera.main.ScreenToWorldPoint(ghostImg.transform.localPosition);
        Vector3 targetPose = ghostImg.transform.position;

        while (ghost.transform.position != targetPose)
        {
            ghost.transform.position = Vector3.MoveTowards(ghost.transform.position, targetPose, Time.deltaTime * 20f);
            yield return null;
        }

        Destroy(ghost);
    }

    public void AddGhost(int ghost, Vector3 createdPoint)
    {
        // StartCoroutine(MoveGhost(createdPoint));
        ghostCount += ghost;
        text.SetText(ghostCount.ToString());
    }

    public void UseGhost(int need)
    {
        if (ghostCount >= need)
            ghostCount -= need;
    }


}
