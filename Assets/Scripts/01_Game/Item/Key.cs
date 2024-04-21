using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    [SerializeField] Door PairDoor;
    Animator anim;

    protected void Awake()
    {
        PairDoor.PairKeyName = gameObject.name;

        ItemData data = GameData.itemDatas.Find(x => x.ID == gameObject.name);
        if (data != null)
        {
            if (data.IsGain)
            {
                PairDoor.isGainItem = true;
                Destroy(gameObject);
            }
        }
        else
        {
            ItemData itemData = new ItemData(gameObject.name, false);
            GameData.itemDatas.Add(itemData);
        }

        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            PairDoor.Unlock();
            anim.SetTrigger("Collected");
        }
    }

    void DoDestroy()
    {
        Destroy(gameObject);
    }

}
