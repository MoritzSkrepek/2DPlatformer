using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public TextMeshProUGUI collectedCoinsTMP;

    [Header("Collectables")]
    private int collectedCoins;

    // Update collected coins TMP
    public void UpdateCollectedCoins()
    {
        collectedCoins++;
        collectedCoinsTMP.text = "Coins: " + collectedCoins;
    }
}
