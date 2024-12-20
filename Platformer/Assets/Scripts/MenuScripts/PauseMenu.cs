using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public void ContinueLevel()
    {
        GameStateController.Instance.UnpauseGame();
        InputController.Instance.EnableInputActions();
    }

    public void ReturnToMainMenu()
    {
        GameStateController.Instance.UnpauseGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        SceneController.Instance.OnExitClicked();
    }
}
