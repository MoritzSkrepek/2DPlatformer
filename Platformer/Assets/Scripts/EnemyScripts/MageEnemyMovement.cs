using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageEnemyMovement : BaseEnemyMovement
{
    protected override void FollowPlayer()
    {
        // If character is not grounded don't check anything
        if (!isGrounded) return;

        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
        rigidBody.velocity = new Vector2(direction * moveSpeed, rigidBody.velocity.y);

        if (isFacingRight && rigidBody.velocity.x < 0f || !isFacingRight && rigidBody.velocity.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }

        // Raycasts
        RaycastHit2D groundInFront = Physics2D.Raycast(transform.position, new Vector2(direction, 0), 2f, groundLayer);
        Debug.DrawRay(transform.position, new Vector2(direction, 0) * 2f, Color.red);
        RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(direction, 0, 0), Vector2.down, 2f, groundLayer);
        Debug.DrawRay(transform.position + new Vector3(direction, 0, 0), Vector2.down * 2f, Color.blue);
        RaycastHit2D platformAbove = Physics2D.Raycast(transform.position, Vector2.up, 4f, groundLayer);
        Debug.DrawRay(transform.position, Vector2.up * 4f, Color.green);

        // No gap / obstacle / platform above in the path
        if ((gapAhead.collider == null && groundInFront.collider == null))
        {
            shouldJump = true;
        }
        // Gap/obstacle/Platform above / in front of enemy
        else if (gapAhead.collider != null || groundInFront.collider != null || platformAbove.collider != null)
        {
            shouldJump = false;
        }
    }
}
