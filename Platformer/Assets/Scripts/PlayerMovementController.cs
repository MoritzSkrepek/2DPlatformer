using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;

// Script to controll the player movement 
// Keys: WASD, C, Space
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] public float moveSpeed;
    private float speedMultiplier = 1f;

    [Header("Jumping")]
    [SerializeField] public float jumpForce;
    [SerializeField] private int maxJumps;
    private float jumpBoostMultiplier = 1f;
    private int remainingJumps;

    [Header("Dashing")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashDuration;
    [SerializeField] private float doubleTapTime; 
    [SerializeField] private float dashCooldown;
    private bool isDashing = false;
    private float lastTapTime = 0f;
    private float dashDirection;
    private float lastHorizontalInput = 0f;
    private float lastDashTime = -Mathf.Infinity;

    [Header("Sliding")]
    [SerializeField] private float slideDuration;
    private bool isSliding = false;
    private float slideDirection;
    private float slideTimer;

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
    private Vector2 wallJumpPower = new Vector2(5f, 10f);

    [Header("Background")]
    [SerializeField] private LayerMask backgroundLayer;

    // Components
    private Animator animator;
    private Rigidbody2D rigidBody2D;

    private void Awake()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        SpeedBoostPotion.OnSpeedBoostCollected += StartSpeedBoost;
        JumpBoostPotion.OnJumpBoostCollected += StartJumpBoost;
    }

    // Unsubscribe from events when objects is not loaded anymore
    // This is needed so when the user quits to main menu and loads
    // into a level again potion events get referenced to the old
    // movement script
    private void OnDisable()
    {
        SpeedBoostPotion.OnSpeedBoostCollected -= StartSpeedBoost;
        JumpBoostPotion.OnJumpBoostCollected -= StartJumpBoost;
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
            rigidBody2D.velocity = new Vector2(horizontalMovement * moveSpeed * speedMultiplier, rigidBody2D.velocity.y);
            FlipSprite();
        }
        SetAnimator();
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
        if (remainingJumps > 0)
        {
            if (context.performed)
            {
                // Hold for full jump
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce * jumpBoostMultiplier);
                remainingJumps--;
                animator.SetTrigger("jump");
            }
            else if (context.canceled && rigidBody2D.velocity.y > 0)
            {
                // Short press for half jump
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce * jumpBoostMultiplier * 0.5f);
                remainingJumps--;
                animator.SetTrigger("jump");
            }
        }

        // Wallslide
        if (context.performed && wallJumpTimer > 0)
        {
            isWallJumping = true;
            // jump away from wall
            rigidBody2D.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); 
            wallJumpTimer = 0;
            animator.SetTrigger("jump");
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
        // While button is held perform slide
        if (context.started && isGrounded && !isSliding && horizontalMovement != 0)
        {
            isSliding = true;
            slideDirection = horizontalMovement;
            StartCoroutine(PerformSlide());
        }
        // If user lets go of butten cancel slide
        else if (context.canceled)
        {
            isSliding = false;
        }
    }

    // Start speed boost
    private void StartSpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        speedMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        speedMultiplier = 1f;
    }

    // Start jump boost
    private void StartJumpBoost(float multiplier, float duration)
    {
        StartCoroutine(JumpBoostCoroutine(multiplier, duration));
    }

    private IEnumerator JumpBoostCoroutine(float multiplier, float duration)
    {
        jumpBoostMultiplier = multiplier;
        yield return new WaitForSeconds(duration);
        jumpBoostMultiplier = 1f;
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

    private IEnumerator PerformSlide()
    {
        float initialSpeed = moveSpeed * speedMultiplier * 1.5f; // Start speed so the beginning of the slide is faster than normal movement
        float targetSpeed = moveSpeed * speedMultiplier * 0.3f;  // Target speed that has to be achieved 

        slideTimer = 0f; 
        isSliding = true;

        while (isSliding && slideTimer < slideDuration)
        {
            // Reduced velocity between player and initial speed
            float currentSlideSpeed = Mathf.Lerp(initialSpeed, targetSpeed, slideTimer / slideDuration);
            rigidBody2D.velocity = new Vector2(currentSlideSpeed * slideDirection, rigidBody2D.velocity.y);
            slideTimer += Time.deltaTime;
            yield return null;
        }

        // Nach dem Slide aufhören
        isSliding = false;
        slideTimer = 0f;
    }

    // Start the dash
    private void StartDash(float direction)
    {
        if (Time.time >= lastDashTime + dashCooldown)
        {
            isDashing = true;
            dashDirection = direction;
            lastDashTime = Time.time;
            rigidBody2D.velocity = new Vector2(dashDirection * speedMultiplier * dashSpeed, rigidBody2D.velocity.y);
            StartCoroutine(StopDashAfterDuration());
        }
    }

    // Stop the dash 
    private IEnumerator StopDashAfterDuration()
    {
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
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
            animator.ResetTrigger("jump");
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

    // Set new values each frame for animations
    private void SetAnimator()
    {
        animator.SetFloat("yVelocity", rigidBody2D.velocity.y);
        animator.SetFloat("magnitude", rigidBody2D.velocity.magnitude);
        animator.SetBool("isWallSliding", isWallSliding);
        animator.SetBool("isSliding", isSliding);
        animator.SetBool("isGrounded", isGrounded);
    }

    // Draws collider boxes
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
