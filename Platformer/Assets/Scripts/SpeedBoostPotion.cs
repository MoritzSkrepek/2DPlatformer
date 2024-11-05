using System;
using System.Collections;
using UnityEngine;

public class SpeedBoostPotion : MonoBehaviour, IItem
{
    public static event Action<float, float> OnSpeedBoostCollected;
    [SerializeField] private float speedMultiplier;
    [SerializeField] private float speedBoostDuration;

    public void Collect()
    {
        OnSpeedBoostCollected.Invoke(speedMultiplier, speedBoostDuration);
        Destroy(gameObject);
    }
}
