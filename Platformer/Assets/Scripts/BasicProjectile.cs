using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicProjectile : MonoBehaviour
{
    [Header("Projectile settings")]
    [SerializeField] private float speed;
    [SerializeField] private float damage;
    [SerializeField] private float lifeTime;

    [Header("Collider")]
    [SerializeField] private CircleCollider2D circleCollider2D;

    protected Rigidbody2D rigidbody2D;
    
    protected virtual void Start()
    {
        StartCoroutine(EnableCollider()); // Enable box collider shortly after shooting so it doesnt hit player
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime); // Destroy after reaching lifetime end
    }

    protected virtual IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.05f);
        circleCollider2D.enabled = true;
    }

    public void Initialize(Vector2 direction)
    {
        rigidbody2D = gameObject.GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = direction * speed;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        // Make sure that the collision is boxCollider of enemy
        if (collision is BoxCollider2D && collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                OnHitEffect();
                Destroy(gameObject);
            }
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        LayerMask collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Ground") || collisionLayer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }

    protected abstract void OnHitEffect();
}
