using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using CC = CinemachineCamera;

public class Rule1Manager : Singleton<Rule1Manager>
{
    [SerializeField] Tilemap ThroughGround;
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
            // 또로롱~ 울리게 하기
            if (CC.Instance.OrthographicSize != ZoomInSize)
            {
                StartCoroutine(CameraZoom());
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            // 또로롱~ 울리게 하기
            if (CC.Instance.OrthographicSize != ZoomInSize)
            {
                CC.Instance.ResetCamera();
            }
        }
    }
}
