using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [Header("Level identification")]
    [SerializeField] public int levelID;
    [SerializeField] public string levelName;

    [Header("Level specific")]
    [SerializeField] public bool hasSubLevel;
    [SerializeField] public Transform startPosition;
    private bool isCompleted = false;
    private int coinsCount;
    public int collectedCoins;

    [Header("Time requirements for stars")]
    [SerializeField] public float timeForOneStar;
    [SerializeField] public float timeForTwoStars;
    [SerializeField] public float timeForThreeStars;

    [Header("Performance")]
    [SerializeField] public float bestTime = float.MaxValue;

    private int acquiredStars;

    private void Awake()
    {
        startPosition = transform.Find("Start Position");
        Transform coinsContainer = transform.Find("Coins");
        coinsCount = coinsContainer.childCount;
    }

    // Mark the level as completed
    public void CompleteLevel(float completionTime, int collectedCoins)
    {
        isCompleted = true;
        this.collectedCoins = collectedCoins;
        if (completionTime < bestTime)
        {
            bestTime = completionTime;
        }
        SetStarRatingUponLevelCompletion(completionTime);
    }

    // Set acquired stars based on completion time
    private void SetStarRatingUponLevelCompletion(float completionTime)
    {
        if (completionTime < timeForThreeStars)
        {
            acquiredStars = 3;
            return;
        }
        else if (completionTime < timeForTwoStars)
        {
            acquiredStars = 2;
            return;
        }
        else if (completionTime < timeForOneStar)
        {
            acquiredStars = 1;
            return;
        }
    }

    public int GetStarRating()
    {
        return acquiredStars;
    }
}
