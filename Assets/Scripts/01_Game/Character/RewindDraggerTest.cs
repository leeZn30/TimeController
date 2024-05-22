using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class RewindDraggerTest : MonoBehaviour
{
    Canvas canvas;
    RectTransform canvasRectTransform;
    bool isDragStart = false;
    RectTransform rect;
    Vector2 startPos;
    Vector2 endPos;
    Vector2 size;
    BoxCollider2D boxCollider;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasRectTransform = canvas.GetComponent<RectTransform>();
        rect = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider2D>();

        // startPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 screenPosition = (Vector2)Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, canvas.worldCamera, out startPos);
        rect.anchoredPosition = startPos;

        rect.sizeDelta = new Vector2(0, 0);
        size = rect.sizeDelta;
        boxCollider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        boxCollider.offset = size * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if ((Mathf.Abs(size.x) > 0.5f || Mathf.Abs(size.y) > 0.5f) && !isDragStart)
        {
            SoundManager.Instance.PlaySFX(AudioType.Character, "Rewind");
            isDragStart = true;
            PostPrecessingController.Instance.CallRewindEffect(0.3f);
        }

        Vector2 screenPosition = (Vector2)Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, screenPosition, canvas.worldCamera, out endPos);
        // endPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        size = (endPos - startPos) * 0.017f;
        rect.sizeDelta = size;
        boxCollider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        boxCollider.offset = size * 0.5f;

        if (Input.GetMouseButtonUp(1) || Input.GetMouseButtonDown(0))
        {
            Character.Instance.FinishRewind();
            PostPrecessingController.Instance.CallRewindEffect(0f);
            Destroy(gameObject);
        }
    }

}
