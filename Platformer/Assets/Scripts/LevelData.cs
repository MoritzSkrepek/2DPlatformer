using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    [Header("Level identification")]
    [SerializeField] public int levelID;
    [SerializeField] public string levelNamel;

    [Header("Level specific")]
    [SerializeField] public int coinsCount;
    [SerializeField] public bool hasSubLevel;
    private bool isCompleted = false;

    [Header("Time requirements for stars")]
    [SerializeField] public float timeForOneStar;
    [SerializeField] public float timeForTwoStars;
    [SerializeField] public float timeForThreeStars;

    [Header("Performance")]
    [SerializeField] public float bestTime = float.MaxValue;

    private int acquiredStars = 0;

    // Mark the level as completed
    public void CompleteLevel(float completionTime)
    {
        isCompleted = true;
        if (completionTime < bestTime)
        {
            bestTime = completionTime;
        }
        SetStarRatingUponLevelCompletion(completionTime);
    }

    private void SetStarRatingUponLevelCompletion(float completionTime)
    {
        if (completionTime < timeForThreeStars)
        {
            acquiredStars = 3;
        }
        else if (completionTime < timeForTwoStars)
        {
            acquiredStars = 2;
        }
        else if (completionTime < timeForOneStar)
        {
            acquiredStars = 1;
        }
    }

    public bool GetCompletionStateOfLevel(int id)
    {
        return isCompleted ? true : false;
    }
}
