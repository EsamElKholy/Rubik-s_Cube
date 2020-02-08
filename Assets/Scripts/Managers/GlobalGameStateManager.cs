using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalGameStateManager : MonoBehaviour
{
    [SerializeField]
    private GlobalGameState globalGameState;

    public void OnMainMenu()
    {
        if (globalGameState)
        {
            globalGameState.SetCurrentGameState(GameState.MainMenu);
        }
    }

    public void OnGameSetup()
    {
        if (globalGameState)
        {
            globalGameState.SetCurrentGameState(GameState.GameSetup);
        }
    }

    public void OnInGame()
    {
        if (globalGameState)
        {
            globalGameState.SetCurrentGameState(GameState.InGame);
        }
    }

    public void OnPause()
    {
        if (globalGameState)
        {
            globalGameState.SetCurrentGameState(GameState.PauseMenu);
        }
    }

    public void OnWin()
    {
        if (globalGameState)
        {
            globalGameState.SetCurrentGameState(GameState.Win);
        }
    }
}
