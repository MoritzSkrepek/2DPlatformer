using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;

    // Player itself
    private Transform player;

    private void Start()
    {
        if (player == null)
        {
            GameObject character = GameObject.FindGameObjectWithTag("Player");
            if (character != null)
            {
                player = character.transform;
            }
            else
            {
                Debug.LogWarning("No character found with the tag 'Player'");
            }
        }
    }

    void Update()
    {
        Vector3 disredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, disredPosition, speed * Time.deltaTime);
    }
}
