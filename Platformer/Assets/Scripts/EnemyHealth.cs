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

    [Header("Loot Table")]
    public List<LootItem> lootTable = new List<LootItem>();

    // Enemy healtbar
    private Slider healthBar;

    // Enemy sprite
    private SpriteRenderer spriteRenderer;

    private void Start()
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
            DropItemOnDefeat();
            Destroy(gameObject);
        }
    }

    private IEnumerator Flash()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitVisibilityDurtation);
        spriteRenderer.color = Color.white;
    }

    public void DropItemOnDefeat()
    {
        foreach (LootItem lootItem in lootTable)
        {
            if (Random.Range(0f, 100f) <= lootItem.dropChance)
            {
                Instantiate(lootItem.itemPrefab, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + 0.5f), Quaternion.identity);
                break; // Ensure only one item drops 
            }
        }
    }
}
