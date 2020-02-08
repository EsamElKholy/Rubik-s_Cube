using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [KAI.KAIEvent]
    public KAI.GameEvent AppStartEvent;

    public PlayerData playerData;
    public GlobalGameState globalGameState;

    public StringVariable mainMenuScene;
    public StringVariable gameScene;

    private void OnEnable()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AppStartEvent)
        {
            AppStartEvent.Raise();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (globalGameState.GetCurrentGameState() == GameState.InGame)
        {
            if (playerData)
            {
                playerData.IncreamentTime(Time.deltaTime);
            }
        }
    }

    public void LoadGame()
    {
        if (mainMenuScene)
        {
            if (mainMenuScene.Value.Length > 0)
            {
                if (gameScene && gameScene.Value.Length > 0)
                {
                    var game = SceneManager.GetSceneByName(gameScene.Value);

                    if (game != null && !game.isLoaded)
                    {
                        SceneManager.LoadSceneAsync(gameScene.Value, LoadSceneMode.Additive);
                    }
                }

                var menu = SceneManager.GetSceneByName(mainMenuScene.Value);

                if (menu != null && menu.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(menu);
                }                
            }
        }
    }

    public void LoadMainMenu()
    {
        if (gameScene)
        {
            if (gameScene.Value.Length > 0)
            {
                if (mainMenuScene && mainMenuScene.Value.Length > 0)
                {
                    var menu = SceneManager.GetSceneByName(mainMenuScene.Value);

                    if (menu != null && !menu.isLoaded)
                    {
                        SceneManager.LoadSceneAsync(mainMenuScene.Value, LoadSceneMode.Additive);
                    }
                }

                var game = SceneManager.GetSceneByName(gameScene.Value);

                if (game != null && game.isLoaded)
                {
                    SceneManager.UnloadSceneAsync(game);
                }                
            }
        }
    }

    public void StartNewGame()
    {
        LoadGame();

        StartCoroutine(StartGame(false));
    }

    private IEnumerator StartGame(bool isContinue)
    {
        var game = SceneManager.GetSceneByName(gameScene.Value);

        while (!game.isLoaded)
        {
            yield return new WaitForEndOfFrame();
        }

        if (RubikGenerator.Instance)
        {
            if (!isContinue)
            {
                playerData.WipeOutData();
            }

            RubikGenerator.Instance.size = playerData.cubeSize;
            RubikGenerator.Instance.GenerateCube();

            FindObjectOfType<RubikInput>().ResetCamera();
            RubikGenerator.Instance.ResetPosition(RubikGenerator.Instance.cubeRoot.gameObject);

            if (isContinue)
            {
                RubikGenerator.Instance.SetColors(playerData);
            }
        }
    }

    public void ContinueGame()
    {
        LoadGame();

        StartCoroutine(StartGame(true));
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
