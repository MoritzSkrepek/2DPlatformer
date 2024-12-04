using UnityEngine;
using UnityEngine.InputSystem;

public class GameStateController : MonoBehaviour
{
    public static GameStateController Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    [Header("UI References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gameOverScreen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetGameState(GameState.Playing); // Initial state is playing
    }

    // Set given state
    public void SetGameState(GameState newState)
    {
        ExitState(CurrentState);
        CurrentState = newState;
        EnterState(CurrentState);
    }

    // Enter given state
    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                Time.timeScale = 1.0f;
                pauseMenu.SetActive(false);
                gameOverScreen.SetActive(false);
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                pauseMenu.SetActive(true);
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                gameOverScreen.SetActive(true);
                break;
        }
    }

    // Exit given state
    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.Paused:
                pauseMenu.SetActive(false);
                break;

            case GameState.GameOver:
                gameOverScreen.SetActive(false);
                break;
        }
    }

    // Pause game state
    public void PauseGame()
    {
        if (CurrentState == GameState.Playing)
        {
            SetGameState(GameState.Paused);
        }
    }

    // Set game state to playing
    public void UnpauseGame()
    {
        if (CurrentState == GameState.Paused)
        {
            SetGameState(GameState.Playing);
        }
    }

    // Triggers gameover game state
    public void TriggerGameOver()
    {
        SetGameState(GameState.GameOver);
    }
}

public enum GameState
{
    Playing,
    Paused,
    GameOver
}
