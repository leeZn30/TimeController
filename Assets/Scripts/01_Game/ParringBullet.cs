using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class ParringBullet : MonoBehaviour
{
    Character character;
    public bool isParried = false;
    LineRenderer lineRenderer;
    float interval = 1f; // 3초
    float nextTime = 0f;
    [SerializeField] List<Vector3> poses = new List<Vector3>();
    public CinemachineVirtualCamera cinevirtual;

    private void Awake()
    {
        character = FindObjectOfType<Character>();
        lineRenderer = GetComponent<LineRenderer>();
        poses.Add(transform.position);
        cinevirtual = FindObjectOfType<CinemachineVirtualCamera>();
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

        // 상위 불렛에 따라 일직선일지 유도탄일지 결정
        if (!isParried)
        {
            transform.Translate(Vector3.left * 10f * Time.deltaTime);
        }

    }

    public void parried()
    {
        isParried = true;
        poses.Add(transform.position);
        lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.ToArray());
        StartCoroutine(softCameraZoom());
    }

    IEnumerator followLine()
    {
        Time.timeScale = 0f;
        // 오브젝트 회전

        for (int i = lineRenderer.positionCount - 1; i >= 0; i--)
        {
            while (transform.position != lineRenderer.GetPosition(i))
            {
                transform.position = Vector2.MoveTowards(transform.position, lineRenderer.GetPosition(i), 15f * Time.unscaledDeltaTime);

                yield return null;
            }
            yield return null;
        }
        cinevirtual.m_Lens.OrthographicSize = 6f;
        cinevirtual.Follow = character.transform;
        Time.timeScale = 1;
    }

    IEnumerator softCameraZoom()
    {
        cinevirtual.LookAt = transform;
        cinevirtual.Follow = transform;

        while (cinevirtual.m_Lens.OrthographicSize > 1.5f)
        {
            cinevirtual.m_Lens.OrthographicSize -= Time.unscaledDeltaTime * 20f;

            yield return null;
        }

        StartCoroutine(followLine());
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
