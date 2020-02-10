using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikCubeManager : MonoBehaviour
{
    public static RubikCubeManager Instance;

    [KAI.KAIEvent]
    public KAI.CustomEvent WinEvent;

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

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckCubeCondition(1));
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.globalGameState.GetCurrentGameState() == GameState.InGame)
        {
            if (GameManager.Instance.playerData.GetSecondsElapsed() > 5)
            {
                if (solved)
                {
                    StopAllCoroutines();

                    if (WinEvent)
                    {
                        WinEvent.Raise();
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// The most important part of the algorithm, according to the slice that is rotating, the axis i'm rotating around and the rotation 
    /// direction:
    /// 1- determine the face of the cube
    /// 2- get the tiles on that face and add them to a list
    /// 3- sort that list by the x, y or z position in ascending or descending order, according to the rotation axis and rotation direction
    /// TODO: Replace the bubble sort with a faster sorting algorithm
    /// </summary>
    /// <param name="face"></param>
    /// <param name="axis"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public List<GameObject> GetFaceTiles(RubikCubeFaces.CubeFace face, AXIS axis, float dir)
    {
        List<GameObject> res = new List<GameObject>();

        switch (face)
        {
            case RubikCubeFaces.CubeFace.Front:
                {
                    var fronts = RubikGenerator.Instance.GetSlice().GetComponentsInChildren<MarkAsFrontFace>();

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
                    var backs = RubikGenerator.Instance.GetSlice().GetComponentsInChildren<MarkAsBackFace>();

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
                    var rights = RubikGenerator.Instance.GetSlice().GetComponentsInChildren<MarkAsRightFace>();

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
                    var lefts = RubikGenerator.Instance.GetSlice().GetComponentsInChildren<MarkAsLeftFace>();

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
                    var ups = RubikGenerator.Instance.GetSlice().GetComponentsInChildren<MarkAsUpFace>();

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
                    var downs = RubikGenerator.Instance.GetSlice().GetComponentsInChildren<MarkAsDownFace>();

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

        // Bubble sorting tiles
        // A really crappy way of doing it 
        // TODO: Optimize this part
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

    public void RotateSide(List<GameObject> side, float dir)
    {
        int size = RubikGenerator.Instance.size;

        List<List<Color>> face = new List<List<Color>>();

        for (int i = 0; i < size; i++)
        {
            face.Add(new List<Color>());
            for (int j = 0; j < size; j++)
            {
                face[i].Add(Color.clear);
            }
        }
        int index = 0;
        for (int j = 0; j < size; j++)
        {
            for (int k = 0; k < size; k++)
            {
                face[j][k] = side[index].GetComponent<Renderer>().material.color;
                index++;
            }
        }

        var rot = Rotation90(face, dir);

        index = 0;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                side[index].GetComponent<Renderer>().material.color = rot[i][j];
                index++;
            }
        }
    }    

    private List<List<Color>> Rotation90(List<List<Color>> arr, float clockwise)
    {
        int size = RubikGenerator.Instance.size;
        List<List<Color>> arr_rotated = new List<List<Color>>();

        for (int i = 0; i < size; i++)
        {
            arr_rotated.Add(new List<Color>());
            for (int j = 0; j < size; j++)
            {
                arr_rotated[i].Add(Color.clear);
            }
        }

        if (clockwise < 0)
        {
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < arr[i].Count; j++)
                {
                    arr_rotated[size - 1 - j][i] = arr[i][j];
                }
            }
        }
        else
        {
            for (var i = 0; i < size; i++)
            {
                for (var j = 0; j < size; j++)
                {
                    arr_rotated[j][size - 1 - i] = arr[i][j];
                }
            }
        }
        return arr_rotated;
    }

    /// <summary>
    /// Given a certain slice face get the desired face tiles (i.e: get the up tiles of the front face)
    /// </summary>
    /// <param name="face"></param>
    /// <param name="wantedFace"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Get a list of face colors 
    /// </summary>
    /// <param name="face"></param>
    /// <returns></returns>
    public List<Color> GetFaceColors(List<GameObject> face)
    {
        List<Color> res = new List<Color>();

        for (int i = 0; i < face.Count; i++)
        {
            res.Add(face[i].GetComponent<Renderer>().material.color);
        }

        return res;
    }

    /// <summary>
    /// Overwrite the face's colors with another list of colors
    /// </summary>
    /// <param name="face"></param>
    /// <param name="colors"></param>
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

    private IEnumerator CheckCubeCondition(float delay)
    {
        while (!solved)
        {
            yield return new WaitForSeconds(delay);

            if (GameManager.Instance.globalGameState.GetCurrentGameState() == GameState.InGame)
            {
                solved = IsSolved();
            }
        }
    }

    /// <summary>
    /// Check each face and see if all of its colors match
    /// </summary>
    /// <returns></returns>
    public bool IsSolved()
    {
        var frontFaceCache = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsFrontFace>();
        var backFaceCache = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsBackFace>();
        var upFaceCache = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsUpFace>();
        var downFaceCache = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsDownFace>();
        var rightFaceCache = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsRightFace>();
        var leftFaceCache = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsLeftFace>();

        var fc = frontFaceCache[0].GetComponent<Renderer>().material.color;
        bool frontSolved = true;

        var bc = backFaceCache[0].GetComponent<Renderer>().material.color;
        bool backSolved = true;

        var uc = upFaceCache[0].GetComponent<Renderer>().material.color;
        bool upSolved = true;

        var dc = downFaceCache[0].GetComponent<Renderer>().material.color;
        bool downSolved = true;

        var rc = rightFaceCache[0].GetComponent<Renderer>().material.color;
        bool rightSolved = true;

        var lc = leftFaceCache[0].GetComponent<Renderer>().material.color;
        bool leftSolved = true;

        for (int i = 1; i < frontFaceCache.Length; i++)
        {
            if (!frontFaceCache[i].GetComponent<Renderer>().material.color.Equals(fc))
            {
                frontSolved = false;
            }
        }

        if (!frontSolved)
        {
            return false;
        }

        for (int i = 1; i < backFaceCache.Length; i++)
        {
            if (!backFaceCache[i].GetComponent<Renderer>().material.color.Equals(bc))
            {
                backSolved = false;
            }
        }

        if (!backSolved)
        {
            return false;
        }

        for (int i = 1; i < upFaceCache.Length; i++)
        {
            if (!upFaceCache[i].GetComponent<Renderer>().material.color.Equals(uc))
            {
                upSolved = false;
            }
        }

        if (!upSolved)
        {
            return false;
        }

        for (int i = 1; i < downFaceCache.Length; i++)
        {
            if (!downFaceCache[i].GetComponent<Renderer>().material.color.Equals(dc))
            {
                downSolved = false;
            }
        }

        if (!downSolved)
        {
            return false;
        }

        for (int i = 1; i < rightFaceCache.Length; i++)
        {
            if (!rightFaceCache[i].GetComponent<Renderer>().material.color.Equals(rc))
            {
                rightSolved = false;
            }
        }

        if (!rightSolved)
        {
            return false;
        }

        for (int i = 1; i < leftFaceCache.Length; i++)
        {
            if (!leftFaceCache[i].GetComponent<Renderer>().material.color.Equals(lc))
            {
                leftSolved = false;
            }
        }

        if (!leftSolved)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Record the current cube tile colors in player data
    /// </summary>
    /// <param name="playerData"></param>
    public void RecordColors(PlayerData playerData)
    {
        if (playerData)
        {
            if (playerData.currentColors.frontFaceColors.Count == 0 || playerData.currentColors.frontFaceColors.Count > RubikGenerator.Instance.size * RubikGenerator.Instance.size)
            {
                playerData.currentColors.frontFaceColors = new List<Color>();
                playerData.currentColors.backFaceColors = new List<Color>();
                playerData.currentColors.upFaceColors = new List<Color>();
                playerData.currentColors.downFaceColors = new List<Color>();
                playerData.currentColors.rightFaceColors = new List<Color>();
                playerData.currentColors.leftFaceColors = new List<Color>();

                playerData.currentColors.frontPositions  = new List<Vector3>();
                playerData.currentColors.backPositions  = new List<Vector3>();
                playerData.currentColors.upPositions    = new List<Vector3>();
                playerData.currentColors.downPositions  = new List<Vector3>();
                playerData.currentColors.rightPositions = new List<Vector3>();
                playerData.currentColors.leftPositions  = new List<Vector3>();

                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    playerData.currentColors.frontFaceColors.Add(Color.clear);
                    playerData.currentColors.backFaceColors.Add(Color.clear);
                    playerData.currentColors.upFaceColors.Add(Color.clear);
                    playerData.currentColors.downFaceColors.Add(Color.clear);
                    playerData.currentColors.rightFaceColors.Add(Color.clear);
                    playerData.currentColors.leftFaceColors.Add(Color.clear);

                    playerData.currentColors.frontPositions.Add(Vector3.zero);
                    playerData.currentColors.backPositions.Add(Vector3.zero);
                    playerData.currentColors.upPositions.Add(Vector3.zero);
                    playerData.currentColors.downPositions.Add(Vector3.zero);
                    playerData.currentColors.rightPositions.Add(Vector3.zero);
                    playerData.currentColors.leftPositions.Add(Vector3.zero);
                }
            }

            var front = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsFrontFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                playerData.currentColors.frontFaceColors[j] = front[j].GetComponent<Renderer>().material.color;
                playerData.currentColors.frontPositions[j] = front[j].transform.position;
            }

            var back = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsBackFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                playerData.currentColors.backFaceColors[j] = back[j].GetComponent<Renderer>().material.color;
                playerData.currentColors.backPositions[j] = back[j].transform.position;
            }

            var up = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsUpFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                playerData.currentColors.upFaceColors[j] = up[j].GetComponent<Renderer>().material.color;
                playerData.currentColors.upPositions[j] = up[j].transform.position;
            }

            var down = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsDownFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                playerData.currentColors.downFaceColors[j] = down[j].GetComponent<Renderer>().material.color;
                playerData.currentColors.downPositions[j] = down[j].transform.position;
            }

            var right = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsRightFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                playerData.currentColors.rightFaceColors[j] = right[j].GetComponent<Renderer>().material.color;
                playerData.currentColors.rightPositions[j] = right[j].transform.position;
            }

            var left = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsLeftFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                playerData.currentColors.leftFaceColors[j] = left[j].GetComponent<Renderer>().material.color;
                playerData.currentColors.leftPositions[j] = left[j].transform.position;
            }
        }
    }

    /// <summary>
    /// Overwrite all tile colors with the colors saved in player data
    /// </summary>
    /// <param name="playerData"></param>
    public void SetColors(PlayerData playerData)
    {
        if (playerData)
        {
            if (playerData.currentColors.frontFaceColors.Count == 0 || playerData.currentColors.frontFaceColors.Count > RubikGenerator.Instance.size * RubikGenerator.Instance.size)
            {
                //RecordColors(playerData);

                return;
            }

            var front = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsFrontFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    if ((front[j].transform.position - playerData.currentColors.frontPositions[i]).magnitude < 0.01f)
                    {
                        front[j].GetComponent<Renderer>().material.color = playerData.currentColors.frontFaceColors[i];
                        break;
                    }
                }
            }

            var back = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsBackFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    if ((back[j].transform.position - playerData.currentColors.backPositions[i]).magnitude < 0.01f)
                    {
                        back[j].GetComponent<Renderer>().material.color = playerData.currentColors.backFaceColors[i];
                        break;
                    }
                }
            }

            var up = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsUpFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    if ((up[j].transform.position - playerData.currentColors.upPositions[i]).magnitude < 0.01f)
                    {
                        up[j].GetComponent<Renderer>().material.color = playerData.currentColors.upFaceColors[i];
                        break;
                    }
                }
            }

            var down = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsDownFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    if ((down[j].transform.position - playerData.currentColors.downPositions[i]).magnitude < 0.01f)
                    {
                        down[j].GetComponent<Renderer>().material.color = playerData.currentColors.downFaceColors[i];
                        break;
                    }
                }
            }

            var right = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsRightFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    if ((right[j].transform.position - playerData.currentColors.rightPositions[i]).magnitude < 0.01f)
                    {
                        right[j].GetComponent<Renderer>().material.color = playerData.currentColors.rightFaceColors[i];
                        break;
                    }
                }
            }

            var left = RubikGenerator.Instance.cubeRoot.GetComponentsInChildren<MarkAsLeftFace>();

            for (int j = 0; j < RubikGenerator.Instance.size * RubikGenerator.Instance.size; j++)
            {
                for (int i = 0; i < RubikGenerator.Instance.size * RubikGenerator.Instance.size; i++)
                {
                    if ((left[j].transform.position - playerData.currentColors.leftPositions[i]).magnitude < 0.01f)
                    {
                        left[j].GetComponent<Renderer>().material.color = playerData.currentColors.leftFaceColors[i];
                        break;
                    }
                }
            }
        }
    }
}
