using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Item : MonoBehaviour
{
    Animator anim;
    [SerializeField] protected TextMeshProUGUI TextPfb;
    protected TextMeshProUGUI text;
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
        text = Instantiate(TextPfb, transform.position += Vector3.up, Quaternion.identity, GameObject.Find("Canvas").transform);
        text.SetText(comment);

        StartCoroutine(ShowItemTextUI());
    }

    protected IEnumerator ShowItemTextUI()
    {

        float duration = 0f;
        while (duration < 0.8f)
        {
            duration += Time.deltaTime;

            text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), Time.deltaTime);
            text.gameObject.transform.position += Vector3.up * Time.deltaTime;

            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        Destroy(text.gameObject);
        Destroy(gameObject);
    }
}
