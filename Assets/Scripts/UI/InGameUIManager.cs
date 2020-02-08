using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum ConfirmationMode
{
    RestartLevel,
    QuitLevel
}

public class InGameUIManager : MonoBehaviour
{
    public Text timer;
    public Text confirmationText;
    public Button undo;
    public Button pause;

    [KAI.KAIEvent]
    public KAI.CustomEvent restartLevelEvent;
    [KAI.KAIEvent]
    public KAI.CustomEvent returnToMainMenuEvent;

    private ConfirmationMode currentConfirmationMode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.globalGameState.GetCurrentGameState() == GameState.GameSetup)
        {
            undo.interactable = false;
            pause.interactable = false;
        }

        if (GameManager.Instance.globalGameState.GetCurrentGameState() == GameState.InGame)
        {
            undo.interactable = true;
            pause.interactable = true;
        }

        if (GameManager.Instance.playerData)
        {
            timer.text = GameManager.Instance.playerData.time.ToString();
        }
    }

    public void ToggleTimer(Toggle val)
    {
        timer.gameObject.SetActive(!val.isOn);
    }

    public void SetConfirmationMode(int mode)
    {
        currentConfirmationMode = (ConfirmationMode)mode;

        switch ((ConfirmationMode)mode)
        {
            case ConfirmationMode.RestartLevel:
                {
                    confirmationText.text = "Are you sure you want to restart the level";
                }
                break;
            case ConfirmationMode.QuitLevel:
                {
                    confirmationText.text = "Are you sure you want to return to main menu";
                }
                break;
            default:
                break;
        }
    }

    public void OnConfirmation()
    {
        switch (currentConfirmationMode)
        {
            case ConfirmationMode.RestartLevel:
                {
                    if (restartLevelEvent)
                    {
                        restartLevelEvent.Raise();
                    }
                }
                break;
            case ConfirmationMode.QuitLevel:
                {
                    if (returnToMainMenuEvent)
                    {
                        returnToMainMenuEvent.Raise();
                    }
                }
                break;
            default:
                break;
        }
    }
}
