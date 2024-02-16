using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class ObjectPool : Singleton<ObjectPool>
{
    public static Queue<T> CreateQueue<T>(int Count, T Prefab, Transform Parent = null) where T : MonoBehaviour
    {
        Queue<T> queue = new Queue<T>();
        for (int i = 0; i < Count; i++)
        {
            T go = null;
            if (Parent != null)
                go = Instantiate(Prefab, Parent.position, Quaternion.identity, Parent);
            else
                go = Instantiate(Prefab, new Vector2(0, 0), quaternion.identity);
            go.gameObject.SetActive(false);
            queue.Enqueue(go.GetComponent<T>());
        }

        return queue;
    }

}
