using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseProjectile : MonoBehaviour
{
    [Header("Projectile settings")]
    [SerializeField] private float speed;
    [SerializeField] public float damage;
    [SerializeField] private float lifeTime;
    [SerializeField] public LayerMask groundLayerMask;
    [SerializeField] public LayerMask wallLayerMask;

    [Header("Collider")]
    [SerializeField] public CircleCollider2D circleCollider2D;

    protected Rigidbody2D rigidBody;
    
    protected virtual void Start()
    {
        StartCoroutine(EnableCollider()); // Enable box collider shortly after shooting so it doesnt hit player
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // Destroy after reaching lifetime end
    }

    // Function to enable the attached CircleCollider2D of Projectile that is not a trigger
    protected virtual IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.05f);
        circleCollider2D.enabled = true;
    }

    // Function to Initialize the projectile itself and give it a velocity to given direction
    // -> This function is used to give the bullet its specified speed
    public void Initialize(Vector2 direction)
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        rigidBody.velocity = direction * speed;
    }

    // Function to check if there is a wall / floor / obstacle between the explosion
    // position of projectile and target enemy in overlay circle
    // -> This function is used for projectiles that have some sort of area of effect
    public bool IsObstacleInBetween(Vector2 origin, Vector2 target)
    {
        RaycastHit2D groundHit = Physics2D.Linecast(origin, target, groundLayerMask);
        RaycastHit2D wallHit = Physics2D.Linecast(origin, target, wallLayerMask);
        return (groundHit.collider != null || wallHit.collider != null) ? true : false;
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
