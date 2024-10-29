using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public TextMeshProUGUI collectedCoinsTMP;

    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    private Animator animator;
    private Rigidbody2D rigidBody2D;

    private bool isGrounded = true;
    private bool isFacingRight = true;
    private float horizontalInput;
    private int collectedCoins = 0;

    private float originalMoveSpeed;
    private float originalJumpForce;

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
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }
    }

    private void FixedUpdate()
    {
        rigidBody2D.velocity = new Vector2(horizontalInput * moveSpeed, rigidBody2D.velocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rigidBody2D.velocity.x));
        animator.SetFloat("yVelocity", rigidBody2D.velocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", !isGrounded);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            isGrounded = false;
            animator.SetBool("isJumping", !isGrounded);
        }
    }

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

    public void UpdateCollectedCoins()
    {
        collectedCoins++;
        collectedCoinsTMP.text = "Coins: " + collectedCoins;
    }

    private void FlipSprite()
    {
        if (isFacingRight && horizontalInput < 0f || !isFacingRight && horizontalInput > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }
}
