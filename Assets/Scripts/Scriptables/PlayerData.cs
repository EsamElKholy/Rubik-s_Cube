using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct LevelTime
{
    public float seconds;
    public float minutes;
    public float hours;

    public override string ToString()
    {
        string res = "";
        string sec = ((int)seconds).ToString();
        if (sec.Length == 1)
        {
            sec = "0" + sec;
        }

        string min = ((int)minutes).ToString();
        if (min.Length == 1)
        {
            min = "0" + min;
        }

        res = ((int)hours).ToString() + ":" + min + ":" + sec;

        return res;
    }
}

[CreateAssetMenu(fileName = "New Player Data", menuName = "Game/Player Data")]
public class PlayerData : ScriptableObject
{
    public int minCubeSize;

    [HideInInspector]
    public string highestScore;
    [HideInInspector]
    public string score;
    [HideInInspector]
    public LevelTime time;
    [HideInInspector]
    public int cubeSize = 2;
    [HideInInspector]
    public RubikCubeFaceColors currentColors = new RubikCubeFaceColors();
    [HideInInspector]
    public Vector3 initialCamPos;
    [HideInInspector]
    public Vector3 initialCamRot;

    public void IncreamentTime(float dt)
    {
        if (time.seconds >= 60)
        {
            time.minutes++;
            time.seconds = 0;
        }

        if (time.minutes >= 60)
        {
            time.hours++;
            time.minutes = 0;
        }

        time.seconds += dt;
    }

    public void WipeOutData()
    {
        time = new LevelTime();
        score = "";
        currentColors = new RubikCubeFaceColors();
    }

    public void SetCubeSizeMode(int mode)
    {
        SetCubeSize(mode + minCubeSize);
    }

    public void SetCubeSizeMode(Dropdown mode)
    {
        SetCubeSize(mode.value + minCubeSize);
    }

    public void SetCubeSize(int size)
    {
        cubeSize = size;
    }

    public bool CanContinue()
    {
        if (time.seconds > 2 && currentColors.frontFaceColors.Count > 0)
        {
            return true;
        }

        return false;
    }

    public float GetSecondsElapsed()
    {
        float res = time.seconds + (time.minutes * 60) + (time.hours * 60);

        return res;
    }
}
