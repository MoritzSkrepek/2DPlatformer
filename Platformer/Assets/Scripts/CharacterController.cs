using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;

public class CharacterController : MonoBehaviour
{
    
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    private float originalMoveSpeed;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float doubleTapTime; // Zeitfenster für Doppelklick
    [SerializeField] private float dashCooldown;
    private bool isDashing = false;
    private float lastTapTime = 0f;
    private float dashDirection;
    private float lastHorizontalInput = 0f;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Sliding")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideDuration;
    private bool isSliding = false;
    private float slideDirection;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    private float originalJumpForce;
    [SerializeField] private int maxJumps;
    private int remainingJumps;

    [Header("ProcessGravity")]
    [SerializeField] private float baseGravity;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float fallSpeedMultiplier;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private bool isFacingRight = true;
    private bool isGrounded;
    private float horizontalMovement;

    [Header("Wall Check")]
    [SerializeField] private Transform wallCheckPos;
    [SerializeField] private Vector2 wallCheckSize;
    [SerializeField] private LayerMask wallLayer;

    [Header("Wall Movement")]
    [SerializeField] private float wallSlideSpeed;
    [SerializeField] private float wallSlideDelay;
    private bool isWallSliding;
    private bool isWallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.5f;
    private float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2(5f, 10f);

    [Header("Collectables")]
    private int collectedCoins = 0;
    
    private Animator animator;
    private Rigidbody2D rigidBody2D;
    public TextMeshProUGUI collectedCoinsTMP;

    private void Awake()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = gameObject.GetComponent<Animator>();
        originalMoveSpeed = moveSpeed;
        originalJumpForce = jumpForce;
    }

    void Update()
    {      
        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        ProcessDash();
        if (!isWallJumping && !isDashing)
        {
            rigidBody2D.velocity = new Vector2(horizontalMovement * moveSpeed, rigidBody2D.velocity.y);
            FlipSprite();
        }
    }

    // WASD for movement
    public void Move(InputAction.CallbackContext context)
    {
        float currentHorizontalInput = context.ReadValue<Vector2>().x;

        // If player is moving and difference between current and horizontal movmenet is greater than a very small number
        // check for double action (AA / DD) and start dash
        if (currentHorizontalInput != 0 && Mathf.Abs(currentHorizontalInput - horizontalMovement) > Mathf.Epsilon)
        {
            if (Time.time - lastTapTime < doubleTapTime && lastHorizontalInput == currentHorizontalInput && !isDashing)
            {
                StartDash(currentHorizontalInput);
            }
            lastTapTime = Time.time;
            lastHorizontalInput = currentHorizontalInput;
        }

        horizontalMovement = currentHorizontalInput;
    }

    // Space vor jumping
    public void Jump(InputAction.CallbackContext context)
    {
        // Normal jump
        // -1, because first jump doesnt get registered
        if (remainingJumps - 1 > 0)
        {
            if (context.performed)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
                remainingJumps--;
            }
        }

        // Wallslide
        if (context.performed && wallJumpTimer > 0)
        {
            isWallJumping = true;
            rigidBody2D.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); // jump away from wall
            wallJumpTimer = 0;
            if (transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }
    }

    // C for sliding
    public void Slide(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Sliding");
        }
    }

    // Process gravity 
    private void ProcessGravity()
    {
        if (rigidBody2D.velocity.y < 0)
        {
            rigidBody2D.gravityScale = baseGravity * fallSpeedMultiplier; // faster falling
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Max(rigidBody2D.velocity.y, -maxFallSpeed)); // cap fallrate so it doesnt go to infinite
        }
        else
        {
            rigidBody2D.gravityScale = baseGravity;
        }
    }

    // Process dash
    private void ProcessDash()
    {
        // If player dashes and the passed time is greater than dashtime + cooldown
        if (isDashing && Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = false;
        }
    }

    // Wallslide process
    private void ProcessWallSlide()
    {
        if (!isGrounded && WallCheck() && horizontalMovement != 0)
        {
            isWallSliding = true;
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Max(rigidBody2D.velocity.y, -wallSlideSpeed)); // Cap auf Fallrate setzen
        }
        else
        {
            isWallSliding = false;
        }
    }

    // Walljump process
    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;
            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    // Cancel state of walljump
    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void StartDash(float direction)
    {
        if (Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            dashDirection = direction;
            lastDashTime = Time.time;
            rigidBody2D.velocity = new Vector2(dashDirection * dashSpeed, rigidBody2D.velocity.y);
            StartCoroutine(StopDashAfterDuration());
        }
    }

    private IEnumerator StopDashAfterDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
    }

    // Activate speed boost
    public void ActivateSpeedBoost(float boostedSpeed, float duration)
    {
        moveSpeed = boostedSpeed;
        StartCoroutine(ResetSpeedAfterDuration(duration));
    }
    
    // Activate jump boost
    public void ActivateJumpBoost(float boostedForce, float duration)
    {
        jumpForce = boostedForce;
        StartCoroutine(ResetJumpForceAfterDuration(duration));
    }

    // Functions to specifiy time for potions to subside
    private IEnumerator ResetJumpForceAfterDuration(float duration)
    {
        yield return new WaitForSeconds((int)duration);
        jumpForce = originalJumpForce;
    }

    private IEnumerator ResetSpeedAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    // Update collected coins TMP
    public void UpdateCollectedCoins()
    {
        collectedCoins++;
        collectedCoinsTMP.text = "Coins: " + collectedCoins;
    }

    // Flips sprite for Movement
    private void FlipSprite()
    {
        if (isFacingRight && horizontalMovement < 0f || !isFacingRight && horizontalMovement > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    // Checks if collider box is touching ground layer
    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            remainingJumps = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    // Checks if collider box is touching wall layer
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    // Draws collider boxes
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
