using UnityEngine.Rendering;
using UnityEngine;
using VolFx;

public class RewindDragger : MonoBehaviour
{
    bool isDragStart = false;

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
        transform.localPosition = startPos;

        spriteRenderer.size = Vector2.zero;
        size = spriteRenderer.size;
        boxCollider.size = new Vector2(Mathf.Abs(size.x), Mathf.Abs(size.y));
        boxCollider.offset = size * 0.5f;
    }

    private void Update()
    {
        if ((Mathf.Abs(size.x) > 0.5f || Mathf.Abs(size.y) > 0.5f) && !isDragStart)
        {
            SoundManager.Instance.PlaySFX(AudioType.Character, "Rewind");
            isDragStart = true;
            PostPrecessingController.Instance.CallRewindEffect(0.3f);
        }

        endPos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        size = endPos - startPos;
        spriteRenderer.size = size;
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
