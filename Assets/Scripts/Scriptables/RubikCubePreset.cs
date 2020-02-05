using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RubikCubeFaces
{
    public enum CubeFace
    {

        Front,
        Back,
        Right,
        Left,
        Up,
        Down
    }

    public CubeFace Face;
    public Color Color;
}

[CreateAssetMenu(fileName = "New Rubik Cube Preset", menuName = "Rubik Cube/Rubik Cube Preset", order = 0)]
public class RubikCubePreset : ScriptableObject
{
    private void OnEnable()
    {
        FrontFace.Face = RubikCubeFaces.CubeFace.Front;
        BackFace.Face = RubikCubeFaces.CubeFace.Back;
        RightFace.Face = RubikCubeFaces.CubeFace.Right;
        LeftFace.Face = RubikCubeFaces.CubeFace.Left;
        UpFace.Face = RubikCubeFaces.CubeFace.Up;
        DownFace.Face = RubikCubeFaces.CubeFace.Down;
    }

    public RubikCubeFaces FrontFace;
    public RubikCubeFaces BackFace;
    public RubikCubeFaces RightFace;
    public RubikCubeFaces LeftFace;
    public RubikCubeFaces UpFace;
    public RubikCubeFaces DownFace;
}
