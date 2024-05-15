using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.ShaderGraph;
using UnityEngine;

public class RewindDragger : MonoBehaviour
{
    Vector3 startPos;
    Vector3 endPos;
    Vector2 size;

    SpriteRenderer spriteRenderer;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        startPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = startPos;

        spriteRenderer.size = Vector2.zero;
        size = spriteRenderer.size;
        boxCollider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        boxCollider.offset = size * 0.5f;
    }

    private void Update()
    {
        endPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        size = endPos - startPos;
        spriteRenderer.size = size;
        boxCollider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        boxCollider.offset = size * 0.5f;
    }

    private void LateUpdate()
    {

    }



}
