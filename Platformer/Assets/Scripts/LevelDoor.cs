using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    public static event Action<TextMeshProUGUI> OnLevelDoorClicked;
    public TextMeshProUGUI TextMeshProUGUI;
    // This is for future implementation for ui loading
    //private int level; 

    public void LevelDoorClicked()
    {
        OnLevelDoorClicked.Invoke(TextMeshProUGUI);
    }
}
