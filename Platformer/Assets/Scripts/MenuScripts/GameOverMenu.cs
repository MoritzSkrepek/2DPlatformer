using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverMenu : MonoBehaviour
{
    public void RetryLevel()
    {
        GameController.Instance.RetryCurrentLevel();
        GameStateController.Instance.SetGameState(GameState.Playing);
        InputController.Instance.EnableInputActions();
    }

    public void ReturnToMainMenu()
    {
        SceneController.Instance.OnBackButtonClicked();
    }

    public void QuitGame()
    {
        SceneController.Instance.OnExitClicked();
    }
}
