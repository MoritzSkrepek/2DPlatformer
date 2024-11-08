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

    [Header("Player")]
    [SerializeField] private GameObject player;

    [Header("Levels")]
    [SerializeField] private List<GameObject> levels;
    private int currentLevelIndex = 0;

    [Header("Collectables")]
    [SerializeField] private int coinsToWin;

    [Header("Message visibility duration")]
    [SerializeField] private float duration;
    
    // Level and coins in level
    private GameObject[] coins;

    // collected coins / coinworth
    private int collectedCoins;
    private int totalCoinWorth;

    private void Awake()
    {
        Coin.OnCoinCollected += UpdateCollectedCoinsTMP;
        LevelDoor.OnLevelDoorClicked += LoadNextLevel; 
    }

    private void UpdateCollectedCoinsTMP(int coinValue)
    {
        Debug.Log("UpdateCollectedCoinsTMP aufgerufen");
        collectedCoins += 1;
        totalCoinWorth += coinValue;
        CheckCollectedCoins();
        UpdateUI();  
    }

    private void CheckCollectedCoins()
    {
        // TODO: a minimum % of coins in level have to be collected to move to next level
    }

    private void ResetCollectedCoins()
    {
        collectedCoins = 0;
        totalCoinWorth = 0;
    }

    private void LoadNextLevel(TextMeshProUGUI informationMesh)
    {
        if (collectedCoins >= coinsToWin)
        {
            int nextLevelIndex = (currentLevelIndex == levels.Count - 1) ? 0 : currentLevelIndex + 1;
            levels[currentLevelIndex].gameObject.SetActive(false);
            levels[nextLevelIndex].gameObject.SetActive(true);
            player.transform.position = new Vector3(0, 0, 0);
            currentLevelIndex = nextLevelIndex;
            ResetCollectedCoins();
            UpdateUI();
        }
        else
        {
            StartCoroutine(showInfomrationMessage(informationMesh));
        }
    }

    private IEnumerator showInfomrationMessage(TextMeshProUGUI informationMesh)
    {
        informationMesh.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        informationMesh.gameObject.SetActive(false);
    }

    private void UpdateUI()
    {
        collectedCoinsTMP.text = (collectedCoins == 0 && totalCoinWorth == 0) ? 
            collectedCoinsTMP.text = " " 
            : 
            collectedCoinsTMP.text = $"Coins: {collectedCoins}" + "\n" + $"Worth: {totalCoinWorth}";
    }
}
