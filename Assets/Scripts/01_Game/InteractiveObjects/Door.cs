using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    bool isGainItem = false;
    bool isInteractable = false;
    bool isPlayerIn = false;

    [SerializeField] GameObject NoItemPfb;
    [SerializeField] GameObject HaveItemPfb;
    [SerializeField] GameObject interactPfb;

    Canvas Fixedcanvas;
    Collider2D collider;

    GameObject interactUI;
    GameObject HaveItemUI;
    GameObject NoItemUI;

    Vector3 ItemPosition;
    Vector3 InteractPosition;


    private void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        collider = GetComponent<BoxCollider2D>();

        ItemPosition = collider.bounds.center;
        InteractPosition = collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2 + interactPfb.transform.localScale.y / 2, 0);

        if (isGainItem)
            HaveItemUI = Instantiate(HaveItemPfb, ItemPosition, Quaternion.identity, Fixedcanvas.transform);
        else
            NoItemUI = Instantiate(NoItemPfb, ItemPosition, Quaternion.identity, Fixedcanvas.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn)
        {
            interact();
        }
    }

    void interact()
    {
        if (isInteractable)
        {
            isInteractable = false;
            SceneManager.LoadScene("Tower1");
        }
        else
        {
            StartCoroutine(Vibrate());
        }
    }

    IEnumerator Vibrate()
    {
        Vector3 startPosition = transform.position;
        float vibrationStrength = 0.1f;
        float vibrationSpeed = 20f;

        float duration = 0f;
        while (duration < 0.5f)
        {
            duration += Time.deltaTime;

            // 진동 강도와 속도에 따라 새로운 위치 계산
            float newPositionX = startPosition.x + Mathf.Sin(Time.time * vibrationSpeed) * vibrationStrength;
            float newPositionY = startPosition.y + Mathf.Sin(Time.time * vibrationSpeed * 1.1f) * vibrationStrength;
            float newPositionZ = startPosition.z + Mathf.Sin(Time.time * vibrationSpeed * 1.2f) * vibrationStrength;

            // 새로운 위치 적용
            transform.position = new Vector3(newPositionX, newPositionY, newPositionZ);
            yield return null;
        }

        transform.position = startPosition;
    }

    public void Open()
    {
        isInteractable = true;
        Destroy(NoItemUI);
        HaveItemUI = Instantiate(HaveItemPfb, ItemPosition, Quaternion.identity, Fixedcanvas.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            interactUI = Instantiate(interactPfb, InteractPosition, Quaternion.identity, Fixedcanvas.transform);
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            isPlayerIn = false;

            if (interactUI != null)
                Destroy(interactUI);
        }
    }

}
