using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance;

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
    private float remaningDamageAmount;

    [Header("Player")]
    [SerializeField] private float hitVisibilityDurtation;
    private GameObject character;
    private SpriteRenderer characterSpriteRenderer;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        currentHealth = maxHealth;
        currentHeartCount = maxHeartCount;
        heartCountTMP.text = maxHeartCount.ToString();
        character = GameObject.Find("Character");
        characterSpriteRenderer = character.GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyMovementController enemy = collision.collider.GetComponent<EnemyMovementController>();
        // In the future this will be a switch for all sorts of collisions
        // e.g.: different enemies, traps, etc...
        if (enemy != null)
        {
            StartCoroutine(Flash());
            TakeDamage(enemy.attackDamage);
        }
    }

    private IEnumerator Flash()
    {
        characterSpriteRenderer.color = Color.red;
        yield return new WaitForSeconds(hitVisibilityDurtation);
        characterSpriteRenderer.color = Color.white;
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
                GameStateController.Instance.TriggerGameOver();
                InputController.Instance.DisableInputActions();
            }
        }
        healthBarSlider.value = maxHealth - currentHealth;
    }

    private void UpdateUI()
    {
        heartCountTMP.text = currentHeartCount.ToString();
    }

    public void ResetHealthBar()
    {
        currentHealth = maxHealth;
        currentHeartCount = maxHeartCount;
        heartCountTMP.text = maxHeartCount.ToString();
        healthBarSlider.value = healthBarSlider.minValue;
    }
}
