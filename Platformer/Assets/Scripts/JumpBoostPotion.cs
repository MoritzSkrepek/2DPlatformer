using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostPotion : MonoBehaviour, IItem
{
    public static event Action<float, float> OnJumpBoostCollected;
    [SerializeField] private float jumpBoostMultiplier;
    [SerializeField] private float jumpBoostDuration;

    public void Collect()
    {
        OnJumpBoostCollected.Invoke(jumpBoostMultiplier, jumpBoostDuration);
        Destroy(gameObject);
    }
}
