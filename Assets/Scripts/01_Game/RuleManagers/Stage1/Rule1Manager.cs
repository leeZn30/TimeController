using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using CC = CinemachineCamera;

public class Rule1Manager : RuleManager
{
    [SerializeField] float ZoomInSize;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            if (CC.Instance.OrthographicSize != ZoomInSize)
            {
                base.Clear();

                CC.Instance.Zoom(3f, 1f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            if (CC.Instance != null && CC.Instance.OrthographicSize != ZoomInSize)
            {
                CC.Instance.ResetCamera();
            }
        }
    }
}
