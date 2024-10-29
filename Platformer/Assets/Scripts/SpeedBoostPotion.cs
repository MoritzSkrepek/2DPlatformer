using System.Collections;
using UnityEngine;

public class SpeedBoostPotion : MonoBehaviour
{
    [SerializeField] private float boostedSpeed = 7.5f;
    [SerializeField] private float duration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterController characterController = collision.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.ActivateSpeedBoost(boostedSpeed, duration);
            }
            Destroy(gameObject); 
        }
    }
}
