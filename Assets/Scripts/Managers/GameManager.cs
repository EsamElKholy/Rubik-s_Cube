using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [KAI.KAIEvent]
    public KAI.GameEvent AppStartEvent;

    public PlayerData playerData;
    public GlobalGameState globalGameState;

    public StringVariable mainMenuScene;
    public StringVariable gameScene;
    private int minCubeSize = 2;
    private int cubeSize;

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

    private void Awake()
    {
        if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (AppStartEvent)
        {
            if (!Directory.Exists(Application.persistentDataPath + "/Saves/"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Saves/");
            }

            Load();
            AppStartEvent.Raise();
            StartCoroutine(Save());
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
            RubikRotator.Instance.ResetCube(RubikGenerator.Instance.cubeRoot.gameObject);

            if (isContinue)
            {
                RubikCubeManager.Instance.SetColors(playerData);
            }
        }
    }

    public void ContinueGame()
    {
        Load();
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

    public IEnumerator Save()
    {
        while (Application.isPlaying)
        {
            //if (!File.Exists(Application.persistentDataPath + "/Saves/" + "player.json"))
            //{
            //    File.Create(Application.persistentDataPath + "/Saves/" + "player.json");
            //}

            string json = JsonUtility.ToJson(playerData);
            File.WriteAllText(Application.persistentDataPath + "/Saves/" + "player.json", json);

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/Saves/" + "player.json"))
        {
            string json = File.ReadAllText(Application.persistentDataPath + "/Saves/" + "player.json");
            JsonUtility.FromJsonOverwrite(json, playerData);
        }         
    }

    private void OnApplicationQuit()
    {
        //Save();
    }

    public void SetCubeSizeMode(Dropdown mode)
    {
        SetCubeSize(mode.value + minCubeSize);
    }

    public void SetCubeSize(int size)
    {
        cubeSize = size;
        playerData.cubeSize = Mathf.Clamp(cubeSize, 2, 6);
    }
}
