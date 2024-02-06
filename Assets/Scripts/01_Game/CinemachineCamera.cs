using System;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class CinemachineCamera : Singleton<CinemachineCamera>
{
    public CinemachineVirtualCamera Camera;
    public CinemachineFramingTransposer composer;

    public Transform Follow { get { return Camera.Follow; } set { Camera.Follow = value; } }
    public Transform LookAt { get { return Camera.LookAt; } set { Camera.LookAt = value; } }
    public float OrthographicSize { get { return Camera.m_Lens.OrthographicSize; } set { Camera.m_Lens.OrthographicSize = value; } }

    [Header("기본 세팅")]
    public float DefaultOrthographicSize;
    public Vector2 DeadZone;
    public Vector2 SoftZone;

    Vector3 LastPosition;

    private void Awake()
    {
        Camera = GetComponent<CinemachineVirtualCamera>();
        composer = Camera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    public void ResetCamera()
    {
        Transform character = FindObjectOfType<Character>().transform;

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

}
