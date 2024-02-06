using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Parriable : MonoBehaviour
{
    [SerializeField] Material material;

    public bool isParried = false;
    LineRenderer lineRenderer;
    float interval = 0.1f; // 3초
    float nextTime = 0f;
    [SerializeField] List<Vector3> poses = new List<Vector3>();


    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        poses.Add(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextTime && !isParried)
        {
            nextTime = Time.time + interval;

            if (poses.Count != 0 && poses.Last() != transform.position)
                poses.Add(transform.position);
        }
    }

    public void parried()
    {
        isParried = true;
        poses.Add(transform.position);
        lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.ToArray());

        CinemachineCamera.Instance.LookAt = transform;
        CinemachineCamera.Instance.Follow = transform;
        StartCoroutine(rotateBullet());
    }

    IEnumerator rotateBullet()
    {
        // 현재 회전 상태
        Quaternion currentRotation = transform.rotation;
        // 목표 회전 각도 (현재 각도에서 180도 추가)
        Vector3 targetRotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + 180f);
        float rotationSpeed = 150f;
        while (transform.rotation != Quaternion.Euler(targetRotation))
        {
            // 부드럽게 회전시키기
            float step = rotationSpeed * Time.unscaledDeltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(targetRotation), step);

            yield return null;
        }

        StartCoroutine(followLine());
    }

    IEnumerator followLine()
    {
        for (int i = lineRenderer.positionCount - 1; i >= 0; i--)
        {
            while (transform.position != lineRenderer.GetPosition(i))
            {
                transform.position = Vector3.MoveTowards(transform.position, lineRenderer.GetPosition(i), 15f * Time.unscaledDeltaTime);

                yield return null;
            }
            yield return null;
        }

        CinemachineCamera.Instance.ResetCamera();
        Vignette vignette;
        FindObjectOfType<Volume>().profile.TryGet(out vignette);
        vignette.intensity.value = 0f;
        Time.timeScale = 1;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isParried && other.tag.Equals("Enemy"))
        {
            other.GetComponent<Enemy>().OnDamaged(10);
            Destroy(gameObject);
        }
    }
}
