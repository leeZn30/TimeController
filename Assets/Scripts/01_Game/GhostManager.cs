using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GhostManager : Singleton<GhostManager>
{
    public int ghostCount;
    [SerializeField] Ghost GhostPfb;

    TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void CreateGhost(int score, Vector3 createdPoint)
    {
        Ghost ghost = Instantiate(GhostPfb, createdPoint, Quaternion.identity, GameObject.Find("Canvas").transform);
        ghost.Score = score;
    }

    public void AddGhost(int ghost)
    {
        ghostCount += ghost;
        text.SetText(ghostCount.ToString());
    }

    public void UseGhost(int need)
    {
        if (ghostCount >= need)
            ghostCount -= need;
    }


}