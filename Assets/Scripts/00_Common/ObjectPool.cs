using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public static Queue<T> CreateQueue<T>(int Count, GameObject Prefab, Transform Parent)
    {
        Queue<T> queue = new Queue<T>();
        for (int i = 0; i < Count; i++)
        {
            GameObject go = Instantiate(Prefab, Parent.position, Quaternion.identity, Parent);
            go.SetActive(false);
            queue.Enqueue(go.GetComponent<T>());
        }

        return queue;
    }
}
