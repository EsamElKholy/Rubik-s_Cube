using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AXIS
{
    X = 0,
    Y = 1,
    Z = 2,
    NONE = 3
}

public class RubikGenerator : MonoBehaviour
{
    public static RubikGenerator Instance;

    public Transform cubeRoot;
    public GameObject cubePrefab;
    public GameObject tilePrefab;
    public RubikCubePreset cubePreset;

    [Range(2, 6)]
    public int size = 2;

    [Range(1, 10)]
    public int scaleFactor = 1;

    [Range(0, 0.5f)]
    public float spacing = 0;

    [Range(0, 0.5f)]
    public float tilePadding = 0;

    private int numOfCubes;
    private int numOfTiles;

    private GameObject slice;
    [HideInInspector]
    public GameObject selecedCube;

    private List<List<List<GameObject>>> cubes = new List<List<List<GameObject>>>();
    private List<List<List<Vector3>>> cubesPositions = new List<List<List<Vector3>>>();
    private List<List<List<Quaternion>>> cubesRotations= new List<List<List<Quaternion>>>();
    private List<GameObject> tiles = new List<GameObject>();

    private float currentAngle;

    private void Awake()
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

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < size; i++)
        {
            cubes.Add(new List<List<GameObject>>());
            cubesPositions.Add(new List<List<Vector3>>());
            cubesRotations.Add(new List<List<Quaternion>>());
            for (int j = 0; j < size; j++)
            {
                cubes[i].Add(new List<GameObject>());
                cubesPositions[i].Add(new List<Vector3>());
                cubesRotations[i].Add(new List<Quaternion>());
                for (int k = 0; k < size; k++)
                {
                    cubes[i][j].Add(null);
                    cubesPositions[i][j].Add(Vector3.zero);
                    cubesRotations[i][j].Add(Quaternion.identity);
                }
            }
        }

        GenerateCube();
        GenerateTiles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateCube()
    {   
        numOfCubes = size * size * size;
        numOfTiles = numOfCubes;

        float offset = size % 2 == 0 ? 0.5f : 0;

        float x = (Mathf.Floor(-size / 2)) + offset;

        for (int i = 0; i < size; i++)
        {
            float y = Mathf.Floor(-size / 2) + offset;

            for (int j = 0; j < size; j++)
            {
                float z = Mathf.Floor(size / 2) - offset;

                for (int k = 0; k < size; k++)
                {
                    Vector3 pos = new Vector3(x, y, z);

                    string cubeName = (i + 1).ToString() + (j + 1).ToString() + (k + 1).ToString();
                    GameObject cube = Instantiate<GameObject>(cubePrefab);
                    cube.name = cubeName;
                    cube.transform.SetParent(cubeRoot);
                    cube.transform.position = pos;
                    cube.transform.localScale = Vector3.one * (1 - spacing);

                    cubes[i][j][k] = cube;
                    cubesPositions[i][j][k] = pos;

                    z -= 1;
                }

                y += 1;
            }

            x += 1;
        }

        slice = new GameObject("Slice");
        slice.transform.SetParent(cubeRoot);
    }

    public void GenerateTiles()
    {
        float scale = 1 - tilePadding;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var tile = Instantiate<GameObject>(tilePrefab);
                tile.transform.SetParent(cubes[i][j][0].transform);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.name = cubes[i][j][0].name + "+Z";
                var tileBody = tile.transform.GetChild(0);
                tileBody.transform.localScale = new Vector3(scale, scale, 0.1f);
                tileBody.transform.localPosition = new Vector3(0, 0, 0.5f);
                tileBody.transform.name = cubes[i][j][0].name;
                tileBody.GetComponent<Renderer>().material.color = cubePreset.BackFace.Color;
                tileBody.gameObject.AddComponent<MarkAsBackFace>();

                var tile1 = Instantiate<GameObject>(tilePrefab);
                tile1.transform.SetParent(cubes[i][j][size - 1].transform);
                tile1.transform.localPosition = Vector3.zero;
                tile1.transform.name = cubes[i][j][size - 1].name + "-Z";

                var tileBody1 = tile1.transform.GetChild(0);
                tileBody1.transform.localScale = new Vector3(scale, scale, 0.1f);
                tileBody1.transform.localPosition = new Vector3(0, 0, -0.5f);
                tileBody1.transform.name = cubes[i][j][size - 1].name;
                tileBody1.GetComponent<Renderer>().material.color = cubePreset.FrontFace.Color;
                tileBody1.gameObject.AddComponent<MarkAsFrontFace>();
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject tile = Instantiate<GameObject>(tilePrefab);
                tile.transform.SetParent(cubes[0][i][j].transform);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.name = cubes[0][i][j].name + "-X";
                Transform tileBody = tile.transform.GetChild(0);
                tileBody.transform.localScale = new Vector3(0.1f, scale, scale);
                tileBody.transform.localPosition = new Vector3(-0.5f, 0, 0);
                tileBody.transform.name = cubes[0][i][j].name;
                tileBody.GetComponent<Renderer>().material.color = cubePreset.LeftFace.Color;
                tileBody.gameObject.AddComponent<MarkAsLeftFace>();

                GameObject tile1 = Instantiate<GameObject>(tilePrefab);
                tile1.transform.SetParent(cubes[size - 1][i][j].transform);
                tile1.transform.localPosition = Vector3.zero;
                tile1.transform.name = cubes[size - 1][i][j].name + "+X";

                Transform tileBody1 = tile1.transform.GetChild(0);
                tileBody1.transform.localScale = new Vector3(0.1f, scale, scale);
                tileBody1.transform.localPosition = new Vector3(0.5f, 0, 0);
                tileBody1.transform.name = cubes[size - 1][i][j].name;
                tileBody1.GetComponent<Renderer>().material.color = cubePreset.RightFace.Color;
                tileBody1.gameObject.AddComponent<MarkAsRightFace>();
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject tile = Instantiate<GameObject>(tilePrefab);
                tile.transform.SetParent(cubes[i][0][j].transform);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.name = cubes[i][0][j].name + "-Y";
                Transform tileBody = tile.transform.GetChild(0);
                tileBody.transform.localScale = new Vector3(scale, 0.1f, scale);
                tileBody.transform.localPosition = new Vector3(0, -0.5f, 0);
                tileBody.transform.name = cubes[i][0][j].name;
                tileBody.GetComponent<Renderer>().material.color = cubePreset.UpFace.Color;
                tileBody.gameObject.AddComponent<MarkAsDownFace>();

                GameObject tile1 = Instantiate<GameObject>(tilePrefab);
                tile1.transform.SetParent(cubes[i][size - 1][j].transform);
                tile1.transform.localPosition = Vector3.zero;
                tile1.transform.name = cubes[i][size - 1][j].name + "+Y";

                Transform tileBody1 = tile1.transform.GetChild(0);
                tileBody1.transform.localScale = new Vector3(scale, 0.1f, scale);
                tileBody1.transform.localPosition = new Vector3(0, 0.5f, 0);
                tileBody1.transform.name = cubes[i][size - 1][j].name;
                tileBody1.GetComponent<Renderer>().material.color = cubePreset.DownFace.Color;
                tileBody1.gameObject.AddComponent<MarkAsUpFace>();
            }
        }
    }

    public GameObject GetSlice(AXIS axis)
    {
        int x =  int.Parse(selecedCube.name[0].ToString()) - 1;
        int y =  int.Parse(selecedCube.name[1].ToString()) - 1;
        int z =  int.Parse(selecedCube.name[2].ToString()) - 1;

        EmptySlice();
        slice.transform.rotation = Quaternion.identity;

        switch (axis)
        {
            case AXIS.Y:
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            var obj = cubes[i][y][j];
                            obj.transform.SetParent(slice.transform);
                        }
                    }
                }
                break;
            case AXIS.X:
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            var obj = cubes[x][i][j];
                            obj.transform.SetParent(slice.transform);
                        }
                    }
                }
                break;
            case AXIS.Z:
                {
                    for (int i = 0; i < size; i++)
                    {
                        for (int j = 0; j < size; j++)
                        {
                            var obj = cubes[i][j][z];
                            obj.transform.SetParent(slice.transform);
                        }
                    }
                }
                break;
            default:
                break;
        }

        return slice;
    }

    public void ResetPosition(GameObject cube)
    {
        int x = int.Parse(cube.name[0].ToString()) - 1;
        int y = int.Parse(cube.name[1].ToString()) - 1;
        int z = int.Parse(cube.name[2].ToString()) - 1;

        var pos = cubesPositions[x][y][z];

        cube.transform.rotation = Quaternion.identity;
        cube.transform.position = pos;

        for (int i = 0; i < cube.transform.childCount; i++)
        {
            cube.transform.GetChild(i).transform.rotation = Quaternion.identity;
        }
    }

    public void SaveRotation(GameObject cube, List<Quaternion> tr)
    {
        int x = int.Parse(cube.name[0].ToString()) - 1;
        int y = int.Parse(cube.name[1].ToString()) - 1;
        int z = int.Parse(cube.name[2].ToString()) - 1;

        cubesRotations[x][y][z] = cube.transform.rotation;

        for (int i = 0; i < cube.transform.childCount; i++)
        {
            tr.Add(cube.transform.GetChild(i).rotation);
        }
    }

    public void RestoreRotation(GameObject cube, List<Quaternion> tr)
    {
        int x = int.Parse(cube.name[0].ToString()) - 1;
        int y = int.Parse(cube.name[1].ToString()) - 1;
        int z = int.Parse(cube.name[2].ToString()) - 1;

        var rot = cubesRotations[x][y][z];

        cube.transform.rotation = rot;

        for (int i = 0; i < cube.transform.childCount; i++)
        {
            cube.transform.GetChild(i).rotation = tr[i];
        }
    }

    public void RotateTiles(AXIS axis, AXIS oldAxis, float direction)
    {
        List<Quaternion> tileRotations = new List<Quaternion>();
        for (int j = 0; j < slice.transform.childCount; j++)
        {
            var cube = slice.transform.GetChild(j);
           // SaveRotation(cube.gameObject, tileRotations);
            ResetPosition(cube.gameObject);
        }

        switch (axis)
        {
            case AXIS.X:
                {
                    if (-direction > 0)
                    {
                        bool left = false;

                        var ff = GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = GetFaceColors(ff);
                        if (GetFaceTiles(ff, RubikCubeFaces.CubeFace.Left).Count > 0)
                        {
                            left = true;
                        }
                       
                        var flf = GetFaceTiles(ff, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var flfc = GetFaceColors(flf);

                        var bf = GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = GetFaceColors(bf);
                        var blf = GetFaceTiles(bf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var blfc = GetFaceColors(blf);

                        var uf = GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = GetFaceColors(uf);
                        var ulf = GetFaceTiles(uf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var ulfc = GetFaceColors(ulf);

                        var df = GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = GetFaceColors(df);
                        var dlf = GetFaceTiles(df, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var dlfc = GetFaceColors(dlf);

                        SetFaceColors(ff, ufc);
                        SetFaceColors(uf, bfc);
                        SetFaceColors(bf, dfc);
                        SetFaceColors(df, ffc);

                        SetFaceColors(flf, ulfc);
                        SetFaceColors(ulf, blfc);
                        SetFaceColors(blf, dlfc);
                        SetFaceColors(dlf, flfc);
                    }
                    else
                    {
                        bool left = false;

                        var ff = GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = GetFaceColors(ff);
                        if (GetFaceTiles(ff, RubikCubeFaces.CubeFace.Left).Count > 0)
                        {
                            left = true;
                        }

                        var flf = GetFaceTiles(ff, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var flfc = GetFaceColors(flf);

                        var bf = GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = GetFaceColors(bf);
                        var blf = GetFaceTiles(bf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var blfc = GetFaceColors(blf);

                        var uf = GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = GetFaceColors(uf);
                        var ulf = GetFaceTiles(uf, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var ulfc = GetFaceColors(ulf);

                        var df = GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = GetFaceColors(df);
                        var dlf = GetFaceTiles(df, left ? RubikCubeFaces.CubeFace.Left : RubikCubeFaces.CubeFace.Right);
                        var dlfc = GetFaceColors(dlf);

                        SetFaceColors(ff, dfc);
                        SetFaceColors(df, bfc);
                        SetFaceColors(bf, ufc);
                        SetFaceColors(uf, ffc);

                        SetFaceColors(flf, dlfc);
                        SetFaceColors(dlf, blfc);
                        SetFaceColors(blf, ulfc);
                        SetFaceColors(ulf, flfc);
                    }
                }
                break;
            case AXIS.Y:
                {
                    if (-direction > 0)
                    {
                        bool up = false;

                        var ff = GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = GetFaceColors(ff);
                        if (GetFaceTiles(ff, RubikCubeFaces.CubeFace.Up).Count > 0)
                        {
                            up = true;
                        }

                        var fuf = GetFaceTiles(ff, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var fufc = GetFaceColors(fuf);

                        var bf = GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = GetFaceColors(bf);
                        var buf = GetFaceTiles(bf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var bufc = GetFaceColors(buf);

                        var rf = GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = GetFaceColors(rf);
                        var ruf = GetFaceTiles(rf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var rufc = GetFaceColors(ruf);

                        var lf = GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = GetFaceColors(lf);
                        var luf = GetFaceTiles(lf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var lufc = GetFaceColors(luf);

                        SetFaceColors(ff, lfc);
                        SetFaceColors(lf, bfc);
                        SetFaceColors(bf, rfc);
                        SetFaceColors(rf, ffc);

                        SetFaceColors(fuf, lufc);
                        SetFaceColors(luf, bufc);
                        SetFaceColors(buf, rufc);
                        SetFaceColors(ruf, fufc);
                    }
                    else
                    {
                        bool up = false;

                        var ff = GetFaceTiles(RubikCubeFaces.CubeFace.Front, axis, -direction);
                        var ffc = GetFaceColors(ff);
                        if (GetFaceTiles(ff, RubikCubeFaces.CubeFace.Up).Count > 0)
                        {
                            up = true;
                        }

                        var fuf = GetFaceTiles(ff, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var fufc = GetFaceColors(fuf);

                        var bf = GetFaceTiles(RubikCubeFaces.CubeFace.Back, axis, -direction);
                        var bfc = GetFaceColors(bf);
                        var buf = GetFaceTiles(bf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var bufc = GetFaceColors(buf);

                        var rf = GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = GetFaceColors(rf);
                        var ruf = GetFaceTiles(rf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var rufc = GetFaceColors(ruf);

                        var lf = GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = GetFaceColors(lf);
                        var luf = GetFaceTiles(lf, up ? RubikCubeFaces.CubeFace.Up : RubikCubeFaces.CubeFace.Down);
                        var lufc = GetFaceColors(luf);

                        SetFaceColors(ff, rfc);
                        SetFaceColors(rf, bfc);
                        SetFaceColors(bf, lfc);
                        SetFaceColors(lf, ffc);

                        SetFaceColors(fuf, rufc);
                        SetFaceColors(ruf, bufc);
                        SetFaceColors(buf, lufc);
                        SetFaceColors(luf, fufc);
                    }
                }
                break;
            case AXIS.Z:
                {
                    if (-direction > 0)
                    {
                        bool front = false;

                        var uf = GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = GetFaceColors(uf);
                        if (GetFaceTiles(uf, RubikCubeFaces.CubeFace.Front).Count > 0)
                        {
                            front = true;
                        }

                        var uff = GetFaceTiles(uf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var uffc = GetFaceColors(uff);

                        var df = GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = GetFaceColors(df);
                        var dff = GetFaceTiles(df, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var dffc = GetFaceColors(dff);

                        var rf = GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = GetFaceColors(rf);
                        var rff = GetFaceTiles(rf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var rffc = GetFaceColors(rff);

                        var lf = GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = GetFaceColors(lf);
                        var lff = GetFaceTiles(lf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var lffc = GetFaceColors(lff);

                        SetFaceColors(uf, lfc);
                        SetFaceColors(lf, dfc);
                        SetFaceColors(df, rfc);
                        SetFaceColors(rf, ufc);

                        SetFaceColors(uff, lffc);
                        SetFaceColors(lff, dffc);
                        SetFaceColors(dff, rffc);
                        SetFaceColors(rff, uffc);
                    }
                    else
                    {
                        bool front = false;

                        var uf = GetFaceTiles(RubikCubeFaces.CubeFace.Up, axis, -direction);
                        var ufc = GetFaceColors(uf);
                        if (GetFaceTiles(uf, RubikCubeFaces.CubeFace.Front).Count > 0)
                        {
                            front = true;
                        }

                        var uff = GetFaceTiles(uf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var uffc = GetFaceColors(uff);

                        var df = GetFaceTiles(RubikCubeFaces.CubeFace.Down, axis, -direction);
                        var dfc = GetFaceColors(df);
                        var dff = GetFaceTiles(df, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var dffc = GetFaceColors(dff);

                        var rf = GetFaceTiles(RubikCubeFaces.CubeFace.Right, axis, -direction);
                        var rfc = GetFaceColors(rf);
                        var rff = GetFaceTiles(rf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var rffc = GetFaceColors(rff);

                        var lf = GetFaceTiles(RubikCubeFaces.CubeFace.Left, axis, -direction);
                        var lfc = GetFaceColors(lf);
                        var lff = GetFaceTiles(lf, front ? RubikCubeFaces.CubeFace.Front : RubikCubeFaces.CubeFace.Back);
                        var lffc = GetFaceColors(lff);

                        SetFaceColors(uf, rfc);
                        SetFaceColors(rf, dfc);
                        SetFaceColors(df, lfc);
                        SetFaceColors(lf, ufc);

                        SetFaceColors(uff, rffc);
                        SetFaceColors(rff, dffc);
                        SetFaceColors(dff, lffc);
                        SetFaceColors(lff, uffc);
                    }
                }
                break;
            case AXIS.NONE:
                break;
            default:
                break;
        }

        //int index = 0;
        //for (int j = 0; j < slice.transform.childCount; j++)
        //{
        //    var cube = slice.transform.GetChild(j);
        //    List<Quaternion> tr = new List<Quaternion>();
        //    for (int i = index; i < index + cube.childCount; i++)
        //    {
        //        tr.Add(tileRotations[i]);
        //    }

        //    index += cube.childCount;
        //    RestoreRotation(cube.gameObject, tr);
        //}
    }

    public List<GameObject> GetFaceTiles(RubikCubeFaces.CubeFace face, AXIS axis, float dir)
    {
        List<GameObject> res = new List<GameObject>();

        switch (face)
        {
            case RubikCubeFaces.CubeFace.Front:
                {
                    var fronts = slice.GetComponentsInChildren<MarkAsFrontFace>();

                    for (int i = 0; i < fronts.Length; i++)
                    {
                        if (fronts[i])
                        {
                            res.Add(fronts[i].gameObject);
                        }
                    }
                    
                }
                break;
            case RubikCubeFaces.CubeFace.Back:
                {
                    var backs = slice.GetComponentsInChildren<MarkAsBackFace>();

                    for (int i = 0; i < backs.Length; i++)
                    {
                        if (backs[i])
                        {
                            res.Add(backs[i].gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Right:
                {
                    var rights = slice.GetComponentsInChildren<MarkAsRightFace>();

                    for (int i = 0; i < rights.Length; i++)
                    {
                        if (rights[i])
                        {
                            res.Add(rights[i].gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Left:
                {
                    var lefts = slice.GetComponentsInChildren<MarkAsLeftFace>();

                    for (int i = 0; i < lefts.Length; i++)
                    {
                        if (lefts[i])
                        {
                            res.Add(lefts[i].gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Up:
                {
                    var ups = slice.GetComponentsInChildren<MarkAsUpFace>();

                    for (int i = 0; i < ups.Length; i++)
                    {
                        if (ups[i])
                        {
                            res.Add(ups[i].gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Down:
                {
                    var downs = slice.GetComponentsInChildren<MarkAsDownFace>();

                    for (int i = 0; i < downs.Length; i++)
                    {
                        if (downs[i])
                        {
                            res.Add(downs[i].gameObject);
                        }
                    }
                }
                break;
            default:
                break;
        }

        switch (face)
        {
            case RubikCubeFaces.CubeFace.Front:
                {
                    for (int i = 0; i < res.Count - 1; i++)
                    {
                        for (int j = 0; j < res.Count - i - 1; j++)
                        {
                            switch (axis)
                            {
                                case AXIS.X:
                                    {
                                        if (dir < 0)
                                        {
                                            if (res[j].transform.position.y > res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.y < res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                case AXIS.Y:
                                    {
                                        if (dir > 0)
                                        {
                                            if (res[j].transform.position.x > res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.x < res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }

                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Back:
                {
                    for (int i = 0; i < res.Count - 1; i++)
                    {
                        for (int j = 0; j < res.Count - i - 1; j++)
                        {
                            switch (axis)
                            {
                                case AXIS.X:
                                    {
                                        if (dir > 0)
                                        {
                                            if (res[j].transform.position.y > res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.y < res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                case AXIS.Y:
                                    {
                                        if (dir < 0)
                                        {
                                            if (res[j].transform.position.x > res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.x < res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                                       
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Right:
                {
                    for (int i = 0; i < res.Count - 1; i++)
                    {
                        for (int j = 0; j < res.Count - i - 1; j++)
                        {
                            switch (axis)
                            {
                                case AXIS.Y:
                                    {
                                        if (dir > 0)
                                        {
                                            if (res[j].transform.position.z > res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.z < res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                case AXIS.Z:
                                    {
                                        if (dir < 0)
                                        {
                                            if (res[j].transform.position.y > res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.y < res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Left:
                {
                    for (int i = 0; i < res.Count - 1; i++)
                    {
                        for (int j = 0; j < res.Count - i - 1; j++)
                        {
                            switch (axis)
                            {
                                case AXIS.Y:
                                    {
                                        if (dir < 0)
                                        {
                                            if (res[j].transform.position.z > res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.z < res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                case AXIS.Z:
                                    {
                                        if (dir > 0)
                                        {
                                            if (res[j].transform.position.y > res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.y < res[j + 1].transform.position.y)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }                                   
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Up:
                {
                    for (int i = 0; i < res.Count - 1; i++)
                    {
                        for (int j = 0; j < res.Count - i - 1; j++)
                        {
                            switch (axis)
                            {
                                case AXIS.X:
                                    {
                                        if (dir < 0)
                                        {
                                            if (res[j].transform.position.z > res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.z < res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                case AXIS.Z:
                                    {
                                        if (dir > 0)
                                        {
                                            if (res[j].transform.position.x > res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.x < res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Down:
                {
                    for (int i = 0; i < res.Count - 1; i++)
                    {
                        for (int j = 0; j < res.Count - i - 1; j++)
                        {
                            switch (axis)
                            {
                                case AXIS.X:
                                    {
                                        if (dir > 0)
                                        {
                                            if (res[j].transform.position.z > res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.z < res[j + 1].transform.position.z)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;                               
                                case AXIS.Z:
                                    {
                                        if (dir < 0)
                                        {
                                            if (res[j].transform.position.x > res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                        else
                                        {
                                            if (res[j].transform.position.x < res[j + 1].transform.position.x)
                                            {
                                                var tmp = res[j];
                                                res[j] = res[j + 1];
                                                res[j + 1] = tmp;
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }                                       
                        }
                    }
                }
                break;
            default:
                break;
        }

        return res;
    }

    public List<GameObject> GetFaceTiles(List<GameObject> face, RubikCubeFaces.CubeFace wantedFace)
    {
        List<GameObject> res = new List<GameObject>();

        switch (wantedFace)
        {
            case RubikCubeFaces.CubeFace.Front:
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var marker = face[i].transform.parent.parent.GetComponentInChildren<MarkAsFrontFace>();
                        if (marker)
                        {
                            res.Add(face[i].transform.parent.parent.GetComponentInChildren<MarkAsFrontFace>().gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Back:
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var marker = face[i].transform.parent.parent.GetComponentInChildren<MarkAsBackFace>();
                        if (marker)
                        {
                            res.Add(face[i].transform.parent.parent.GetComponentInChildren<MarkAsBackFace>().gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Right:
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var marker = face[i].transform.parent.parent.GetComponentInChildren<MarkAsRightFace>();
                        if (marker)
                        {
                            res.Add(face[i].transform.parent.parent.GetComponentInChildren<MarkAsRightFace>().gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Left:
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var marker = face[i].transform.parent.parent.GetComponentInChildren<MarkAsLeftFace>();
                        if (marker)
                        {
                            res.Add(face[i].transform.parent.parent.GetComponentInChildren<MarkAsLeftFace>().gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Up:
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var marker = face[i].transform.parent.parent.GetComponentInChildren<MarkAsUpFace>();
                        if (marker)
                        {
                            res.Add(face[i].transform.parent.parent.GetComponentInChildren<MarkAsUpFace>().gameObject);
                        }
                    }
                }
                break;
            case RubikCubeFaces.CubeFace.Down:
                {
                    for (int i = 0; i < face.Count; i++)
                    {
                        var marker = face[i].transform.parent.parent.GetComponentInChildren<MarkAsDownFace>();
                        if (marker)
                        {
                            res.Add(face[i].transform.parent.parent.GetComponentInChildren<MarkAsDownFace>().gameObject);
                        }
                    }
                }
                break;
            default:
                break;
        }

        return res;
    }

    public List<Color> GetFaceColors(List<GameObject> face)
    {
        List<Color> res = new List<Color>();

        for (int i = 0; i < face.Count; i++)
        {
            res.Add(face[i].GetComponent<Renderer>().material.color);
        }

        return res;
    }

    public void SetFaceColors(List<GameObject> face, List<Color> colors)
    {
        if (face.Count == colors.Count)
        {
            for (int i = 0; i < face.Count; i++)
            {
                face[i].GetComponent<Renderer>().material.color = colors[i];
            }
        }
    }
    
    public GameObject GetSlice()
    {
        return slice;
    }

    public void EmptySlice()
    {
        if (slice)
        {
            for (int i = 0; i < slice.transform.childCount; i++)
            {
                var child = slice.transform.GetChild(i);
                child.SetParent(cubeRoot);
                i--;
            }
        }
    }

    public void SetSelectedCube(int x, int y, int z)
    {
        selecedCube = cubes[x][y][z];
    }
}
