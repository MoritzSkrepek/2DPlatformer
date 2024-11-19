using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ProgressManager : MonoBehaviour
{
    public static ProgressManager Instance { get; private set; }

    private const string SaveKey = "LevelProgressData";
    private Dictionary<int, LevelProgress> levelProgressData = new Dictionary<int, LevelProgress>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved data
            LoadProgress();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LevelData[] levelDataArray = FindObjectsOfType<LevelData>();
        foreach (LevelData level in levelDataArray)
        {
            ProgressManager.Instance.RegisterLevel(level.levelID, level.levelName);
        }
    }

    // Register level progress
    public void RegisterLevel(int levelID, string levelName)
    {
        if (!levelProgressData.ContainsKey(levelID))
        {
            levelProgressData[levelID] = new LevelProgress(levelID, levelName);
        }
    }

    // Get progress from level by id
    public LevelProgress GetLevelProgress(int levelID)
    {
        if (levelProgressData.TryGetValue(levelID, out LevelProgress progress))
        {
            return progress;
        }
        Debug.LogWarning($"Level ID {levelID} not found in progress data.");
        return null;
    }

    // Update level progress
    public void UpdateLevelProgress(int levelID, bool completed, float time, int stars, int coins)
    {
        if (levelProgressData.TryGetValue(levelID, out LevelProgress progress))
        {
            progress.UpdateProgress(completed, time, stars, coins);
            SaveProgress();
        }
        else
        {
            Debug.LogError($"Level ID {levelID} not found! Cannot update progress.");
        }
    }

    // Save all progress in playerprefs as JSON
    public void SaveProgress()
    {
        string json = JsonUtility.ToJson(new ListWrapper<LevelProgress>(levelProgressData.Values.ToList()));
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log($"Progress saved: {json}");
    }

    // Load progress from playerprefs
    public void LoadProgress()
    {
        if (PlayerPrefs.HasKey(SaveKey))
        {
            string json = PlayerPrefs.GetString(SaveKey);
            List<LevelProgress> progressList = JsonUtility.FromJson<ListWrapper<LevelProgress>>(json).list;

            foreach (LevelProgress progress in progressList)
            {
                levelProgressData[progress.levelID] = progress;
            }
        }
    }

    // Get the id from the next incomplete level
    public int GetNextIncompleteLevelID()
    {
        foreach (var progress in levelProgressData.Values.OrderBy(l => l.levelID))
        {
            if (progress.isCompleted == false)
            {
                return progress.levelID;
            }
        }
        return -1; // All levels completed
    }
}

// Wrapper for JSON serialisation
[System.Serializable]
public class ListWrapper<T>
{
    public List<T> list;

    public ListWrapper(List<T> list)
    {
        this.list = list;
    }
}
