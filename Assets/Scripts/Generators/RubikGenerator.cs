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

    private GameObject slice;

    [HideInInspector]
    public GameObject selecedCube;

    [HideInInspector]
    public List<List<List<Vector3>>> cubesPositions = new List<List<List<Vector3>>>();
    private List<List<List<GameObject>>> cubes = new List<List<List<GameObject>>>();
    private List<GameObject> tiles = new List<GameObject>();   

    private float currentAngle;

    private bool solved;

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

    private void Start()
    {
    }

    private void Update()
    {
       
    }

    public void GenerateCube()
    {
        if (cubeRoot.transform.childCount > 0)
        {
            DestroyCube();
        }

        for (int i = 0; i < size; i++)
        {
            cubes.Add(new List<List<GameObject>>());
            cubesPositions.Add(new List<List<Vector3>>());
            for (int j = 0; j < size; j++)
            {
                cubes[i].Add(new List<GameObject>());
                cubesPositions[i].Add(new List<Vector3>());
                for (int k = 0; k < size; k++)
                {
                    cubes[i][j].Add(null);
                    cubesPositions[i][j].Add(Vector3.zero);
                }
            }
        }

        if (GameManager.Instance.playerData)
        {
            GameManager.Instance.playerData.cubeSize = size;
        }

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
                    GameObject cube = Instantiate<GameObject>(cubePrefab, Vector3.zero, Quaternion.identity);
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

        GenerateTiles();      
    }

    public void GenerateCube(int size)
    {
        this.size = size;

        GenerateCube();
    }

    private void GenerateTiles()
    {
        float scale = 1 - tilePadding;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                var tile = Instantiate<GameObject>(tilePrefab, Vector3.zero, Quaternion.identity);
                tile.transform.SetParent(cubes[i][j][0].transform);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.name = cubes[i][j][0].name + "+Z";
                var tileBody = tile.transform.GetChild(0);
                tileBody.transform.localScale = new Vector3(scale, scale, 0.1f);
                tileBody.transform.localPosition = new Vector3(0, 0, 0.5f);
                tileBody.transform.name = cubes[i][j][0].name;
                Material material = new Material(tileBody.GetComponent<Renderer>().material);
                material.color = cubePreset.BackFace.Color;
                tileBody.GetComponent<Renderer>().material = material;
                tileBody.gameObject.AddComponent<MarkAsBackFace>();

                var tile1 = Instantiate<GameObject>(tilePrefab, Vector3.zero, Quaternion.identity);
                tile1.transform.SetParent(cubes[i][j][size - 1].transform);
                tile1.transform.localPosition = Vector3.zero;
                tile1.transform.name = cubes[i][j][size - 1].name + "-Z";

                var tileBody1 = tile1.transform.GetChild(0);
                tileBody1.transform.localScale = new Vector3(scale, scale, 0.1f);
                tileBody1.transform.localPosition = new Vector3(0, 0, -0.5f);
                tileBody1.transform.name = cubes[i][j][size - 1].name;
                material = new Material(tileBody1.GetComponent<Renderer>().material);
                material.color = cubePreset.FrontFace.Color;
                tileBody1.GetComponent<Renderer>().material = material;
                tileBody1.gameObject.AddComponent<MarkAsFrontFace>();
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject tile = Instantiate<GameObject>(tilePrefab, Vector3.zero, Quaternion.identity);
                tile.transform.SetParent(cubes[0][i][j].transform);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.name = cubes[0][i][j].name + "-X";
                Transform tileBody = tile.transform.GetChild(0);
                tileBody.transform.localScale = new Vector3(0.1f, scale, scale);
                tileBody.transform.localPosition = new Vector3(-0.5f, 0, 0);
                tileBody.transform.name = cubes[0][i][j].name;
                Material material = new Material(tileBody.GetComponent<Renderer>().material);
                material.color = cubePreset.LeftFace.Color;
                tileBody.GetComponent<Renderer>().material = material;
                tileBody.gameObject.AddComponent<MarkAsLeftFace>();

                GameObject tile1 = Instantiate<GameObject>(tilePrefab, Vector3.zero, Quaternion.identity);
                tile1.transform.SetParent(cubes[size - 1][i][j].transform);
                tile1.transform.localPosition = Vector3.zero;
                tile1.transform.name = cubes[size - 1][i][j].name + "+X";

                Transform tileBody1 = tile1.transform.GetChild(0);
                tileBody1.transform.localScale = new Vector3(0.1f, scale, scale);
                tileBody1.transform.localPosition = new Vector3(0.5f, 0, 0);
                tileBody1.transform.name = cubes[size - 1][i][j].name;
                material = new Material(tileBody1.GetComponent<Renderer>().material);
                material.color = cubePreset.RightFace.Color;
                tileBody1.GetComponent<Renderer>().material = material;
                tileBody1.gameObject.AddComponent<MarkAsRightFace>();
            }
        }

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                GameObject tile = Instantiate<GameObject>(tilePrefab, Vector3.zero, Quaternion.identity);
                tile.transform.SetParent(cubes[i][0][j].transform);
                tile.transform.localPosition = Vector3.zero;
                tile.transform.name = cubes[i][0][j].name + "-Y";
                Transform tileBody = tile.transform.GetChild(0);
                tileBody.transform.localScale = new Vector3(scale, 0.1f, scale);
                tileBody.transform.localPosition = new Vector3(0, -0.5f, 0);
                tileBody.transform.name = cubes[i][0][j].name;
                Material material = new Material(tileBody.GetComponent<Renderer>().material);
                material.color = cubePreset.UpFace.Color;
                tileBody.GetComponent<Renderer>().material = material;
                tileBody.gameObject.AddComponent<MarkAsDownFace>();

                GameObject tile1 = Instantiate<GameObject>(tilePrefab, Vector3.zero, Quaternion.identity);
                tile1.transform.SetParent(cubes[i][size - 1][j].transform);
                tile1.transform.localPosition = Vector3.zero;
                tile1.transform.name = cubes[i][size - 1][j].name + "+Y";

                Transform tileBody1 = tile1.transform.GetChild(0);
                tileBody1.transform.localScale = new Vector3(scale, 0.1f, scale);
                tileBody1.transform.localPosition = new Vector3(0, 0.5f, 0);
                tileBody1.transform.name = cubes[i][size - 1][j].name;
                material = new Material(tileBody1.GetComponent<Renderer>().material);
                material.color = cubePreset.DownFace.Color;
                tileBody1.GetComponent<Renderer>().material = material;
                tileBody1.gameObject.AddComponent<MarkAsUpFace>();
            }
        }
    }

    private void DestroyCube()
    {
        for (int i = 0; i < cubeRoot.childCount; i++)
        {
            DestroyImmediate(cubeRoot.GetChild(i).gameObject);
            i--;
        }

        DestroyImmediate(slice);
    }

    /// <summary>
    /// If there is a selected small cube then given the axis of rotation group the appropriate cubes into one slice
    /// </summary>
    /// <param name="axis"></param>
    /// <returns></returns>
    public GameObject GetSlice(AXIS axis)
    {
        int x = int.Parse(selecedCube.name[0].ToString()) - 1;
        int y = int.Parse(selecedCube.name[1].ToString()) - 1;
        int z = int.Parse(selecedCube.name[2].ToString()) - 1;

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
