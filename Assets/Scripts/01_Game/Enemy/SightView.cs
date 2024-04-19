using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SightView : MonoBehaviour
{
    CCTVEnemy cctv;

    Color blinkColor;
    [SerializeField] float blinkDuration = 1.0f;
    [SerializeField] float blinkedTime = 3f;
    [SerializeField] float coloredTime = 3f;

    new private SpriteRenderer renderer;
    private Color initialColor;
    private bool isBlinking = false;

    void Start()
    {
        cctv = GetComponentInParent<CCTVEnemy>();

        renderer = GetComponent<SpriteRenderer>();

        initialColor = renderer.color;
        blinkColor = new Color(renderer.color.r, renderer.color.b, renderer.color.g, 0);

        // 깜빡이기 시작
        StartBlinking();
    }

    // 깜빡이는 효과 시작
    void StartBlinking()
    {
        if (!isBlinking)
        {
            StartCoroutine(BlinkCoroutine());
            isBlinking = true;
        }
    }

    // 깜빡이는 효과 코루틴
    IEnumerator BlinkCoroutine()
    {
        while (true)
        {
            // 점차적으로 색을 변경하여 깜빡이는 효과 구현
            float elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                renderer.color = Color.Lerp(initialColor, blinkColor, elapsedTime / blinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            cctv.isSightVewing = false;
            yield return new WaitForSeconds(blinkedTime);

            cctv.isSightVewing = true;
            // 색을 되돌리는 부분 추가
            elapsedTime = 0f;
            while (elapsedTime < blinkDuration)
            {
                renderer.color = Color.Lerp(blinkColor, initialColor, elapsedTime / blinkDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(coloredTime);
        }
    }
}
