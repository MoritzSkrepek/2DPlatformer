using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile damage")]
    [SerializeField] private float projectileDamage;

    [Header("Projectile lifespan")]
    [SerializeField] private float projectileLifeSpan;

    [Header("Collision layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [Header("Collider")]
    [SerializeField] private CircleCollider2D circleCollider;

    private void Start()
    {
        StartCoroutine(EnableCollider());
        Destroy(gameObject, projectileLifeSpan);
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.05f);
        circleCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is BoxCollider2D)
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(projectileDamage);
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision!");
        LayerMask collisionLayer = collision.gameObject.layer;
        if (collisionLayer == LayerMask.NameToLayer("Ground") || collisionLayer == LayerMask.NameToLayer("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
