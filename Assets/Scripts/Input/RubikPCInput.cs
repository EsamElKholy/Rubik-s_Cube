using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikPCInput : MonoBehaviour
{
    private RubikController rubikController;
    private RaycastHit hit;
    private Vector3 oldMousePos;
    private GameObject firstSelectedCube;
    private GameObject lastSelectedCube;
    private GameObject cube;
    private Vector3 firstHitPosition;
    private Vector3 lastHitPosition;
    private bool dragging = false;
    private bool lockDragging = false;

    // Start is called before the first frame update
    void Start()
    {
        rubikController = GetComponent<RubikController>();    
    }

    // Update is called once per frame
    void Update()
    {
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
                        axis = AXIS.Y;

                        if (dir.x >= 0)
                        {
                            direction = -1;
                        }
                        else
                        {
                            direction = 1;
                        }
                    }
                    else if (maxElement == ay)
                    {
                        axis = AXIS.X;

                        if (dir.y >= 0)
                        {
                            direction = 1;
                        }
                        else
                        {
                            direction = -1;
                        }
                    }
                }
                else if (xz1)
                {
                    axis = AXIS.Y;

                    float ax = Mathf.Abs(dir.x);
                    float ay = Mathf.Abs(dir.y);
                    float az = Mathf.Abs(dir.z);

                    float maxElement = Mathf.Max(ax, Mathf.Max(ay, az));

                    if (maxElement == ax)
                    {
                        axis = AXIS.Z;

                        if (dir.x >= 0)
                        {
                            direction = -1;
                        }
                        else
                        {
                            direction = 1;
                        }
                    }
                    else if (maxElement == az)
                    {
                        axis = AXIS.X;

                        if (dir.z > 0)
                        {
                            direction = 1;
                        }
                        else
                        {
                            direction = -1;
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

                        if (Vector3.Dot(dir, Vector3.back) > 0)
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
                        axis = AXIS.Z;

                        if (Vector3.Dot(dir, Vector3.up) >= 0)
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

                rubikController.Rotate(axis, direction * 90, direction);
            }

            cube = null;
            dragging = false;
            lockDragging = false;
        }
    }
}
