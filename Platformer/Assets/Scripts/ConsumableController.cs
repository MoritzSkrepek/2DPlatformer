using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsumableController : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private float originalMoveSpeed;
    private float originalJumpForce;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        originalMoveSpeed = playerMovement.moveSpeed;
        originalJumpForce = playerMovement.jumpForce;
    }

    // Activate speed boost
    public void ActivateSpeedBoost(float boostedSpeed, float duration)
    {
        playerMovement.moveSpeed = boostedSpeed;
        StartCoroutine(ResetSpeedAfterDuration(duration));
    }

    // Activate jump boost
    public void ActivateJumpBoost(float boostedForce, float duration)
    {
        playerMovement.jumpForce = boostedForce;
        StartCoroutine(ResetJumpForceAfterDuration(duration));
    }

    private IEnumerator ResetJumpForceAfterDuration(float duration)
    {
        yield return new WaitForSeconds((int)duration);
        playerMovement.jumpForce = originalJumpForce;
    }

    private IEnumerator ResetSpeedAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerMovement.moveSpeed = originalMoveSpeed;
    }
}
