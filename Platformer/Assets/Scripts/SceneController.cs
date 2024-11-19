using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI infoTMP;

    [Header("Notification visibility duration")]
    [SerializeField] private float visibilityTimer;

    public void OnLevelSelectionClicked()
    {
        SceneManager.LoadScene("GameScene");
    }

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

    public void OnBackButtonClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnExitClicked()
    {
        // For closing in editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

        Application.Quit();
    }

    private IEnumerator SetNotifyText()
    {
        infoTMP.gameObject.SetActive(true);
        yield return new WaitForSeconds(visibilityTimer);
        infoTMP.gameObject.SetActive(false);
    }
}
