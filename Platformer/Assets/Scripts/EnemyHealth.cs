using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health stats")]
    [SerializeField] private float maxHealth;
    private float currentHealth;

    [Header("Enemy")]
    [SerializeField] private float hitVisibilityDurtation;

    // Enemy healtbar
    private Slider healthBar;

    // Enemy sprite
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        Transform canvasTransform = gameObject.transform.Find("Canvas");
        Transform sliderTransform = canvasTransform.Find("Slider");
        healthBar = sliderTransform.GetComponent<Slider>();
        healthBar.maxValue = maxHealth;
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.value = maxHealth - currentHealth;
        StartCoroutine(Flash());
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitVisibilityDurtation);
        spriteRenderer.color = Color.white;
    }
}
