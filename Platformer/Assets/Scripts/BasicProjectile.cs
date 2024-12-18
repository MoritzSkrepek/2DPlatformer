using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BasicProjectile : MonoBehaviour
{
    [Header("Projectile settings")]
    [SerializeField] private float speed;
    [SerializeField] public float damage;
    [SerializeField] private float lifeTime;

    [Header("Collider")]
    [SerializeField] public CircleCollider2D circleCollider2D;

    protected Rigidbody2D rigidBody;
    
    protected virtual void Start()
    {
        StartCoroutine(EnableCollider()); // Enable box collider shortly after shooting so it doesnt hit player
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // Destroy after reaching lifetime end
    }

    protected virtual IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.05f);
        circleCollider2D.enabled = true;
    }

    public void Initialize(Vector2 direction)
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;
    }

    // Collision with enemies
    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is BoxCollider2D && collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                OnEnemyEffect(enemyHealth);
                Destroy(gameObject);
            }
        }
    }

    // Collision with envirenment
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        LayerMask collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Ground") || collisionLayer == LayerMask.NameToLayer("Wall"))
        {
            OnEnvirenmentHitEffect();
            Destroy(gameObject);
        }
    }

    // Abstract functions that is overwritten in sub classes for different behaviour
    protected abstract void OnEnemyEffect(EnemyHealth enemy);

    protected abstract void OnEnvirenmentHitEffect();
}
