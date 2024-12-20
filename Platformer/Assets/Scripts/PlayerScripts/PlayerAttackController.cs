using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerAttackController : MonoBehaviour
{
    public static PlayerAttackController Instance;

    [Header("Projectile Prefabs")]
    [SerializeField] private GameObject fireballProjectilePrefab;
    [SerializeField] private GameObject iceballProjectilePrefab;

    private float offsetY = 1f;

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

    public void ShootProjectile(Vector2 targetPosition, ProjectileType projectileType)
    {
        GameObject selectedProjectilePrefab = GetProjectilePrefab(projectileType);

        if (selectedProjectilePrefab == null) return;

        // Berechne die Richtung
        Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + offsetY);
        Vector2 direction = (targetPosition - spawnPosition).normalized;

        // Erzeuge das Projektil
        GameObject projectileObject = Instantiate(selectedProjectilePrefab, spawnPosition, Quaternion.identity);

        // Initialisiere das Projektil
        BaseProjectile projectile = projectileObject.GetComponent<BaseProjectile>();
        if (projectile != null)
        {
            projectile.Initialize(direction);
        }
    }

    private GameObject GetProjectilePrefab(ProjectileType projectileType)
    {
        switch (projectileType)
        {
            case ProjectileType.Fireball:
                return fireballProjectilePrefab;
            case ProjectileType.Iceball:
                return iceballProjectilePrefab;
            default:
                return null;
        }
    }
}
