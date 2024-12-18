using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FireballProjectile : BasicProjectile
{
    [Header("Fireball settings")]
    [SerializeField] private float explosionRadius;

    // HashSet to ensure a direct hit only takes damage once
    private HashSet<EnemyHealth> alreadyHitEnemies = new HashSet<EnemyHealth>();

    protected override void OnEnemyEffect(EnemyHealth enemy)
    {
        enemy.TakeDamage(damage);
        alreadyHitEnemies.Add(enemy);
        Explode();
    }

    protected override void OnEnvirenmentHitEffect()
    {
        Explode();
    }

    // Explode projectile and damage all enemies in overlap
    // TODO: Make it so explosions dont go through solid walls | floors
    private void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && hit is BoxCollider2D) // This will be changed in the future
            {
                EnemyHealth enemy = hit.GetComponent<EnemyHealth>();
                if (enemy != null && !alreadyHitEnemies.Contains(enemy))
                {
                    enemy.TakeDamage(damage);
                    alreadyHitEnemies.Add(enemy);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
