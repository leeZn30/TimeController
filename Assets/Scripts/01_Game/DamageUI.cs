using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageUI : Singleton<DamageUI>
{
    [SerializeField] TextMeshProUGUI DamagePfb;

    Queue<TextMeshProUGUI> Damages;

    private void Awake()
    {
        Damages = ObjectPool.CreateQueue<TextMeshProUGUI>(5, DamagePfb, transform);
    }

    public void ShowDamage(int damage, Vector2 position, bool isCritical = false)
    {
        TextMeshProUGUI go = Damages.Dequeue();
        go.transform.position = position + Vector2.up;
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

        text.color = new Color(219, 100, 54);
        Damages.Enqueue(text);
        text.gameObject.SetActive(false);
    }
}
