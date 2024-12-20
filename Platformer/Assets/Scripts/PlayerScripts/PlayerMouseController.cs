using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Script to controll the players mouse interactions
// Keys: right-/ left click
public class PlayerMouseController : MonoBehaviour
{
    [Header("Interaction layer")]
    [SerializeField] private LayerMask backgroundLayer;

    [Header("Time between attacks")]
    [SerializeField] private float attackTimer;
    private float lastAttackTime = -Mathf.Infinity;

    private List<ProjectileType> projectileTypes = new List<ProjectileType> { ProjectileType.Fireball, ProjectileType.Iceball };
    private ProjectileType currentActiveProjectileType = ProjectileType.Fireball;
    private int currentProjectileIndex = 0;

    // Left click for attacking
    public void LeftClick(InputAction.CallbackContext context)
    {
        if (context.performed && CanAttack())
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
            PlayerAttackController.Instance.ShootProjectile(worldPoint, currentActiveProjectileType);
            lastAttackTime = Time.time;
        }
    }

    private bool CanAttack()
    {
        return (Time.time >= lastAttackTime + attackTimer) ? true : false;
    }

    // Right click for world interactions
    // (such as destroying crates, talking with nps, entering the next level, ...)
    public void RightClick(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, backgroundLayer);
            
            // Complete current level and enter next level
            if (hit.collider != null)
            {
                switch (hit.collider.tag)
                {
                    case "NextLevelDoor":
                        LevelDoor levelDoor = hit.collider.GetComponent<LevelDoor>();
                        levelDoor.LevelDoorClicked();
                        break;

                    // Default case: no hit                
                    default:
                        break;
                }
            }
        }
    }

    // Scroll through attack types
    public void Scroll(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            float scrollValue = context.ReadValue<float>();

            if (scrollValue > 0)
            {
                // Upwards scroll
                currentProjectileIndex = (currentProjectileIndex + 1) % projectileTypes.Count;
            }
            else if (scrollValue < 0)
            {
                // Downwards scroll
                currentProjectileIndex = (currentProjectileIndex - 1 + projectileTypes.Count) % projectileTypes.Count;
            }
            currentActiveProjectileType = projectileTypes[currentProjectileIndex];
            Debug.Log($"Current selected speel: {currentActiveProjectileType.ToString()}");
        }
    }
}
