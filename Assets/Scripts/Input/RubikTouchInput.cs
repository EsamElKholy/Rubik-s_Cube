using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RubikInput))]
public class RubikTouchInput : MonoBehaviour
{
    private RubikInput rubikInput;
    private Vector2[] lastZoomPositions;
    private bool wasZoomingLastFrame;
    private float lastOffset;
    // Start is called before the first frame update
    void Start()
    {
        rubikInput = GetComponent<RubikInput>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (GameManager.Instance.globalGameState.GetCurrentGameState() == GameState.InGame && !rubikInput.GetController().scrambling && !rubikInput.GetController().rotationLocked)
        {           
            if (Input.touchCount == 2)
            {
                Vector2[] newPositions = new Vector2[] { Input.GetTouch(0).position, Input.GetTouch(1).position };

                if (!wasZoomingLastFrame)
                {
                    lastZoomPositions = newPositions;
                    wasZoomingLastFrame = true;
                }
                else
                {
                    if (Input.GetTouch(1).phase == TouchPhase.Moved)
                    {
                        float newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                        float oldDistance = Vector2.Distance(lastZoomPositions[0], lastZoomPositions[1]);
                        float offset = newDistance - oldDistance;

                        float zoomValue = offset;

                        rubikInput.ExecuteZoomInput(zoomValue / 100);
                    }

                    lastZoomPositions = newPositions;
                }
            }

            rubikInput.x = 0;
            rubikInput.y = 0;

            if (Input.touchCount == 1)
            {
                var touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began)
                {
                    rubikInput.ProcessDragInput(touch.position);
                }

                if (rubikInput.cubeRotationMode)
                {
                    rubikInput.ProcessCubeRotation(touch.position, touch.deltaPosition.x, touch.deltaPosition.y);
                }

                if (rubikInput.cameraOrbitMode)
                {
                    rubikInput.ExecuteCameraOrbit(touch.deltaPosition.x / 10, touch.deltaPosition.y / 10);
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    rubikInput.FinishDragInput();
                }
            }
        }
    }
}
