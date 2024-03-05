using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] GameObject interactPfb;
    [SerializeField] GameObject ItemInfoPfb;
    Canvas Fixedcanvas;
    Canvas Canvas;
    GameObject interactUI;

    bool isInteractable = true;
    bool isPlayerIn;

    private void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn && isInteractable)
        {
            interact();
        }
    }

    virtual protected void interact()
    {
        isInteractable = false;
    }

    protected void ShowItem()
    {
        Destroy(interactUI);
        GameManager.pushESC(Instantiate(ItemInfoPfb, Canvas.transform));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player") && isInteractable)
        {
            interactUI = Instantiate(interactPfb, transform.position + new Vector3(0, 2, 0), Quaternion.identity, Fixedcanvas.transform);
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
