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

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Levels")]
    [SerializeField] private List<GameObject> levelObjects;
    private Dictionary<int, GameObject> levels = new Dictionary<int, GameObject>();
    private int currentActiveLevelID;
    private LevelData currentLevelData;

    [Header("Criterias to enter next level")]
    [SerializeField] private int coinsToWin;

    [Header("Message visibility duration")]
    [SerializeField] private float duration;

    // collected coins / coinworth
    private int collectedCoins;
    private int totalCoinWorth;

    // level timer
    private float levelTimer;

    private void Awake()
    {
        foreach (var levelObject in levelObjects)
        {
            LevelData levelData = levelObject.GetComponent<LevelData>();
            if (levelData)
            {
                levels[levelData.levelID] = levelObject;
                Debug.Log($"Level Name: {levelData.name} Level ID {levelData.levelID}");
            }
        }
        Coin.OnCoinCollected += UpdateCollectedCoinsTMP;
        LevelDoor.OnLevelDoorClicked += LoadNextLevel;
        PlayerMouseController.OnLevelCompleted += CompleteCurrentLevel;
    }

    private void Update()
    {
        if (currentActiveLevelID != 0)
        {
            levelTimer += Time.deltaTime;
            UpdateTimerUI();
        }
    }

    // Update timer UI
    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(levelTimer / 60);
        int seconds = Mathf.FloorToInt(levelTimer % 60);
        timerTMP.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Format "MM:SS"
    }

    // Complete the current level
    private void CompleteCurrentLevel(float completionTime)
    {
        if (levels.ContainsKey(currentActiveLevelID))
        {
            currentLevelData = levels[currentActiveLevelID].GetComponent<LevelData>();
            currentLevelData.CompleteLevel(completionTime);
        }
        else
        {
            Debug.LogError($"Level with ID {currentActiveLevelID} does not exist in the dictionary!");
        }
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

    // Load specific level by id
    public void LoadLevel(/*int levelID*/)
    {
        int levelID = int.Parse(levelInputField.text);
        Debug.Log(levelID);
        if (!levels.ContainsKey(levelID))
        {
            Debug.LogError("[ERROR] Level does not exist");
            return;
        }
        if (currentActiveLevelID != 0)
        {
            levels[currentActiveLevelID].SetActive(false);
        }

        levels[levelID].SetActive(true);
        player.transform.position = new Vector3(0, 0, 0);
        currentActiveLevelID = levelID;

        ResetCollectedCoins();
        UpdateUI();
    }

    // Auto load next level when player clicks on level-door
    private void LoadNextLevel(TextMeshProUGUI informationMesh)
    {
        if (collectedCoins >= coinsToWin)
        {
            int nextLevelIndex = (currentActiveLevelID == levelObjects.Count - 1) ? 1 : currentActiveLevelID + 1;
            levelObjects[currentActiveLevelID].gameObject.SetActive(false);
            levelObjects[nextLevelIndex].gameObject.SetActive(true);
            player.transform.position = new Vector3(0, 0, 0);
            currentActiveLevelID = nextLevelIndex;
            ResetCollectedCoins();
            UpdateUI();
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
}
