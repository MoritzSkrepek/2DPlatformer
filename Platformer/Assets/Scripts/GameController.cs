using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Used for updates in the UI
public class GameController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI collectedCoinsTMP;
    [SerializeField] private TMP_InputField levelInputField;
    [SerializeField] private TextMeshProUGUI timerTMP;
    [SerializeField] private GameObject levelSelectionUI;

    [Header("Player")]
    [SerializeField] private GameObject player;
    private Transform levelStartPosition;

    [Header("Criterias to enter next level")]
    [SerializeField] private int coinsToWin;

    [Header("Message visibility duration")]
    [SerializeField] private float duration;

    private Dictionary<int, GameObject> levels = new Dictionary<int, GameObject>();
    private int currentActiveLevelID;
    private LevelData currentLevelData;

    // collected coins / coinworth
    private int collectedCoins;
    private int totalCoinWorth;

    // level timer
    private float levelTimer;

    private void Start()
    {
        // Get levels in scene
        LevelData[] levelDataArray = Resources.FindObjectsOfTypeAll<LevelData>();

        // Map levels to dict
        foreach (LevelData levelData in levelDataArray)
        {
            levels[levelData.levelID] = levelData.gameObject;
        }

        Coin.OnCoinCollected += UpdateCollectedCoinsTMP;
        LevelDoor.OnLevelDoorClicked += LoadNextLevel;
    }

    private void Update()
    {
        if (currentActiveLevelID != 0)
        {
            levelTimer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // Load specific level by id
    public void LoadLevel(int levelID)
    {
        currentActiveLevelID = levelID;
        if (!levels.ContainsKey(currentActiveLevelID))
        {
            Debug.LogError("[ERROR] Level does not exist");
            return;
        }
        if (currentActiveLevelID != 0)
        {
            levels[currentActiveLevelID].SetActive(false);
        }

        levels[currentActiveLevelID].SetActive(true);

        //player.transform.position = new Vector3(0, 0, 0);
        currentLevelData = levels[currentActiveLevelID].GetComponent<LevelData>();
        player.transform.position = currentLevelData.startPosition.position;
        player.SetActive(true);

        ResetCollectedCoins();
        UpdateUI();
        levelSelectionUI.SetActive(false);
    }

    // Auto load next level when player clicks on level-door
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
                currentLevelData.CompleteLevel(levelTimer, collectedCoins);
                currentActiveLevelID = nextLevelID;
                currentLevelData = levels[currentActiveLevelID].GetComponent<LevelData>();

                // Put player on starting position
                player.transform.position = currentLevelData.startPosition.position;

                levelTimer = 0f;
                ResetCollectedCoins();
                UpdateUI();
            }
            else if (!levels.ContainsKey(nextLevelID))
            {
                // Something like this if it is desired to auto return to first level 
                //int nextLevelID = (currentActiveLevelID + 1) % levelObjects.Count;
                currentLevelData.CompleteLevel(levelTimer, collectedCoins);
                levelTimer = 0f;
                ResetCollectedCoins();
                UpdateUI();
            }
        }
        else
        {
            StartCoroutine(showInfomrationMessage(informationMesh));
        }
    }

    // Show the user that he cant enter next level yet
    private IEnumerator showInfomrationMessage(TextMeshProUGUI informationMesh)
    {
        informationMesh.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        informationMesh.gameObject.SetActive(false);
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
