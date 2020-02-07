using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LevelTime
{
    public float seconds;
    public float minutes;
    public float hours;
}

[CreateAssetMenu(fileName = "New Player Data", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    [HideInInspector]
    public string highestScore;
    [HideInInspector]
    public string score;
    [HideInInspector]
    public LevelTime time;
    [HideInInspector]
    public int cubeSize;
    [HideInInspector]
    public RubikCubeFaceColors currentColors = new RubikCubeFaceColors();
}
