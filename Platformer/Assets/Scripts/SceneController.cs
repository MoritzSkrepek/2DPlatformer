using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI infoTMP;

    [Header("Notification visibility duration")]
    [SerializeField] private float visibilityTimer;

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

    // Load level selection
    public void OnLevelSelectionClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

    // Continue game in next level
    public void OnContinueButtonClicked()
    {
        int nextLevelID = ProgressManager.Instance.GetNextIncompleteLevelID();
        if (nextLevelID != -1)
        {
            PlayerPrefs.SetString("LoadType", "Continue");
            PlayerPrefs.SetInt("NextLevelID", nextLevelID);
            PlayerPrefs.Save();
            SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.Log("All levels complete");
            PlayerPrefs.DeleteKey("NextLevelID");
            PlayerPrefs.DeleteKey("LoadType");
            PlayerPrefs.Save();
            StartCoroutine(SetNotifyText());
        }
    }

    // Go back to main menu
    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Quit game
    public void OnExitClicked()
    {
        // For closing in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    // Visualise info text for user
    private IEnumerator SetNotifyText()
    {
        infoTMP.gameObject.SetActive(true);
        yield return new WaitForSeconds(visibilityTimer);
        infoTMP.gameObject.SetActive(false);
    }
}
