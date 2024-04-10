using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using CC = CinemachineCamera;

public class Rule1Manager : Singleton<Rule1Manager>
{
    [SerializeField] Tilemap ThroughGround;
    [SerializeField] float ZoomInSize;

    Collider2D collider;

    private void Update()
    {
        // Vector3Int cellPosition = ThroughGround.WorldToCell(Character.Instance.transform.position);
        // // ThourghGround에 있으면 Camera 줌인
        // if (ThroughGround.HasTile(cellPosition))
        // {
        //     if (CC.Instance.OrthographicSize != ZoomInSize)
        //     {
        //         CC.Instance.OrthographicSize = ZoomInSize;
        //     }
        // }
        // else // 카메라 줌아웃
        // {
        //     CC.Instance.ResetCamera();
        // }
    }

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
}
