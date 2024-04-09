using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportPointer : MonoBehaviour
{
    [SerializeField] Tilemap ground;

    private void Awake()
    {
        ground = GameObject.Find("Ground").GetComponent<Tilemap>();
    }

    private void Update()
    {
        // 텔레포트 중단
        if (Input.GetMouseButtonDown(1) && Character.Instance.isTeleport)
        {
            Character.Instance.isTeleport = false;
            Time.timeScale = 1f;
            Destroy(gameObject);
        }
        // 텔레포트 완료
        else if (Input.GetMouseButtonDown(0) && Character.Instance.isTeleport)
        {
            if (isTeleportable())
            {
                Character.Instance.DoTeleport();

                Character.Instance.isTeleport = false;
                Time.timeScale = 1f;
                Destroy(gameObject);
            }
        }

        transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    bool isTeleportable()
    {
        Vector3Int cellPosition = ground.WorldToCell(transform.position);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.back, 1f, LayerMask.GetMask("Map"));

        if (ground.HasTile(cellPosition) || hit.collider != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}

