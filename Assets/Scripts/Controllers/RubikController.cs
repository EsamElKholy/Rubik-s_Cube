using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RubikController : MonoBehaviour
{
    private AXIS currentAxis = AXIS.NONE;
    private bool rotationLocked;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate(AXIS axis, float angle, float direction)
    {
        if (!rotationLocked)
        {
            rotationLocked = true;

            var slice = RubikGenerator.Instance.GetSlice(axis);

            StartCoroutine(RotateSlice(slice, axis, angle, 0.5f, direction));        
        }
    }

    private IEnumerator RotateSlice(GameObject slice, AXIS axis, float angle, float time, float direction)
    {
        float currentTime = 0;

        Quaternion oldRotation = slice.transform.localRotation;
        Quaternion rotation = Quaternion.identity;

        switch (axis)
        {
            case AXIS.X:
                {
                    rotation = Quaternion.Euler(angle, 0, 0) * oldRotation;
                }
                break;
            case AXIS.Y:
                {
                    rotation = Quaternion.Euler(0, angle, 0) * oldRotation;
                }
                break;
            case AXIS.Z:
                {
                    rotation = Quaternion.Euler(0, 0, angle) * oldRotation;
                }
                break;
            case AXIS.NONE:
                break;
            default:
                break;
        }

        while (currentTime < time)
        {
            slice.transform.localRotation = Quaternion.Slerp(oldRotation, rotation, currentTime / time);
            currentTime += Time.deltaTime;

            yield return null;
        }

        slice.transform.localRotation = rotation;
        rotationLocked = false;

        RubikGenerator.Instance.RotateTiles(axis, currentAxis, direction);

        RubikGenerator.Instance.EmptySlice();

        currentAxis = axis;
    }

    public void SetSelectedCube(GameObject cube)
    {
        RubikGenerator.Instance.selecedCube = cube;
    }
}
