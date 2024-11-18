using UnityEngine;

[System.Serializable]
public class LevelProgress
{
    public int levelID;
    public string levelName;
    public bool isCompleted;
    public float bestTime;
    public int acquiredStars;
    public int collectedCoins;

    // Constructor
    public LevelProgress(int id, string name)
    {
        levelID = id;
        levelName = name;
        isCompleted = false;
        bestTime = float.MaxValue; // Initialise with nothing
        acquiredStars = 0;
        collectedCoins = 0;
    }

    // Update progress
    public void UpdateProgress(bool completed, float time, int stars, int coins)
    {
        isCompleted = completed;
        bestTime = Mathf.Min(bestTime, time); 
        acquiredStars = Mathf.Max(acquiredStars, stars); 
        collectedCoins = Mathf.Max(collectedCoins, coins); 
    }
}
