using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour, IItem
{
    public static event Action<int> OnCoinCollected;
    [SerializeField] private int coinValue;
    public void Collect()
    {
        OnCoinCollected.Invoke(coinValue);
        Destroy(gameObject);
    }
}
