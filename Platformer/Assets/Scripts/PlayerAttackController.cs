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

        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
        Vector2 spawnPosition = new Vector2(transform.position.x, transform.position.y + offsetY);
        GameObject projectile = Instantiate(basicProjectile, spawnPosition, Quaternion.identity);
        projectile.GetComponent<Rigidbody2D>().velocity = direction * projectileMovementSpeed;
    }
}

public enum ProjectileType
{
    basic,
    fireball,
    iceball
};
