using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("Game Controller")]
    [SerializeField] private GameObject gameControllerObject;
    private GameController gameControllerScript;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;
    private InputActionMap playerActionMap;
    private InputAction moveAction;
    private InputAction slideAction;
    private InputAction jumpAction;

    private void Awake()
    {
        playerActionMap = inputActions.FindActionMap("Player");
        moveAction = playerActionMap.FindAction("Move");
        slideAction = playerActionMap.FindAction("Slide");
        jumpAction = playerActionMap.FindAction("Jump");

        gameControllerScript = gameControllerObject.GetComponent<GameController>();

        IgnoreMovementActions();
    }

    public void ContinueLevel()
    {
        gameControllerScript.UnpauseGame();
        UnavoidMovementAction();
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // For closing in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    // Ignore all movement actions
    private void IgnoreMovementActions()
    {
        moveAction.Disable();
        jumpAction.Disable();
        slideAction.Disable();
    }

    // Un-ignore all movement actions
    private void UnavoidMovementAction()
    {
        moveAction.Enable();
        jumpAction.Enable();
        slideAction.Enable();
    }
}
