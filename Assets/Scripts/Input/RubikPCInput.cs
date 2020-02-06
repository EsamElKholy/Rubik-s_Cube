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

    // Start is called before the first frame update
    void Start()
    {
        rubikController = GetComponent<RubikController>();    
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
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


        if (Input.GetMouseButtonUp(1))
        {
            Camera.main.transform.SetParent(transform);
            transform.rotation = Quaternion.identity;
            Camera.main.transform.SetParent(null);
        }

        float x = 0;
        float y = 0;

        if (Input.GetMouseButton(0))
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

        if (Input.GetMouseButtonUp(0))
        {
            if (firstSelectedCube && !firstHitPosition.Equals(lastHitPosition) && !lockDragging)
            {
                lockDragging = true;

                float minScale1 = Mathf.Min(
                    firstSelectedCube.transform.localScale.x, Mathf.Min(
                        firstSelectedCube.transform.localScale.y, 
                        firstSelectedCube.transform.localScale.z));

                float minScale2 = Mathf.Min(
                    lastSelectedCube.transform.localScale.x, Mathf.Min(
                        lastSelectedCube.transform.localScale.y,
                        lastSelectedCube.transform.localScale.z));

                bool xy1 = false, xz1 = false, yz1 = false;
                bool xy2 = false, xz2 = false, yz2 = false;

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

                if (minScale2 == lastSelectedCube.transform.localScale.x)
                {
                    yz2 = true;
                }
                else if (minScale2 == lastSelectedCube.transform.localScale.y)
                {
                    xz2 = true;
                }
                else if (minScale2 == lastSelectedCube.transform.localScale.z)
                {
                    xy2 = true;
                }

                float direction = 1;

                var dir = (lastHitPosition - firstHitPosition).normalized;

                if (xy1)
                {
                    float ax = Mathf.Abs(dir.x);
                    float ay = Mathf.Abs(dir.y);
                    float az = Mathf.Abs(dir.z);

                    float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));
                    var marker = firstSelectedCube.GetComponent<MarkAsFrontFace>();

                    if (maxElement == ax)
                    {
                        axis = AXIS.Y;

                        if (Vector3.Dot(dir, Vector3.right) >= 0)
                        {
                            if (marker || xy2 || yz2)
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
                            if (marker || xy2 || yz2)
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
                        axis = AXIS.X;

                        if (Vector3.Dot(Vector3.up, dir) >= 0)
                        {
                            if (marker || xy2 || yz2)
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
                            if (marker || xy2 || yz2)
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
                    var marker = firstSelectedCube.GetComponent<MarkAsUpFace>();

                    if (maxElement == ax)
                    {
                        axis = AXIS.Z;

                        if (Vector3.Dot(dir, Vector3.right) >= 0)
                        {
                            if (marker || xy2 || yz2)
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
                            if (marker || xy2 || yz2)
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
                        axis = AXIS.X;

                        if (Vector3.Dot(dir, Vector3.forward) > 0)
                        {
                            if (marker || xy2 || yz2)
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
                            if (marker || xy2 || yz2)
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
                else if (yz1)
                {
                    float ax = Mathf.Abs(dir.x);
                    float ay = Mathf.Abs(dir.y);
                    float az = Mathf.Abs(dir.z);

                    float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));
                    var marker = lastSelectedCube.GetComponent<MarkAsRightFace>();

                    if (maxElement == az)
                    {
                        axis = AXIS.Y;

                        if (Vector3.Dot(dir, Vector3.back) >= 0)
                        {
                            if (marker || xy2 || xz2)
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
                            if (marker || xy2 || xz2)
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
                        axis = AXIS.Z;

                        if (Vector3.Dot(dir, Vector3.up) >= 0)
                        {
                            if (marker || xy2 || xz2)
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
                            if (marker || xy2 || xz2)
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

            cube = null;
            dragging = false;
            lockDragging = false;

            firstSelectedCube = null;
            lastSelectedCube = null;
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
