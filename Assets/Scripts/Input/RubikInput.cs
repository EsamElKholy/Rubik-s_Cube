using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikInput : MonoBehaviour
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

    private Quaternion oldCubeR;
    private Vector3 oldCubeP;
    private Quaternion oldCamR;
    private Vector3 oldCamP;

    private bool firstOrbit = true;

    private float orbitSpeed = 150;

    [HideInInspector]
    public bool cubeRotationMode = false;
    [HideInInspector]
    public bool cameraOrbitMode = false;

    private float minZoom = 30;
    private float maxZoom = 90;
    private float zoomSpeed = 800;

    [HideInInspector]
    public float x;
    [HideInInspector]
    public float y;

    public new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        rubikController = GetComponent<RubikController>();
    }

    public void ExecuteZoomInput(float zoomValue)
    {
        if (zoomValue != 0)
        {
            if (camera)
            {
                camera.fieldOfView -= zoomValue * zoomSpeed * Time.deltaTime;

                camera.fieldOfView = Mathf.Clamp(camera.fieldOfView, minZoom, maxZoom);
            }
        }
    }

    public void ProcessDragInput(Vector3 inputPosition)
    {
        var ray = camera.ScreenPointToRay(inputPosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (cameraOrbitMode)
            {
                camera.transform.SetParent(transform);
                transform.rotation = Quaternion.identity;
                camera.transform.SetParent(null);
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

    public void ProcessCubeRotation(Vector3 inputPosition, float _x, float _y)
    {
        var ray = camera.ScreenPointToRay(inputPosition);

        x = _x;
        y = _y;

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

    public void ExecuteCubeRotation()
    {
        if (firstSelectedCube && !firstHitPosition.Equals(lastHitPosition) && !lockDragging)
        {
            lockDragging = true;

            float minScale = Mathf.Min(
                firstSelectedCube.transform.localScale.x, Mathf.Min(
                    firstSelectedCube.transform.localScale.y,
                    firstSelectedCube.transform.localScale.z));

            bool xy = false, xz = false, yz = false;

            AXIS axis = AXIS.NONE;

            if (minScale == firstSelectedCube.transform.localScale.x)
            {
                yz = true;
            }
            else if (minScale == firstSelectedCube.transform.localScale.y)
            {
                xz = true;
            }
            else if (minScale == firstSelectedCube.transform.localScale.z)
            {
                xy = true;
            }

            float direction = 1;

            var dir = (lastHitPosition - firstHitPosition).normalized;

            if (xy)
            {
                float ax = Mathf.Abs(dir.x);
                float ay = Mathf.Abs(dir.y);
                float az = Mathf.Abs(dir.z);

                float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                if (maxElement == ax)
                {
                    var marker = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                    axis = AXIS.Y;

                    if (Vector3.Dot(dir, Vector3.right) >= 0)
                    {
                        if (marker)
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
                        if (marker)
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
                    var marker = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                    axis = AXIS.X;

                    if (Vector3.Dot(Vector3.up, dir) >= 0)
                    {
                        if (marker)
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
                        if (marker)
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
            else if (xz)
            {
                float ax = Mathf.Abs(dir.x);
                float ay = Mathf.Abs(dir.y);
                float az = Mathf.Abs(dir.z);

                float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                if (maxElement == ax)
                {
                    var marker = firstSelectedCube.GetComponent<MarkAsUpFace>();

                    axis = AXIS.Z;

                    if (Vector3.Dot(dir, Vector3.right) >= 0)
                    {
                        if (marker)
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
                        if (marker)
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
                    var marker = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                    axis = AXIS.X;

                    if (Vector3.Dot(dir, Vector3.forward) > 0)
                    {
                        if (marker)
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
                        if (marker)
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
            else if (yz)
            {
                float ax = Mathf.Abs(dir.x);
                float ay = Mathf.Abs(dir.y);
                float az = Mathf.Abs(dir.z);

                float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                if (maxElement == az)
                {
                    var marker = firstSelectedCube.GetComponent<MarkAsRightFace>();

                    axis = AXIS.Y;

                    if (Vector3.Dot(dir, Vector3.back) >= 0)
                    {
                        if (marker)
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
                        if (marker)
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
                    var marker = firstSelectedCube.GetComponent<MarkAsRightFace>();

                    axis = AXIS.Z;

                    if (Vector3.Dot(dir, Vector3.up) > 0)
                    {
                        if (marker)
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
                        if (marker)
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

            rubikController.Rotate(axis, direction * 90, direction, RotationCommand.CommandType.Manual);
        }
    }

    public void ExecuteCameraOrbit(float x, float y)
    {
        if (!firstOrbit)
        {
            camera.transform.rotation = oldCamR;
            camera.transform.position = oldCamP;
            transform.rotation = oldCubeR;
            transform.position = oldCubeP;
        }

        Vector2 delta = new Vector2(x * orbitSpeed, y * orbitSpeed);
        delta.y = ClampAngle(delta.y, -360, 360);
        camera.transform.RotateAround(Vector3.zero, Vector3.up, delta.x * Time.deltaTime);
        transform.RotateAround(Vector3.zero, camera.transform.right, delta.y * Time.deltaTime);
        camera.transform.LookAt(Vector3.zero, camera.transform.up);

        oldCamR = camera.transform.rotation;
        oldCamP = camera.transform.position;
        oldCubeR = transform.rotation;
        oldCubeP = transform.position;
        firstOrbit = false;
    }

    public void FinishDragInput()
    {
        if (cubeRotationMode)
        {
            ExecuteCubeRotation();
        }

        if (cameraOrbitMode)
        {
            camera.transform.SetParent(transform);
            transform.rotation = Quaternion.identity;
            camera.transform.SetParent(null);
        }
        cube = null;
        dragging = false;
        lockDragging = false;

        firstSelectedCube = null;
        lastSelectedCube = null;

        cameraOrbitMode = false;
        cubeRotationMode = false;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public RubikController GetController()
    {
        return rubikController;
    }
}
