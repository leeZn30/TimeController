using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    Canvas canvas;
    GameObject go;
    public GameObject info;

    bool isPlayerIn;

    private void Awake()
    {
        canvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn)
        {
            info.SetActive(true);
            StartCoroutine(delay());
        }
    }

    IEnumerator delay()
    {
        yield return new WaitForSeconds(5f);

        info.SetActive(false);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            go = Instantiate(Resources.Load<GameObject>("Prefabs/상호작용 UI"), transform.position + new Vector3(0, 3, 0), Quaternion.identity, canvas.transform);
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            isPlayerIn = false;

            if (go != null)
                Destroy(go);
        }

    }
}