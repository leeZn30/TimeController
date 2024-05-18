using System;
using System.Collections;
using Cinemachine;
using UnityEngine;

public class CinemachineCamera : Singleton<CinemachineCamera>
{
    public CinemachineVirtualCamera Camera;
    public CinemachineFramingTransposer composer;

    public Transform Follow { get { return Camera.m_Follow; } set { Camera.m_Follow = value; } }
    public Transform LookAt { get { return Camera.m_LookAt; } set { Camera.m_LookAt = value; } }
    public float OrthographicSize { get { return Camera.m_Lens.OrthographicSize; } set { Camera.m_Lens.OrthographicSize = value; } }

    [Header("기본 세팅")]
    public float DefaultOrthographicSize;
    public Vector2 DeadZone;
    public Vector2 SoftZone;

    private void Awake()
    {
        Camera = GetComponent<CinemachineVirtualCamera>();
        composer = Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void ResetCamera()
    {
        Transform character = Character.Instance.transform;

        Camera.m_Lens.OrthographicSize = DefaultOrthographicSize;
        composer.m_DeadZoneWidth = DeadZone.x;
        composer.m_DeadZoneHeight = DeadZone.y;
        composer.m_SoftZoneHeight = SoftZone.x;
        composer.m_SoftZoneHeight = SoftZone.y;
        Camera.Follow = character;
        Camera.LookAt = character;
    }

    public void ChangeDeadZone(Vector2 newZone)
    {
        composer.m_DeadZoneWidth = newZone.x;
        composer.m_DeadZoneHeight = newZone.y;
    }

    public void ChangeSoftZone(Vector2 newZone)
    {
        composer.m_SoftZoneWidth = newZone.x;
        composer.m_SoftZoneHeight = newZone.y;
    }

    public void Zoom(float targetFloat, float duration)
    {
        StartCoroutine(Zooming(targetFloat, duration));
    }

    IEnumerator Zooming(float targetSize, float duration)
    {
        ChangeSoftZone(new Vector2(0, 0));

        float currentTime = 0f;

        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            OrthographicSize = Mathf.Lerp(OrthographicSize, targetSize, currentTime / duration);

            yield return null;
        }

    }

}
