using System;
using UnityEngine;

public class Game : MonoBehaviour
{
    public event Action GameStarted, GameOver, GamePaused, GameResumed;
    public static Game Instance { get; private set; }
    public GameState State { get; private set; }
    public bool IsPaused { get; private set; }
    private InputManager _input;


    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance.gameObject);
        }

        Instance = this;

        _input = ProjectContext.Instance.InputManager;
        State = GameState.Playing;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // temp
        LeanTween.reset();
    }

    private void OnEnable() =>_input.EscapeTriggered += TogglePause;
    private void OnDisable() => _input.EscapeTriggered -= TogglePause;


    public void StartGame()
    {
        if (State != GameState.Off)
            throw new InvalidOperationException("Cannot start the game from the current state!");

        State = GameState.Playing;
        GameStarted?.Invoke();
    }

    public void ExitGame()
    {
        if (State != GameState.Paused)
            throw new InvalidOperationException("Cannot finish the game from the current state!");

        State = GameState.GameOver;
        GameOver?.Invoke();

        Cleanup();

        ProjectContext.Instance.SceneLoader.SwitchSceneAsync(Constants.SCENE_MAIN_MENU);
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            IsPaused = false;
            Time.timeScale = 1f;

            GameResumed?.Invoke();
            State = GameState.Playing;

            ProjectContext.Instance.GUIManager.ExitAll();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            if (State != GameState.Playing)
                throw new InvalidOperationException("Cannot pause the game from the current state!");

            IsPaused = true;
            Time.timeScale = 0f;

            GamePaused?.Invoke();
            State = GameState.Paused;

            ProjectContext.Instance.GUIManager.LoadAndProcessPauseMenu();

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
    }

    private void Cleanup()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        ProjectContext.Instance.GUIManager.ExitAll();
    }

    public enum GameState
    {
        Off,
        Playing,
        Paused,
        GameOver
    }
}