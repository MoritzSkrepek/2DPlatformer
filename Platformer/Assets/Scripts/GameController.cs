using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Used for updates in the UI
public class GameController : MonoBehaviour
{
    public TextMeshProUGUI collectedCoinsTMP;

    [Header("Collectables")]
    private int collectedCoins;
    private int totalCoinWorth;

    private void Awake()
    {
        Coin.OnCoinCollected += UpdateCollectedCoinsTMP;
    }

    private void UpdateCollectedCoinsTMP(int coinValue)
    {
        collectedCoins += 1;
        totalCoinWorth += coinValue;
        collectedCoinsTMP.text = $"Coins: {collectedCoins}" + "\n" + $"Worth: {totalCoinWorth}";
    }
}
