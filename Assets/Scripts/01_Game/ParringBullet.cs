using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Cinemachine;

public class ParringBullet : MonoBehaviour
{
    public CinemachineVirtualCamera cinevirtual;
    Character character;
    public bool isParried = false;
    LineRenderer lineRenderer;
    float interval = 1f; // 3초
    float nextTime = 0f;
    [SerializeField] List<Vector3> poses = new List<Vector3>();

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
        // if (Time.time > nextTime && !isParried)
        // {
        //     nextTime = Time.time + interval;

        //     if (poses.Count != 0 && poses.Last() != transform.position)
        //         poses.Add(transform.position);
        // }

        // 상위 불렛에 따라 일직선일지 유도탄일지 결정
        if (!isParried)
        {
            if (poses.Count != 0 && poses.Last() != transform.position)
                poses.Add(transform.position);

            transform.Translate(Vector3.left * 10f * Time.deltaTime);
        }

    }

    public void stopBullet()
    {
        isParried = true;
    }

    public void parried()
    {
        poses.Add(transform.position);
        lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.ToArray());

        cinevirtual.LookAt = transform;
        cinevirtual.Follow = transform;
        StartCoroutine(followLine());
    }

    IEnumerator followLine()
    {
        // 오브젝트 회전

        for (int i = lineRenderer.positionCount - 1; i >= 0; i--)
        {
            while (transform.position != lineRenderer.GetPosition(i))
            {
                transform.position = Vector2.MoveTowards(transform.position, lineRenderer.GetPosition(i), 30f * Time.unscaledDeltaTime);

                yield return null;
            }
            yield return null;
        }
        cinevirtual.m_Lens.OrthographicSize = 6f;
        cinevirtual.Follow = character.transform;
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
