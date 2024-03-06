using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    [Header("Game Info")]
    [SerializeField] static int _stage;
    public static int Stage { get { return _stage; } set { _stage = value; } }

    [Header("PlayerData")]
    [SerializeField] static float _hp = 16;
    public static float Hp { get { return _hp; } set { _hp = value; } }

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}
