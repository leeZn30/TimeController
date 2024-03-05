using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet Data", menuName = "Scriptable Object/Bullet Data", order = int.MaxValue)]
public class BulletData : ScriptableObject
{
    [SerializeField]
    private string BulletName;
    public string Name { get { return BulletName; } }

    [SerializeField]
    private int damage;
    public int Damage { get { return damage; } }

    [SerializeField]
    private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }

    [SerializeField]
    private float maxDistance;
    public float MaxDistance { get { return maxDistance; } }

    [SerializeField]
    private float rotateSpeed;
    public float RotateSpeed { get { return rotateSpeed; } }

    [SerializeField]
    private float parryPercent;
    public float ParryPercent { get { return parryPercent; } }

    [SerializeField]
    private Material material;
    public Material Material { get { return material; } }

}
