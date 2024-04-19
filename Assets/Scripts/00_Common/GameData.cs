using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : Singleton<GameData>
{
    [Header("Game Info")]
    static int _stage;
    public static int Stage { get { return _stage; } set { _stage = value; } }
    static int _reviveScene;
    public static int ReviveScene { get { return _reviveScene; } set { _reviveScene = value; } }
    static int _checkpoint = -1;
    public static int CheckPoint { get { return _checkpoint; } set { _checkpoint = value; } }
    static int _ghosts;
    public static int Ghosts { get { return _ghosts; } set { _ghosts = value; } }

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
