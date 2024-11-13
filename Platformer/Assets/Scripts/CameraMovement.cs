using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Follow settings")]
    [SerializeField] private Vector3 offset;
    [SerializeField] private float speed;

    [Header("Player")]
    [SerializeField] private Transform player;

    void Update()
    {
        Vector3 disredPosition = player.position + offset;
        transform.position = Vector3.Lerp(transform.position, disredPosition, speed * Time.deltaTime);
    }
}
