using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartItem : MonoBehaviour, IItem
{
    public static event Action<int> OnHeartCollected;

    [Header("Healing factor")]
    [SerializeField] private int healingFactor;

    public void Collect()
    {
        OnHeartCollected.Invoke(healingFactor);
        Destroy(gameObject);
    }
}
