using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParringBullet : MonoBehaviour
{
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
    }
    // Start is called before the first frame update
    void Start()
    {

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
            transform.Translate(Vector3.left * 10f * Time.unscaledDeltaTime);
        }

    }


    public void parried()
    {
        isParried = true;
        lineRenderer.positionCount = poses.Count;
        lineRenderer.SetPositions(poses.ToArray());
        StartCoroutine(softCameraZoom());
        // StartCoroutine(followLine());
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
                Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
                yield return null;
            }
            yield return null;
        }

        Camera.main.orthographicSize = 5f;
        Camera.main.transform.position = new Vector3(0, 0, -10);
        Time.timeScale = 1;
    }

    IEnumerator softCameraZoom()
    {
        while (Camera.main.orthographicSize > 1.5f)
        {
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
            Camera.main.orthographicSize -= Time.unscaledDeltaTime * 20f;
            yield return null;
        }

        StartCoroutine(followLine());
    }
}
