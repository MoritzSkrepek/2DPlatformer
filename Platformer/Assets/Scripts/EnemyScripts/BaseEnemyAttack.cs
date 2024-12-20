using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyAttack : MonoBehaviour
{
    [Header("Attack settings")]
    [SerializeField] protected float attackDamage;

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerHealth player = collision.collider.GetComponent<PlayerHealth>();
        if (player != null)
        {
            AttackPlayer(player);
        }
    }

    protected abstract void AttackPlayer(PlayerHealth player);
}
