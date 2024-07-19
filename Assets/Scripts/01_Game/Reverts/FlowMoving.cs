using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowMoving : MovingEvent
{
    [SerializeField] float waveFrequency;
    [SerializeField] float waveHeight;
    [SerializeField] Vector3 startPose;
    [SerializeField] Vector3 targetPose;
    [SerializeField] float duration;

    void Awake()
    {
        startPose = transform.position;
    }

    protected override IEnumerator Move()
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // 위치 보간
            Vector3 newPosition = Vector3.Lerp(startPose, targetPose, t);

            // 파도 효과 추가
            newPosition.y += Mathf.Sin(t * Mathf.PI * waveFrequency) * waveHeight;

            // 위치 설정
            transform.position = newPosition;

            yield return null; // 다음 프레임까지 대기
        }

        // 최종 위치를 명시적으로 설정
        transform.position = targetPose;

    }
}
