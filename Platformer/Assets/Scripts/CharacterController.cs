using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    
    private float characterHeight;
    private bool isGrounded = true;
    private bool isFacingRight = true;
    private float horizontalInput;
    private Animator animator;
    private Rigidbody2D rigidBody2D;

    private void Awake()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        rigidBody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        animator = gameObject.GetComponent<Animator>();
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
        isGrounded = true;
        animator.SetBool("isJumping", !isGrounded);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
        animator.SetBool("isJumping", !isGrounded);
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
