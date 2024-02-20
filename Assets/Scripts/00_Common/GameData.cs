using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class GameData : Singleton<GameData>
{
    [Header("Game Info")]
    [SerializeField] static int _stage;
    public static int Stage { get { return _stage; } set { _stage = value; } }
}
