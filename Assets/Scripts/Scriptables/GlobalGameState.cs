using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    MainMenu,
    GameSetup,
    InGame,
    PauseMenu,
    Scramble,
    Win
}

[CreateAssetMenu(fileName = "New Global Game State", menuName = "Game State/Global Game State")]
public class GlobalGameState : ScriptableObject
{
    [SerializeField]
    private GameState currentGameState;

    private void OnEnable()
    {
        currentGameState = GameState.MainMenu;
    }

    public void SetCurrentGameState(GameState state)
    {
        currentGameState = state;
    }

    public GameState GetCurrentGameState()
    {
        return currentGameState;
    }
}
