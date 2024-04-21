using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using CC = CinemachineCamera;

public class Rule1Manager : RuleManager
{
    [SerializeField] float ZoomInSize;

    IEnumerator CameraZoom()
    {
        CC.Instance.ChangeSoftZone(new Vector2(0, 0));
        while (CC.Instance.OrthographicSize > ZoomInSize)
        {
            CC.Instance.OrthographicSize -= Time.unscaledDeltaTime * 20f;

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            if (CC.Instance.OrthographicSize != ZoomInSize)
            {
                base.Clear();

                StartCoroutine(CameraZoom());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            if (CC.Instance.OrthographicSize != ZoomInSize)
            {
                CC.Instance.ResetCamera();
            }
        }
    }
}
