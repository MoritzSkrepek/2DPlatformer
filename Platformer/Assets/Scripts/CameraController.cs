using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; 
    public Vector2 followOffset = new Vector2(5f, 5f); // Offset für den Bereich, in dem die Kamera folgt
    public float smoothSpeed = 5f; // Geschwindigkeit der Kamerabewegung

    private void Start()
    {
        if (target == null)
        {
            GameObject character = GameObject.FindGameObjectWithTag("Player");
            if (character != null)
            {
                target = character.transform;
            }
            else
            {
                Debug.LogWarning("No character found with the tag 'Player'");
            }
        }
    }

    void Update()
    {
        if (target == null)
        {
            return;
        }

        Vector3 offsetPosition = transform.position - target.position;

        if (Mathf.Abs(offsetPosition.x) > followOffset.x || Mathf.Abs(offsetPosition.y) > followOffset.y)
        {
            Vector3 targetPosition = new Vector3(
                target.position.x + (offsetPosition.x > 0 ? followOffset.x : -followOffset.x),
                target.position.y + (offsetPosition.y > 0 ? followOffset.y : -followOffset.y),
                transform.position.z
            );

            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }
}
