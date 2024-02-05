using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    [SerializeField] float dissapearTime;
    [SerializeField] float minScale; // 최소 스케일

    private Vector3 initialScale;

    public SpriteRenderer sprite;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        initialScale = transform.localScale;
    }

    private void Start()
    {
        StartCoroutine(Dissapear());
    }

    IEnumerator Dissapear()
    {
        float duration = 0f;
        while (duration < dissapearTime)
        {
            duration += Time.unscaledDeltaTime;

            sprite.color = Color.Lerp(sprite.color, new Color(sprite.color.r, sprite.color.g, sprite.color.b, 0), Time.unscaledDeltaTime);

            // 스케일을 변경하여 오브젝트를 축소
            // 축소 비율을 계산 (0에서 1로)
            float shrinkRatio = Mathf.Clamp01(duration / (dissapearTime * 1.5f));

            // 스케일을 변경하여 오브젝트를 축소
            transform.localScale = initialScale * Mathf.Max(1.0f - shrinkRatio, minScale);

            yield return null;
        }

        Destroy(gameObject);
    }
}
