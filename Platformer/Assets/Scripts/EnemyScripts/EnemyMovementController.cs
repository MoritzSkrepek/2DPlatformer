using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.Image;

public class EnemyMovementController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

    [Header("Attacks")]
    [SerializeField] public float attackDamage;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private LayerMask groundLayer;
    private bool isGrounded = true;

    [Header("Wall Check")]
    [SerializeField] private LayerMask wallLayer;

    // Random movement parameters
    [Header("Random Movement Timing")]
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minMoveTime;
    [SerializeField] private float maxMoveTime;
    private float movementTimer = 0f;
    private int moveDirection = 1;
    private bool isFacingRight = true;

    // Jump cooldown
    [Header("Jump Cooldown")]
    [SerializeField] private float jumpCooldown = 1.5f;
    private float lastJumpTime = -10f;

    // Player following criteria
    [Header("Player following")]
    [SerializeField] private float detectionRadius;
    private bool enemySighted = false;
    private bool shouldJump = false;

    // States for random movement
    private enum EnemyState { Idle, Moving }
    private EnemyState currentState = EnemyState.Idle;

    // Components
    private Rigidbody2D rigidBody2D;
    private Transform playerLocation;

    void Start()
    {
        rigidBody2D = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(RandomMovementRoutine());
    }

    void Update()
    {
        GroundCheck();
        if (IsPlayerSighted() && !CheckObstacleBetween())
        {
            FollowPlayer();
        }
        else
        {
            if (currentState == EnemyState.Moving)
            {
                RandomMove();
            }
            else
            {
                StopFollowingPlayer();
            }
        }
    }

    // Make enemy jump
    private void FixedUpdate()
    {
        if (playerLocation != null)
        {
            if (isGrounded && shouldJump && Time.time > lastJumpTime + jumpCooldown)
            {
                shouldJump = false;
                lastJumpTime = Time.time;
                rigidBody2D.velocity = new Vector2(rigidBody2D.velocity.x, jumpForce);
            }
        }
    }

    // Follow the character
    private void FollowPlayer()
    {
        // If character is not grounded don't check anything
        if (!isGrounded) return;

        float direction = Mathf.Sign(playerLocation.position.x - transform.position.x);
        rigidBody2D.velocity = new Vector2(direction * moveSpeed, rigidBody2D.velocity.y);

        if (isFacingRight && rigidBody2D.velocity.x < 0f || !isFacingRight && rigidBody2D.velocity.x > 0f)
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

    // Stop following the character
    private void StopFollowingPlayer()
    {
        rigidBody2D.velocity = new Vector2(0f, rigidBody2D.velocity.y);
    }

    // Make a random move
    private void RandomMove()
    {
        rigidBody2D.velocity = new Vector2(moveDirection * moveSpeed, rigidBody2D.velocity.y);
        FlipSprite();
        RaycastHit2D gapAhead = Physics2D.Raycast(transform.position + new Vector3(moveDirection, 0, 0), Vector2.down, 2f, groundLayer);
        if (!gapAhead.collider)
        {
            moveDirection *= -1;
            FlipSprite();
        }
        movementTimer -= Time.deltaTime;
        if (movementTimer <= 0f)
        {
            currentState = EnemyState.Idle;
        }
    }

    // Hold the current state of random movement for specified time
    private IEnumerator RandomMovementRoutine()
    {
        while (true)
        {
            if (!enemySighted)
            {
                if (currentState == EnemyState.Idle)
                {
                    // Generate random idle time
                    float idleDuration = UnityEngine.Random.Range(minIdleTime, maxIdleTime);
                    yield return new WaitForSeconds(idleDuration);
                    currentState = EnemyState.Moving;

                    moveDirection = UnityEngine.Random.value > 0.5f ? 1 : -1;
                    movementTimer = UnityEngine.Random.Range(minMoveTime, maxMoveTime);
                }
                else if (currentState == EnemyState.Moving)
                {
                    yield return new WaitForSeconds(movementTimer);
                    currentState = EnemyState.Idle;
                }
            }
            yield return null;
        }
    }

    // Flip sprite if direction is changed
    private void FlipSprite()
    {
        if (isFacingRight && moveDirection == -1 || !isFacingRight && moveDirection == 1)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private bool CheckObstacleBetween()
    {
        if (playerLocation == null) return true;
        RaycastHit2D groundHit = Physics2D.Linecast(transform.position, playerLocation.position, groundLayer);
        RaycastHit2D wallHit = Physics2D.Linecast(transform.position, playerLocation.position, wallLayer);
        return (groundHit.collider != null ||  wallHit.collider != null) ? true : false;
    }

    private bool IsPlayerSighted()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(gameObject.GetComponent<SpriteRenderer>().bounds.center, detectionRadius);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                playerLocation = collider.transform;
                return true;
            }
        }
        playerLocation = null;
        return false;
    }

    // Check if enemey is grounded
    private void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckPos.position, detectionRadius);
    }
}