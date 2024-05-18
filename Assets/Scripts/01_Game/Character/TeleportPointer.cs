using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TeleportPointer : MonoBehaviour
{
    Tilemap ground;
    [SerializeField] List<Tilemap> grounds = new List<Tilemap>();

    private void Awake()
    {
        ground = GameObject.Find("Ground").GetComponent<Tilemap>();

        Time.timeScale = 0.05f;
        SoundManager.Instance.AdjucstBGMPitch(0.6f, 0.5f);
        PostPrecessingController.Instance.CallTeleportStartEffect();
    }

    private void Update()
    {
        // 텔레포트 중단
        if (Input.GetMouseButtonDown(1))
        {
            Character.Instance.StopTeleport();
            Time.timeScale = 1f;
            SoundManager.Instance.AdjucstBGMPitch();
            PostPrecessingController.Instance.CallTeleportFinishEffect();
            Destroy(gameObject);
        }
        // 텔레포트 완료
        else if (Input.GetMouseButtonDown(0))
        {
            if (isTeleportable())
            {
                SoundManager.Instance.PlaySFX(AudioType.Character, "Teleport");
                Character.Instance.FinishTeleport();
                Time.timeScale = 1f;
                SoundManager.Instance.AdjucstBGMPitch();
                PostPrecessingController.Instance.CallTeleportFinishEffect();
                Destroy(gameObject);
            }
        }

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    bool isTeleportable()
    {
        Vector3Int cellPosition = ground.WorldToCell(transform.position);
        int layerMask = (1 << LayerMask.NameToLayer("Map")) | (1 << LayerMask.NameToLayer("UnTeleportableObject"));
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.back, 1f, layerMask);

        if (ground.HasTile(cellPosition) || hit.collider != null)
        {
            return false;
        }
        return true;
    }

}

