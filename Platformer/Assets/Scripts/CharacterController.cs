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
        if (!isWallJumping)
        {
            rigidBody2D.velocity = new Vector2(horizontalMovement * moveSpeed, rigidBody2D.velocity.y);
            FlipSprite();
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        // -1, Weil der erste Jump nicht registriert wird (Player ist in dem Moment noch grounded)
        if (remainingJumps - 1 > 0)
        {
            if (context.performed)
            {
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
                remainingJumps--;
            }
        }

        if (context.performed && wallJumpTimer > 0)
        {
            isWallJumping = true;
            rigidBody2D.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y); // Von Wand wegspringen
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

    private void ProcessGravity()
    {
        if (rigidBody2D.velocity.y < 0)
        {
            rigidBody2D.gravityScale = baseGravity * fallSpeedMultiplier; // Schnelleres fallen
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, Mathf.Max(rigidBody2D.velocity.y, -maxFallSpeed)); // Cap auf Fallrate setzen
        }
        else
        {
            rigidBody2D.gravityScale = baseGravity;
        }
    }

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

    private void CancelWallJump()
    {
        isWallJumping = false;
    }


    // Methos to Activate Boosts
    public void ActivateSpeedBoost(float boostedSpeed, float duration)
    {
        moveSpeed = boostedSpeed;
        StartCoroutine(ResetSpeedAfterDuration(duration));
    }

    private IEnumerator ResetSpeedAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        moveSpeed = originalMoveSpeed;
    }

    public void ActivateJumpBoost(float boostedForce, float duration)
    {
        jumpForce = boostedForce;
        StartCoroutine(ResetJumpForceAfterDuration(duration));
    }

    private IEnumerator ResetJumpForceAfterDuration(float duration)
    {
        yield return new WaitForSeconds((int)duration);
        jumpForce = originalJumpForce;
    }
    // Methos to Activate Boosts

    public void UpdateCollectedCoins()
    {
        collectedCoins++;
        collectedCoinsTMP.text = "Coins: " + collectedCoins;
    }

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

    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
