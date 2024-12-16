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
    [SerializeField] private GameObject basicProjectile;
    [SerializeField] private GameObject iceballProjectile;
    [SerializeField] private GameObject fireballProjectile;
    [SerializeField] private float projectileMovementSpeed;
    private GameObject activeProjectile;

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

    public void ShootProjectile(Vector2 targetPosition /*ProjectileType type = ProjectileType.basic*/)
    {
        /* This code is for future impl. when there are different attack types 
         
        activeProjectile = null;
        switch (type)
        {
            case ProjectileType.basic:
                projectile = basicProjectile; break;
            case ProjectileType.iceball:
                projectile = iceballProjectile; break;
            case ProjectileType.fireball:
                projectile = fireballProjectile; break; 
            default:
                projectile.SetActive(false); break;
        }
        */

        // Berechne die Richtung von der Spawn-Position zum Zielpunkt
        Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + offsetY);
        Vector2 direction = (targetPosition - spawnPosition).normalized;

        // Erzeuge das Projektil an der Spawn-Position
        GameObject projectile = Instantiate(basicProjectile, spawnPosition, Quaternion.identity);

        // Setze die Geschwindigkeit des Projektils, um es in Richtung des Zielpunkts zu bewegen
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileMovementSpeed;

    }
}

public enum ProjectileType
{
    basic,
    fireball,
    iceball
};
