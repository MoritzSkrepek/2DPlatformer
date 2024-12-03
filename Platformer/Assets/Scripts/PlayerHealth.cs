using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Healthbar UI")]
    [SerializeField] private TextMeshProUGUI heartCountTMP;
    [SerializeField] private Slider healthBarSlider;

    [Header("Game over UI")]
    [SerializeField] private GameObject gameOverUI;

    [Header("Health")]
    [SerializeField] private float maxHealth;
    private float currentHealth;
    [SerializeField] private int maxHeartCount;
    private int currentHeartCount;  
    private float damageAmount;

    private void Start()
    {
        currentHealth = maxHealth;
        currentHeartCount = maxHeartCount;
        heartCountTMP.text = maxHeartCount.ToString();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Enemy enemy = collision.collider.GetComponent<Enemy>();
        if (enemy != null)
        {
            TakeDamage(enemy.attackDamage);
        }
    }

    private void TakeDamage(float damage) 
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            if (currentHeartCount > 0)
            {
                currentHeartCount--;
                currentHealth = maxHealth;
                UpdateUI();
            }
            else
            {
                currentHealth = 0;
                Debug.Log("Game over");
            }
        }
        healthBarSlider.value = maxHealth - currentHealth;
    }

    private void UpdateUI()
    {
        heartCountTMP.text = currentHeartCount.ToString();  
    }

    public void ResetHealth()
    {
        healthBarSlider.value = maxHealth;
    }

    public void ResetHeartCoint()
    {
        heartCountTMP.text = maxHeartCount.ToString();
    }
}
