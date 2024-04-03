using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    Animator anim;
    protected string comment;

    virtual protected void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            anim.SetTrigger("Collected");
            OperateItem();
        }
    }

    virtual protected void OperateItem()
    {
        FixedUIManager.Instance.ShowText(comment, new Vector2(transform.localPosition.x, transform.localPosition.y + transform.localScale.y * 0.8f));
        Destroy(gameObject);
    }

}
