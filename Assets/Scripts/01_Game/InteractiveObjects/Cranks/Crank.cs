using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crank : MonoBehaviour
{
    bool isInteractable = true;
    bool isPlayerIn = false;

    [SerializeField] GameObject interactPfb;
    [SerializeField] Sprite CrankUp;
    [SerializeField] Sprite CrankDown;
    GameObject interactUI;

    Canvas Fixedcanvas;
    Collider2D collider;
    SpriteRenderer spriteRenderer;

    Vector3 InteractPosition;
    [SerializeField] float positionOffset;


    private void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        collider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        interactPfb = Resources.Load<GameObject>("Prefabs/01_Game/E");
        CrankUp = Resources.Load<Sprite>("Images/crank-up");
        CrankDown = Resources.Load<Sprite>("Images/crank-down");

        InteractPosition = collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2 + interactPfb.transform.localScale.y / 2 + positionOffset, 0);
    }


    private void Update()
    {
        ShowInteractUI();

        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn)
        {
            interact();
        }
    }

    virtual protected void interact()
    {
        isInteractable = false;
        spriteRenderer.sprite = CrankDown;

        StartCoroutine(Push());

    }

    IEnumerator Push()
    {
        yield return new WaitForSeconds(2);

        isInteractable = true;
        spriteRenderer.sprite = CrankUp;
    }

    void ShowInteractUI()
    {
        if (isPlayerIn && isInteractable)
        {
            if (interactUI == null)
                interactUI = Instantiate(interactPfb, InteractPosition, Quaternion.identity, Fixedcanvas.transform);
        }
        else
        {
            if (interactUI != null)
                Destroy(interactUI);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player") && isInteractable)
        {
            // interactUI = Instantiate(interactPfb, InteractPosition, Quaternion.identity, Fixedcanvas.transform);
            isPlayerIn = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            isPlayerIn = false;

            // if (interactUI != null)
            //     Destroy(interactUI);
        }
    }

}
