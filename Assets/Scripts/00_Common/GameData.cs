using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : Singleton<GameData>
{
    [Header("Stage Data")]
    static int _stage;
    public static int Stage { get { return _stage; } set { _stage = value; } }
    static int _reviveScene = -1;
    public static int ReviveScene { get { return _reviveScene; } set { _reviveScene = value; } }
    static int _checkpoint = -1;
    public static int CheckPoint { get { return _checkpoint; } set { _checkpoint = value; } }
    static int _ghosts;
    public static int Ghosts { get { return _ghosts; } set { _ghosts = value; } }
    static string _door;
    public static string Door { get { return _door; } set { _door = value; } }

    [Header("Player Data")]
    static float _hp = 16;
    public static float Hp { get { return _hp; } set { _hp = value; } }
    static bool _teleportActive;
    public static bool TeleportActive;
    static bool _rewindActive;
    public static bool RewindActive;
    static bool _slowActive;
    public static bool SlowActive;
    static int _nowGhosts;
    public static int NowGhosts { get { return _nowGhosts; } set { _nowGhosts = value; } }


    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

}
