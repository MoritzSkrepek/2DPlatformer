using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

// Used for updates in the UI
public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject levelUI;
    [SerializeField] private GameObject levelSelectionUI;
    private TextMeshProUGUI timerTMP;
    private TextMeshProUGUI collectedCoinsTMP;
    private TextMeshProUGUI totalCollectedCoinsTMP;

    [Header("Player")]
    [SerializeField] private GameObject player;
    private Transform levelStartPosition;

    [Header("Criterias to enter next level")]
    [SerializeField] private int coinsToWin;

    [Header("Notification visibility duration")]
    [SerializeField] private float visibilityTimer;

    private Dictionary<int, GameObject> levels = new Dictionary<int, GameObject>();
    private int currentActiveLevelID;
    private LevelData currentLevelData;

    // Collected coins / coinworth
    private int collectedCoins;
    private int totalCoinWorth;

    // Level timer
    private float levelTimer;

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
        SetGUIComponents();
        RegisterLevels();
        // If user clicks on continue and there is a next level load into that level
        if (PlayerPrefs.GetString("LoadType") == "Continue" && PlayerPrefs.HasKey("NextLevelID"))
        {
            ContinueWithNextLevel();
            PlayerPrefs.DeleteKey("LoadType");
            PlayerPrefs.Save();
        }
        Coin.OnCoinCollected += UpdateCollectedCoinsTMP;
        LevelDoor.OnLevelDoorClicked += LoadNextLevel;
    }

    // Unsubscribe from events when objects is not loaded anymore
    private void OnDisable()
    {
        Coin.OnCoinCollected -= UpdateCollectedCoinsTMP;
        LevelDoor.OnLevelDoorClicked -= LoadNextLevel;
    }

    private void Update()
    {
        if (currentActiveLevelID != 0)
        {
            levelTimer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // User pressed Escape
    public void TogglePause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GameStateController.Instance.CurrentState == GameState.Playing)
            {
                GameStateController.Instance.PauseGame();
                InputController.Instance.DisableInputActions();
            }
            else if (GameStateController.Instance.CurrentState == GameState.Paused)
            {
                GameStateController.Instance.UnpauseGame();
                InputController.Instance.EnableInputActions();
            }
        }
    }

    // User clicked on continue and if there exists a level after the last completed one, 
    // load into this directly
    private void ContinueWithNextLevel()
    {
        if (PlayerPrefs.HasKey("NextLevelID"))
        {
            int nextLevelID = PlayerPrefs.GetInt("NextLevelID");
            LoadLevel(nextLevelID);
        }
        else
        {
            Debug.Log("All levels finished. Can't continue");
        }
    }

    // Get UI components from main game object
    private void SetGUIComponents()
    {
        timerTMP = levelUI.transform.Find("LevelTimer").GetComponent<TextMeshProUGUI>();
        collectedCoinsTMP = levelUI.transform.Find("Coins").GetComponent<TextMeshProUGUI>();
        totalCollectedCoinsTMP = levelSelectionUI.transform.Find("Total Collected Coins TMP").GetComponent<TextMeshProUGUI>();

        totalCollectedCoinsTMP.text = ProgressManager.Instance.GetTotalCollectedCoins().ToString();
    }

    private void RegisterLevels()
    {
        // Get levels in scene
        LevelData[] levelDataArray = Resources.FindObjectsOfTypeAll<LevelData>();

        // Map levels to dict
        foreach (LevelData levelData in levelDataArray)
        {
            levels[levelData.levelID] = levelData.gameObject;

            // Register each level on the progession manager to save progression
            ProgressManager.Instance.RegisterLevel(levelData.levelID, levelData.levelName);
        }
    }

    // Load specific level by id
    public void LoadLevel(int levelID)
    {
        currentActiveLevelID = levelID;
        Debug.Log(currentActiveLevelID);
        if (!levels.ContainsKey(currentActiveLevelID))
        {
            Debug.LogError("[ERROR] Level does not exist");
            return;
        }
        if (currentActiveLevelID != 0)
        {
            Debug.Log("Deactivating level");
            levels[currentActiveLevelID].SetActive(false);
        }

        levels[currentActiveLevelID].SetActive(true);

        levelUI.SetActive(true);
        
        currentLevelData = levels[currentActiveLevelID].GetComponent<LevelData>();
        player.transform.position = currentLevelData.startPosition.position;
        player.SetActive(true);

        ResetCollectedCoins();
        UpdateUI();
        levelSelectionUI.SetActive(false);
    }

    // Auto load next level when character clicks on level-door
    private void LoadNextLevel(TextMeshProUGUI informationMesh)
    {
        if (collectedCoins >= coinsToWin)
        {
            int nextLevelID = currentActiveLevelID + 1;
            if (levels.ContainsKey(nextLevelID))
            {
                // Deactivate old level
                levels[currentActiveLevelID].SetActive(false);

                // Activate new level
                levels[nextLevelID].SetActive(true);

                // Update current level data
                CompleteCurrentLevel();
                currentActiveLevelID = nextLevelID;
                currentLevelData = levels[currentActiveLevelID].GetComponent<LevelData>();

                // Put character on starting position
                player.transform.position = currentLevelData.startPosition.position;

                levelTimer = 0f;
                ResetCollectedCoins();
                PlayerHealth.Instance.ResetHealthBar();
                UpdateUI();
            }
            else if (!levels.ContainsKey(nextLevelID))
            {
                // Something like this if it is desired to auto return to first level 
                //int nextLevelID = (currentActiveLevelID + 1) % levelObjects.Count;
                CompleteCurrentLevel();
                levelTimer = 0f;
                ResetCollectedCoins();
                PlayerHealth.Instance.ResetHealthBar();
                UpdateUI();
            }
        }
        else
        {
            StartCoroutine(showInfomrationMessage(informationMesh));
        }
    }

    public void RetryCurrentLevel() 
    {
        //levels[currentActiveLevelID].SetActive(false);
        levelTimer = 0f;
        ResetCollectedCoins();
        PlayerHealth.Instance.ResetHealthBar();
        LoadLevel(currentActiveLevelID);
    }

    // Show the user that he cant enter next level yet
    private IEnumerator showInfomrationMessage(TextMeshProUGUI informationMesh)
    {
        informationMesh.gameObject.SetActive(true);
        yield return new WaitForSeconds(visibilityTimer);
        informationMesh.gameObject.SetActive(false);
    }

    private void CompleteCurrentLevel()
    {
        if (currentLevelData == null)
        {
            Debug.LogError("No current level data available!");
            return;
        }

        currentLevelData.CompleteLevel(levelTimer, collectedCoins);

        float completionTime = levelTimer;
        int stars = currentLevelData.GetStarRating();
        int coinsCollected = collectedCoins;

        // Update progress in manager
        ProgressManager.Instance.UpdateLevelProgress(
            currentActiveLevelID,
            true, // Markiere Level als abgeschlossen
            completionTime,
            stars,
            coinsCollected
        );
    }

    // Update the collected coins ui
    private void UpdateUI()
    {
        collectedCoinsTMP.text = (collectedCoins == 0 && totalCoinWorth == 0) ? 
            collectedCoinsTMP.text = " " : 
            collectedCoinsTMP.text = $"Coins: {collectedCoins}" + "\n" + $"Worth: {totalCoinWorth}";
    }

    // Update timer UI
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(levelTimer / 60);
        int seconds = Mathf.FloorToInt(levelTimer % 60);
        int milliseconds = Mathf.FloorToInt((levelTimer * 1000) % 1000);
        timerTMP.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    // Update coins TMP
    private void UpdateCollectedCoinsTMP(int coinValue)
    {
        collectedCoins += 1;
        totalCoinWorth += coinValue;
        UpdateUI();
    }

    // Reset coin count
    private void ResetCollectedCoins()
    {
        collectedCoins = 0;
        totalCoinWorth = 0;
    }
}
