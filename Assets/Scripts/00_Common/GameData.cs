using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    [Header("Game Info")]
    static int _stage;
    public static int Stage { get { return _stage; } set { _stage = value; } }
    static int _checkpoint = -1;
    public static int CheckPoint { get { return _checkpoint; } set { _checkpoint = value; } }

    [Header("PlayerData")]
    static float _hp = 16;
    public static float Hp { get { return _hp; } set { _hp = value; } }
    static bool _teleportActive;
    public static bool TeleportActive;
    static bool _rewindActive;
    public static bool RewindActive;
    static bool _slowActive;
    public static bool SlowActive;

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}
