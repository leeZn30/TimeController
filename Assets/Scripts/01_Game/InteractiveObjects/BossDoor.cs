using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class BossDoor : MonoBehaviour
{
    [SerializeField] bool isOpen = true;
    [SerializeField] protected string NextScene;
    bool isPlayerIn = false;

    [SerializeField] GameObject interactPfb;
    Vector3 InteractPosition;

    GameObject interactUI;
    protected Canvas Fixedcanvas;
    protected Collider2D collider;
    SpriteRenderer sprite;

    void Awake()
    {
        Fixedcanvas = GameObject.Find("FixedCanvas").GetComponent<Canvas>();
        collider = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();

        InteractPosition = collider.bounds.center + new Vector3(0, collider.bounds.size.y / 2 + interactPfb.transform.localScale.y / 2, 0);

        if (!isOpen)
        {
            sprite.color = Color.gray;
        }
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
        if (isOpen)
        {
            SoundManager.Instance.PlaySFX(AudioType.BossDoor, "Enter");

            SceneChanger.MakeSceneHandOverData(gameObject.name);
            SceneChanger.LoadSceneByDoor(NextScene);
        }
    }


    public void Open()
    {
        isOpen = true;

        if (sprite.color != Color.white)
            sprite.color = Color.white;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player") && isOpen)
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
