using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RubikInput))]
public class RubikPCInput : MonoBehaviour
{
    private RubikInput rubikInput;
    
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
            float zoomValue = Input.GetAxis("Mouse ScrollWheel");

            rubikInput.ExecuteZoomInput(zoomValue);

            rubikInput.x = 0;
            rubikInput.y = 0;

            if (Input.GetMouseButtonDown(0))
            {
                rubikInput.ProcessDragInput(Input.mousePosition);
            }

            if (rubikInput.cubeRotationMode)
            {
                rubikInput.ProcessCubeRotation(Input.mousePosition, Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            if (rubikInput.cameraOrbitMode)
            {
                rubikInput.ExecuteCameraOrbit(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            }

            if (Input.GetMouseButtonUp(0))
            {
                rubikInput.FinishDragInput();
            }
        }
    }
}
