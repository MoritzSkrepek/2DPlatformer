using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpBoostPotion : MonoBehaviour
{
    [SerializeField] float boostedJumpForce = 7.5f;
    [SerializeField] float duration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ConsumableController consumableController = collision.GetComponent<ConsumableController>();
            if (consumableController != null) 
            {
                consumableController.ActivateJumpBoost(boostedJumpForce, duration);
                Destroy(gameObject);
            }
        }
    }
}
