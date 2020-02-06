using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikPCInput : MonoBehaviour
{   
    private RubikController rubikController;

    private RaycastHit hit;

    private GameObject firstSelectedCube;
    private Vector3 firstHitPosition;

    private GameObject lastSelectedCube;
    private Vector3 lastHitPosition;

    private GameObject cube;

    private bool dragging = false;
    private bool lockDragging = false;

    private Vector3 old;

    private Quaternion oldCubeR;
    private Vector3 oldCubeP;
    private Quaternion oldCamR;
    private Vector3 oldCamP;
    private bool firstOrbit = true;
    private bool cubeRotationMode = false;
    private bool cameraOrbitMode = false;

    // Start is called before the first frame update
    void Start()
    {
        rubikController = GetComponent<RubikController>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (!rubikController.scrambling)
        {
            float x = 0;
            float y = 0;

            if (Input.GetMouseButtonDown(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (cameraOrbitMode)
                    {
                        Camera.main.transform.SetParent(transform);
                        transform.rotation = Quaternion.identity;
                        Camera.main.transform.SetParent(null);
                    }

                    cube = hit.collider.gameObject;

                    if (!dragging)
                    {
                        firstSelectedCube = cube;
                        firstHitPosition = hit.point;
                        dragging = true;
                    }

                    lastHitPosition = hit.point;
                    lastSelectedCube = cube;

                    cubeRotationMode = true;
                    cameraOrbitMode = false;
                }
                else
                {
                    cameraOrbitMode = true;
                    cubeRotationMode = false;

                    cube = null;
                    dragging = false;
                    lockDragging = false;

                    firstSelectedCube = null;
                    lastSelectedCube = null;
                }
            }

            if (cubeRotationMode)
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                x = Input.GetAxis("Mouse X");
                y = Input.GetAxis("Mouse Y");

                if (Physics.Raycast(ray, out hit))
                {
                    cube = hit.collider.gameObject;

                    if (!dragging)
                    {
                        firstSelectedCube = cube;
                        firstHitPosition = hit.point;
                        dragging = true;
                    }

                    lastHitPosition = hit.point;
                    lastSelectedCube = cube;
                }
            }

            if (cameraOrbitMode)
            {
                if (!firstOrbit)
                {
                    Camera.main.transform.rotation = oldCamR;
                    Camera.main.transform.position = oldCamP;
                    transform.rotation = oldCubeR;
                    transform.position = oldCubeP;
                }

                Vector2 d = new Vector2(Input.GetAxis("Mouse X") * 150, Input.GetAxis("Mouse Y") * 150);
                d.y = ClampAngle(d.y, -360, 360);
                Camera.main.transform.RotateAround(RubikGenerator.Instance.transform.position, Vector3.up, d.x * Time.deltaTime);
                transform.RotateAround(RubikGenerator.Instance.transform.position, Camera.main.transform.right, d.y * Time.deltaTime);
                Camera.main.transform.LookAt(RubikGenerator.Instance.transform, Camera.main.transform.up);
                old = (Input.mousePosition);

                oldCamR = Camera.main.transform.rotation;
                oldCamP = Camera.main.transform.position;
                oldCubeR = transform.rotation;
                oldCubeP = transform.position;
                firstOrbit = false;
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (cubeRotationMode)
                {
                    if (firstSelectedCube && !firstHitPosition.Equals(lastHitPosition) && !lockDragging)
                    {
                        lockDragging = true;

                        float minScale1 = Mathf.Min(
                            firstSelectedCube.transform.localScale.x, Mathf.Min(
                                firstSelectedCube.transform.localScale.y,
                                firstSelectedCube.transform.localScale.z));

                        bool xy1 = false, xz1 = false, yz1 = false;

                        AXIS axis = AXIS.NONE;

                        if (minScale1 == firstSelectedCube.transform.localScale.x)
                        {
                            yz1 = true;
                        }
                        else if (minScale1 == firstSelectedCube.transform.localScale.y)
                        {
                            xz1 = true;
                        }
                        else if (minScale1 == firstSelectedCube.transform.localScale.z)
                        {
                            xy1 = true;
                        }

                        float direction = 1;

                        var dir = (lastHitPosition - firstHitPosition).normalized;

                        if (xy1)
                        {
                            float ax = Mathf.Abs(dir.x);
                            float ay = Mathf.Abs(dir.y);
                            float az = Mathf.Abs(dir.z);

                            float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                            if (maxElement == ax)
                            {
                                var marker1 = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                                axis = AXIS.Y;

                                if (Vector3.Dot(dir, Vector3.right) >= 0)
                                {
                                    if (marker1)
                                    {
                                        direction = -1;
                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                }
                                else
                                {
                                    if (marker1)
                                    {
                                        direction = 1;
                                    }
                                    else
                                    {
                                        direction = -1;
                                    }
                                }
                            }
                            else if (maxElement == ay)
                            {
                                var marker1 = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                                axis = AXIS.X;

                                if (Vector3.Dot(Vector3.up, dir) >= 0)
                                {
                                    if (marker1)
                                    {
                                        direction = 1;
                                    }
                                    else
                                    {
                                        direction = -1;
                                    }
                                }
                                else
                                {
                                    if (marker1)
                                    {
                                        direction = -1;
                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                }
                            }
                        }
                        else if (xz1)
                        {
                            float ax = Mathf.Abs(dir.x);
                            float ay = Mathf.Abs(dir.y);
                            float az = Mathf.Abs(dir.z);

                            float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                            if (maxElement == ax)
                            {
                                var marker1 = firstSelectedCube.GetComponent<MarkAsUpFace>();

                                axis = AXIS.Z;

                                if (Vector3.Dot(dir, Vector3.right) >= 0)
                                {
                                    if (marker1)
                                    {
                                        direction = -1;
                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                }
                                else
                                {
                                    if (marker1)
                                    {
                                        direction = 1;
                                    }
                                    else
                                    {
                                        direction = -1;
                                    }
                                }
                            }
                            else if (maxElement == az)
                            {
                                var marker1 = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                                axis = AXIS.X;

                                if (Vector3.Dot(dir, Vector3.forward) > 0)
                                {
                                    if (marker1)
                                    {
                                        direction = -1;
                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                }
                                else
                                {
                                    if (marker1)
                                    {
                                        direction = 1;
                                    }
                                    else
                                    {
                                        direction = -1;
                                    }
                                }
                            }
                        }
                        else if (yz1)
                        {
                            float ax = Mathf.Abs(dir.x);
                            float ay = Mathf.Abs(dir.y);
                            float az = Mathf.Abs(dir.z);

                            float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                            if (maxElement == az)
                            {
                                var marker1 = firstSelectedCube.GetComponent<MarkAsRightFace>();

                                axis = AXIS.Y;

                                if (Vector3.Dot(dir, Vector3.back) >= 0)
                                {
                                    if (marker1)
                                    {
                                        direction = 1;
                                    }
                                    else
                                    {
                                        direction = -1;
                                    }
                                }
                                else
                                {
                                    if (marker1)
                                    {
                                        direction = -1;
                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                }
                            }
                            else if (maxElement == ay)
                            {
                                var marker1 = firstSelectedCube.GetComponent<MarkAsRightFace>();

                                axis = AXIS.Z;

                                if (Vector3.Dot(dir, Vector3.up) > 0)
                                {
                                    if (marker1)
                                    {
                                        direction = 1;
                                    }
                                    else
                                    {
                                        direction = -1;
                                    }
                                }
                                else
                                {
                                    if (marker1)
                                    {
                                        direction = -1;

                                    }
                                    else
                                    {
                                        direction = 1;
                                    }
                                }
                            }
                        }

                        rubikController.SetSelectedCube(firstSelectedCube.transform.parent.parent.gameObject);

                        rubikController.Rotate(axis, direction * 90, direction);
                    }
                }
                if (cameraOrbitMode)
                {
                    Camera.main.transform.SetParent(transform);
                    transform.rotation = Quaternion.identity;
                    Camera.main.transform.SetParent(null);
                }
                cube = null;
                dragging = false;
                lockDragging = false;

                firstSelectedCube = null;
                lastSelectedCube = null;

                cameraOrbitMode = false;
                cubeRotationMode = false;
            }
        }
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
