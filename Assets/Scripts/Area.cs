using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum Type
{
    vertical, horizontal
}

public class Area : MonoBehaviour
{
    [SerializeField] Type type;
    [SerializeField] float[] CameraPoses = new float[2];
    Character player;

    private void Awake()
    {
        player = FindObjectOfType<Character>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            Rigidbody2D rigid = player.GetComponent<Rigidbody2D>();
            Vector3 targetPose;
            if ((int)type == 1)
            {
                if (rigid.velocity.x < 0)
                {
                    targetPose = new Vector3(CameraPoses[0], Camera.main.transform.position.y, Camera.main.transform.position.z);
                }
                else
                {
                    targetPose = new Vector3(CameraPoses[1], Camera.main.transform.position.y, Camera.main.transform.position.z);
                }
            }
            else
            {
                if (rigid.velocity.y < 0)
                {
                    targetPose = new Vector3(Camera.main.transform.position.x, CameraPoses[0], Camera.main.transform.position.z);
                }
                else
                {
                    targetPose = new Vector3(Camera.main.transform.position.x, CameraPoses[1], Camera.main.transform.position.z);
                }
            }

            StartCoroutine(CameraMove(targetPose));
        }
    }

    IEnumerator CameraMove(Vector3 targetPose)
    {
        // 나중에 Lerp 써서 더 부드럽게
        float duration = 0.0f;
        float time = 0.3f;
        while (duration < time)
        {
            duration += Time.deltaTime;
            Camera.main.transform.Translate(targetPose * Time.deltaTime * 5);
            yield return null;
        }

        Camera.main.transform.position = targetPose;
        Debug.Log("Moving End");
    }

}
