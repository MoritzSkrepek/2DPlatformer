using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CharacterController characterController = collision.GetComponent<CharacterController>();
            characterController.UpdateCollectedCoins();
            Destroy(gameObject);
        }
    }
}
