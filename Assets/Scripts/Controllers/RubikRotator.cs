using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikRotator : MonoBehaviour
{
    public static RubikRotator Instance;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Reset the cube to its original position and orientation
    /// </summary>
    /// <param name="cube"></param>
    public void ResetCube(GameObject cube)
    {
        if (cube.name.Equals(RubikGenerator.Instance.cubeRoot.name))
        {
            for (int i = 0; i < cube.transform.childCount; i++)
            {
                if (cube.transform.GetChild(i).name.Equals("Slice"))
                {
                    continue;
                }

                int x = int.Parse(cube.transform.GetChild(i).name[0].ToString()) - 1;
                int y = int.Parse(cube.transform.GetChild(i).name[1].ToString()) - 1;
                int z = int.Parse(cube.transform.GetChild(i).name[2].ToString()) - 1;

                var pos = RubikGenerator.Instance.cubesPositions[x][y][z];

                cube.transform.GetChild(i).rotation = Quaternion.identity;
                cube.transform.GetChild(i).position = pos;

                for (int j = 0; j < cube.transform.GetChild(i).childCount; j++)
                {
                    cube.transform.GetChild(i).GetChild(j).transform.rotation = Quaternion.identity;
                }
            }
        }
        else
        {
            int x = int.Parse(cube.name[0].ToString()) - 1;
            int y = int.Parse(cube.name[1].ToString()) - 1;
            int z = int.Parse(cube.name[2].ToString()) - 1;

            var pos = RubikGenerator.Instance.cubesPositions[x][y][z];

            cube.transform.rotation = Quaternion.identity;
            cube.transform.position = pos;

            for (int i = 0; i < cube.transform.childCount; i++)
            {
                cube.transform.GetChild(i).transform.rotation = Quaternion.identity;
            }
        }
    }

    /// <summary>
    /// Given the axis of rotation and the rotation direction:
    /// 1- get each face and cache its colors
    /// 2- overwrite the face colors with the appropriate color list (CCW or CW)
    /// </summary>
    /// <param name="axis"></param>
    /// <param name="oldAxis"></param>
    /// <param name="direction"></param>
    public void RotateTiles(AXIS axis, float direction)
    {
        List<Quaternion> tileRotations = new List<Quaternion>();
        for (int j = 0; j < RubikGenerator.Instance.GetSlice().transform.childCount; j++)
        {
            var cube = RubikGenerator.Instance.GetSlice().transform.GetChild(j);
            ResetCube(cube.gameObject);
        }

        /*
         ff => front face
         ffc => front face color
         flf => left side of front face
         flfc => color of left side of front face            
         */

        switch (axis)
        {
            case AXIS.X:
                {
                    if (-direction > 0)
                    {
                        bool left = false;

                        var ff = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = RubikCubeManager.Instance.GetFaceColors(ff);
                        if (RubikCubeManager.Instance.GetFaceTiles(ff, RubikCubeFaces.CubeFace.Left).Count > 0)
                        {
                            left = true;
                        }

                        var flf = RubikCubeManager.Instance.GetFaceTiles(ff, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var flfc = RubikCubeManager.Instance.GetFaceColors(flf);

                        var bf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = RubikCubeManager.Instance.GetFaceColors(bf);
                        var blf = RubikCubeManager.Instance.GetFaceTiles(bf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var blfc = RubikCubeManager.Instance.GetFaceColors(blf);

                        var uf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = RubikCubeManager.Instance.GetFaceColors(uf);
                        var ulf = RubikCubeManager.Instance.GetFaceTiles(uf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var ulfc = RubikCubeManager.Instance.GetFaceColors(ulf);

                        var df = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = RubikCubeManager.Instance.GetFaceColors(df);
                        var dlf = RubikCubeManager.Instance.GetFaceTiles(df, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var dlfc = RubikCubeManager.Instance.GetFaceColors(dlf);

                        RubikCubeManager.Instance.SetFaceColors(ff, ufc);
                        RubikCubeManager.Instance.SetFaceColors(uf, bfc);
                        RubikCubeManager.Instance.SetFaceColors(bf, dfc);
                        RubikCubeManager.Instance.SetFaceColors(df, ffc);

                        RubikCubeManager.Instance.SetFaceColors(flf, ulfc);
                        RubikCubeManager.Instance.SetFaceColors(ulf, blfc);
                        RubikCubeManager.Instance.SetFaceColors(blf, dlfc);
                        RubikCubeManager.Instance.SetFaceColors(dlf, flfc);
                    }
                    else
                    {
                        bool left = false;

                        var ff = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = RubikCubeManager.Instance.GetFaceColors(ff);
                        if (RubikCubeManager.Instance.GetFaceTiles(ff, RubikCubeFaces.CubeFace.Left).Count > 0)
                        {
                            left = true;
                        }

                        var flf = RubikCubeManager.Instance.GetFaceTiles(ff, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var flfc = RubikCubeManager.Instance.GetFaceColors(flf);

                        var bf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = RubikCubeManager.Instance.GetFaceColors(bf);
                        var blf = RubikCubeManager.Instance.GetFaceTiles(bf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var blfc = RubikCubeManager.Instance.GetFaceColors(blf);

                        var uf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = RubikCubeManager.Instance.GetFaceColors(uf);
                        var ulf = RubikCubeManager.Instance.GetFaceTiles(uf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var ulfc = RubikCubeManager.Instance.GetFaceColors(ulf);

                        var df = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = RubikCubeManager.Instance.GetFaceColors(df);
                        var dlf = RubikCubeManager.Instance.GetFaceTiles(df, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var dlfc = RubikCubeManager.Instance.GetFaceColors(dlf);

                        RubikCubeManager.Instance.SetFaceColors(ff, dfc);
                        RubikCubeManager.Instance.SetFaceColors(df, bfc);
                        RubikCubeManager.Instance.SetFaceColors(bf, ufc);
                        RubikCubeManager.Instance.SetFaceColors(uf, ffc);

                        RubikCubeManager.Instance.SetFaceColors(flf, dlfc);
                        RubikCubeManager.Instance.SetFaceColors(dlf, blfc);
                        RubikCubeManager.Instance.SetFaceColors(blf, ulfc);
                        RubikCubeManager.Instance.SetFaceColors(ulf, flfc);
                    }
                }
                break;
            case AXIS.Y:
                {
                    if (-direction > 0)
                    {
                        bool up = false;

                        var ff = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = RubikCubeManager.Instance.GetFaceColors(ff);
                        if (RubikCubeManager.Instance.GetFaceTiles(ff, RubikCubeFaces.CubeFace.Up).Count > 0)
                        {
                            up = true;
                        }

                        var fuf = RubikCubeManager.Instance.GetFaceTiles(ff, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var fufc = RubikCubeManager.Instance.GetFaceColors(fuf);

                        var bf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = RubikCubeManager.Instance.GetFaceColors(bf);
                        var buf = RubikCubeManager.Instance.GetFaceTiles(bf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var bufc = RubikCubeManager.Instance.GetFaceColors(buf);

                        var rf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = RubikCubeManager.Instance.GetFaceColors(rf);
                        var ruf = RubikCubeManager.Instance.GetFaceTiles(rf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var rufc = RubikCubeManager.Instance.GetFaceColors(ruf);

                        var lf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = RubikCubeManager.Instance.GetFaceColors(lf);
                        var luf = RubikCubeManager.Instance.GetFaceTiles(lf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var lufc = RubikCubeManager.Instance.GetFaceColors(luf);

                        RubikCubeManager.Instance.SetFaceColors(ff, lfc);
                        RubikCubeManager.Instance.SetFaceColors(lf, bfc);
                        RubikCubeManager.Instance.SetFaceColors(bf, rfc);
                        RubikCubeManager.Instance.SetFaceColors(rf, ffc);

                        RubikCubeManager.Instance.SetFaceColors(fuf, lufc);
                        RubikCubeManager.Instance.SetFaceColors(luf, bufc);
                        RubikCubeManager.Instance.SetFaceColors(buf, rufc);
                        RubikCubeManager.Instance.SetFaceColors(ruf, fufc);
                    }
                    else
                    {
                        bool up = false;

                        var ff = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = RubikCubeManager.Instance.GetFaceColors(ff);
                        if (RubikCubeManager.Instance.GetFaceTiles(ff, RubikCubeFaces.CubeFace.Up).Count > 0)
                        {
                            up = true;
                        }

                        var fuf = RubikCubeManager.Instance.GetFaceTiles(ff, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var fufc = RubikCubeManager.Instance.GetFaceColors(fuf);

                        var bf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = RubikCubeManager.Instance.GetFaceColors(bf);
                        var buf = RubikCubeManager.Instance.GetFaceTiles(bf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var bufc = RubikCubeManager.Instance.GetFaceColors(buf);

                        var rf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = RubikCubeManager.Instance.GetFaceColors(rf);
                        var ruf = RubikCubeManager.Instance.GetFaceTiles(rf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var rufc = RubikCubeManager.Instance.GetFaceColors(ruf);

                        var lf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = RubikCubeManager.Instance.GetFaceColors(lf);
                        var luf = RubikCubeManager.Instance.GetFaceTiles(lf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var lufc = RubikCubeManager.Instance.GetFaceColors(luf);

                        RubikCubeManager.Instance.SetFaceColors(ff, rfc);
                        RubikCubeManager.Instance.SetFaceColors(rf, bfc);
                        RubikCubeManager.Instance.SetFaceColors(bf, lfc);
                        RubikCubeManager.Instance.SetFaceColors(lf, ffc);

                        RubikCubeManager.Instance.SetFaceColors(fuf, rufc);
                        RubikCubeManager.Instance.SetFaceColors(ruf, bufc);
                        RubikCubeManager.Instance.SetFaceColors(buf, lufc);
                        RubikCubeManager.Instance.SetFaceColors(luf, fufc);
                    }
                }
                break;
            case AXIS.Z:
                {
                    if (-direction > 0)
                    {
                        bool front = false;

                        var uf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = RubikCubeManager.Instance.GetFaceColors(uf);
                        if (RubikCubeManager.Instance.GetFaceTiles(uf, RubikCubeFaces.CubeFace.Front).Count > 0)
                        {
                            front = true;
                        }

                        var uff = RubikCubeManager.Instance.GetFaceTiles(uf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var uffc = RubikCubeManager.Instance.GetFaceColors(uff);

                        var df = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = RubikCubeManager.Instance.GetFaceColors(df);
                        var dff = RubikCubeManager.Instance.GetFaceTiles(df, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var dffc = RubikCubeManager.Instance.GetFaceColors(dff);

                        var rf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = RubikCubeManager.Instance.GetFaceColors(rf);
                        var rff = RubikCubeManager.Instance.GetFaceTiles(rf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var rffc = RubikCubeManager.Instance.GetFaceColors(rff);

                        var lf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = RubikCubeManager.Instance.GetFaceColors(lf);
                        var lff = RubikCubeManager.Instance.GetFaceTiles(lf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var lffc = RubikCubeManager.Instance.GetFaceColors(lff);

                        RubikCubeManager.Instance.SetFaceColors(uf, lfc);
                        RubikCubeManager.Instance.SetFaceColors(lf, dfc);
                        RubikCubeManager.Instance.SetFaceColors(df, rfc);
                        RubikCubeManager.Instance.SetFaceColors(rf, ufc);

                        RubikCubeManager.Instance.SetFaceColors(uff, lffc);
                        RubikCubeManager.Instance.SetFaceColors(lff, dffc);
                        RubikCubeManager.Instance.SetFaceColors(dff, rffc);
                        RubikCubeManager.Instance.SetFaceColors(rff, uffc);
                    }
                    else
                    {
                        bool front = false;

                        var uf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = RubikCubeManager.Instance.GetFaceColors(uf);
                        if (RubikCubeManager.Instance.GetFaceTiles(uf, RubikCubeFaces.CubeFace.Front).Count > 0)
                        {
                            front = true;
                        }

                        var uff = RubikCubeManager.Instance.GetFaceTiles(uf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var uffc = RubikCubeManager.Instance.GetFaceColors(uff);

                        var df = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = RubikCubeManager.Instance.GetFaceColors(df);
                        var dff = RubikCubeManager.Instance.GetFaceTiles(df, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var dffc = RubikCubeManager.Instance.GetFaceColors(dff);

                        var rf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = RubikCubeManager.Instance.GetFaceColors(rf);
                        var rff = RubikCubeManager.Instance.GetFaceTiles(rf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var rffc = RubikCubeManager.Instance.GetFaceColors(rff);

                        var lf = RubikCubeManager.Instance.GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = RubikCubeManager.Instance.GetFaceColors(lf);
                        var lff = RubikCubeManager.Instance.GetFaceTiles(lf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var lffc = RubikCubeManager.Instance.GetFaceColors(lff);

                        RubikCubeManager.Instance.SetFaceColors(uf, rfc);
                        RubikCubeManager.Instance.SetFaceColors(rf, dfc);
                        RubikCubeManager.Instance.SetFaceColors(df, lfc);
                        RubikCubeManager.Instance.SetFaceColors(lf, ufc);

                        RubikCubeManager.Instance.SetFaceColors(uff, rffc);
                        RubikCubeManager.Instance.SetFaceColors(rff, dffc);
                        RubikCubeManager.Instance.SetFaceColors(dff, lffc);
                        RubikCubeManager.Instance.SetFaceColors(lff, uffc);
                    }
                }
                break;
            case AXIS.NONE:
                break;
            default:
                break;
        }
    }
}
