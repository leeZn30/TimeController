using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tower : MonoBehaviour
{
    bool isGainItem = false;
    bool isInteractable = true;
    bool isPlayerIn = false;

    [SerializeField] GameObject NoItemPfb;
    [SerializeField] GameObject HaveItemPfb;
    [SerializeField] GameObject interactPfb;

    Canvas Fixedcanvas;
    Collider2D collider;

    GameObject interactUI;
    GameObject HaveItemUI;
    GameObject NoItemUI;


    private void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        collider = GetComponent<BoxCollider2D>();

        if (isGainItem)
            HaveItemUI = Instantiate(HaveItemPfb, collider.bounds.center + new Vector3(0, 1, 0), Quaternion.identity, Fixedcanvas.transform);
        else
            NoItemUI = Instantiate(NoItemPfb, collider.bounds.center + new Vector3(0, 1, 0), Quaternion.identity, Fixedcanvas.transform);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn && isInteractable)
        {
            interact();
        }
    }

    void interact()
    {
        isInteractable = false;
        SceneManager.LoadScene("Tower1");
    }

    public void Open()
    {
        isInteractable = true;
        Destroy(NoItemUI);
        HaveItemUI = Instantiate(HaveItemPfb, collider.bounds.center + new Vector3(0, 1, 0), Quaternion.identity, Fixedcanvas.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player") && isInteractable)
        {
            interactUI = Instantiate(interactPfb, collider.bounds.center + new Vector3(0, 2, 0), Quaternion.identity, Fixedcanvas.transform);
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
