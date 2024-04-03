using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
struct BulletInfo
{
    public string Name;
    public Bullet Prefab;
    public int Count;
}

public class BulletManager : Singleton<BulletManager>
{

    [Header("Bullet 정보")]
    [SerializeField] List<BulletInfo> BulletInfos = new List<BulletInfo>();

    static Dictionary<string, Queue<Bullet>> Bullets;

    private void Awake()
    {
        // Reload를 대비해서 awake할때 선언하기
        Bullets = new Dictionary<string, Queue<Bullet>>();

        foreach (BulletInfo bi in BulletInfos)
        {
            Bullets.TryAdd(bi.Name, ObjectPool.CreateQueue<Bullet>(bi.Count, bi.Prefab, transform));
        }
    }

    public static Bullet TakeOutBullet(string name, Vector2 position)
    {
        Bullet bullet = Bullets[name].Dequeue();
        bullet.transform.position = position;
        bullet.gameObject.SetActive(true);
        bullet.Init();

        return bullet;
    }

    public static void InsertBullet(Bullet bullet)
    {
        bullet.GetComponent<Parriable>().enabled = false;

        bullet.GetComponent<LineRenderer>().positionCount = 0;
        bullet.transform.rotation = Quaternion.identity;
        bullet.gameObject.SetActive(false);

        Bullets[bullet.BulletName].Enqueue(bullet);
    }
}
