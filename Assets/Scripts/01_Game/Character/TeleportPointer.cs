using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TeleportPointer : MonoBehaviour
{
    [SerializeField] Tilemap ground;
    Collider2D collider;

    private void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
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

        if (ground.HasTile(cellPosition))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

}

