using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    [SerializeField] String BoxID => gameObject.name;

    [SerializeField] GameObject interactPfb;
    [SerializeField] GameObject ItemInfoPfb;
    protected Canvas Fixedcanvas;
    protected Canvas Canvas;
    GameObject interactUI;
    protected Collider2D collider;
    [SerializeField] float positionOffset;

    [SerializeField] protected bool isInteractable = true;
    bool isPlayerIn;

    protected virtual void Awake()
    {
        ObjectData data = GameData.ObjectDatas.Find(e => e.ID == BoxID);
        if (data == null)
        {
            GameData.ObjectDatas.Add(new ObjectData(BoxID, true));
        }
        else
        {
            if (!data.IsExist) isInteractable = false;
        }

        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, collider.bounds.size, 0);
        if (colliders.Length > 0)
        {
            foreach (Collider2D collider in colliders)
            {
                if (!collider.gameObject.CompareTag("Player"))
                {
                    isInteractable = false;
                    break;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.E) && isPlayerIn && isInteractable)
        {
            interact();
        }
    }

    virtual protected void interact()
    {
        isInteractable = false;
        GameData.ObjectDatas.Find(e => e.ID == BoxID).IsExist = false;
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
            interactUI = Instantiate(interactPfb, collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2 + interactPfb.transform.localScale.y / 2 + positionOffset, 0), Quaternion.identity, Fixedcanvas.transform);
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
