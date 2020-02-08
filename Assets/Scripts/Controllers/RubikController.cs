using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct RotationCommand
{
    public enum CommandType
    {
        Auto,
        Manual
    }

    public CommandType commandType;

    public int xIndex;
    public int yIndex;
    public int zIndex;

    public AXIS axis;
    public float direction;
    public float angle;
}

public class RubikController : MonoBehaviour
{
    [KAI.KAIEvent]
    public KAI.GameEvent onScrambleFinish;

    private AXIS currentAxis = AXIS.NONE;
    private bool rotationLocked;
    [HideInInspector]
    public bool scrambling = false;

    private Stack<RotationCommand> rotationCommands = new Stack<RotationCommand>();

    // Start is called before the first frame update
    void Start()
    {
        //Scramble();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.globalGameState.GetCurrentGameState() == GameState.GameSetup)
        {
            Scramble();
        }
    }

    public void Rotate(AXIS axis, float angle, float direction, RotationCommand.CommandType rotationType, float rotationTime = 0.5f)
    {
        if (!rotationLocked)
        {
            RotationCommand command = new RotationCommand();

            command.xIndex = int.Parse(RubikGenerator.Instance.selecedCube.name[0].ToString()) - 1;
            command.yIndex = int.Parse(RubikGenerator.Instance.selecedCube.name[1].ToString()) - 1;
            command.zIndex = int.Parse(RubikGenerator.Instance.selecedCube.name[2].ToString()) - 1;

            command.direction = -direction;
            command.angle = -angle;
            command.axis = axis;
            command.commandType = rotationType;

            if (rotationType == RotationCommand.CommandType.Manual)
            {
                rotationCommands.Push(command);
            }

            rotationLocked = true;

            var slice = RubikGenerator.Instance.GetSlice(axis);

            StartCoroutine(RotateSlice(slice, axis, angle, rotationTime, direction));        
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

        if (!scrambling)
        {
            RubikGenerator.Instance.RecordColors(GameManager.Instance.playerData);
        }
    }

    private RotationCommand GenerateRotationCommand()
    {
        RotationCommand rotationCommand = new RotationCommand();

        rotationCommand.xIndex = Random.Range(0, RubikGenerator.Instance.size - 1);
        rotationCommand.yIndex = Random.Range(0, RubikGenerator.Instance.size - 1);
        rotationCommand.zIndex = Random.Range(0, RubikGenerator.Instance.size - 1);

        rotationCommand.axis = (AXIS)Random.Range(0, 2);
        rotationCommand.direction = Mathf.Clamp(Random.Range(-1, 1), -1, 1);
        rotationCommand.angle = 90;

        return rotationCommand;
    }

    public void Scramble()
    {
        if (!scrambling)
        {
            scrambling = true;
            StartCoroutine(AutoRotate(3, 0.3f));
        }
    }

    private IEnumerator AutoRotate(float time, float timeForEachRotation)
    {
        yield return new WaitForSeconds(0.8f);

        int numOfRotations = Mathf.CeilToInt(time / timeForEachRotation);

        for (int i = 0; i < numOfRotations; i++)
        {
            var command = GenerateRotationCommand();
            SetSelectedCube(command.xIndex, command.yIndex, command.zIndex);
            Rotate(command.axis, command.angle, command.direction, RotationCommand.CommandType.Auto, timeForEachRotation);

            while (rotationLocked)
            {
                yield return null;
            }
        }

        scrambling = false;

        if (onScrambleFinish)
        {
            onScrambleFinish.Raise();
        }
    }

    public void UndoRotation()
    {
        if (rotationCommands.Count > 0)
        {
            var command = rotationCommands.Pop();

            SetSelectedCube(command.xIndex, command.yIndex, command.zIndex);
            Rotate(command.axis, command.angle, command.direction, RotationCommand.CommandType.Auto);
        }
    }

    public void SetSelectedCube(GameObject cube)
    {
        RubikGenerator.Instance.selecedCube = cube;
    }

    public void SetSelectedCube(int x, int y, int z)
    {
        RubikGenerator.Instance.SetSelectedCube(x, y, z);
    }
}
