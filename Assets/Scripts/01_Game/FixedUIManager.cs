using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class FixedUIManager : Singleton<FixedUIManager>
{
    [SerializeField] TextMeshProUGUI DamagePfb;
    [SerializeField] TextMeshProUGUI TextPfb;

    Queue<TextMeshProUGUI> Damages;

    private void Awake()
    {
        Damages = ObjectPool.CreateQueue<TextMeshProUGUI>(5, DamagePfb, transform);
    }

    public void ShowText(string comment, Vector2 position)
    {
        TextMeshProUGUI text = Instantiate(TextPfb, position, Quaternion.identity, transform);
        text.SetText(comment);

        StartCoroutine(TextShowing(text));
    }

    IEnumerator TextShowing(TextMeshProUGUI text)
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
    }

    public void ShowDamage(int damage, Vector2 position, bool isCritical = false)
    {
        TextMeshProUGUI go = Damages.Dequeue();
        go.transform.position = position;
        go.SetText(damage.ToString());
        go.gameObject.SetActive(true);

        if (!isCritical)
        {
            go.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            go.transform.GetChild(0).gameObject.SetActive(true);
        }

        StartCoroutine(damageShowing(go));
    }

    IEnumerator damageShowing(TextMeshProUGUI text)
    {
        yield return new WaitForSeconds(0.5f);

        float duration = 0f;
        while (duration < 0.5f)
        {
            duration += Time.deltaTime;

            text.color = Color.Lerp(text.color, new Color(text.color.r, text.color.g, text.color.b, 0), Time.deltaTime);
            text.gameObject.transform.position += Vector3.up * Time.deltaTime;

            yield return null;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
        Damages.Enqueue(text);
        text.gameObject.SetActive(false);
    }
}
