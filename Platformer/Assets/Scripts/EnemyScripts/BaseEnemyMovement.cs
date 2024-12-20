using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEnemyMovement : MonoBehaviour
{
    [Header("Movement settings")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float jumpCooldown;
    protected float lastJumpTime = -10f;

    [Header("Patrol settings")]
    [SerializeField] private float minIdleTime;
    [SerializeField] private float maxIdleTime;
    [SerializeField] private float minMoveTime;
    [SerializeField] private float maxMoveTime;
    protected float movementTimer = 0f;
    protected int moveDirection = 1;
    protected bool isFacingRight = true;

    [Header("Player follow settings")]
    [SerializeField] private float detectionRadius;
    protected bool enemySighted = false;
    protected bool shouldJump = false;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPos;
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] protected LayerMask groundLayer;
    protected bool isGrounded = true;

    [Header("Wall Check")]
    [SerializeField] protected LayerMask wallLayer;

    protected Rigidbody2D rigidBody;
    protected Transform playerLocation;
    protected enum EnemyState { Idle, Moving }
    protected EnemyState currentState = EnemyState.Idle;

    protected virtual void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody2D>();
        StartCoroutine(RandomMovementRoutine());
    }

    protected virtual void Update()
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

    protected virtual void FixedUpdate()
    {
        if (playerLocation != null)
        {
            if (isGrounded && shouldJump && Time.time > lastJumpTime + jumpCooldown)
            {
                shouldJump = false;
                lastJumpTime = Time.time;
                rigidBody.velocity = new Vector2(rigidBody.velocity.x, jumpForce);
            }
        }
    }

    public void StopFollowingPlayer()
    {
        rigidBody.velocity = new Vector2(0f, rigidBody.velocity.y);
    }

    protected virtual IEnumerator RandomMovementRoutine()
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

    protected virtual void RandomMove()
    {
        rigidBody.velocity = new Vector2(moveDirection * moveSpeed, rigidBody.velocity.y);
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

    protected virtual void FlipSprite()
    {
        if (isFacingRight && moveDirection == -1 || !isFacingRight && moveDirection == 1)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public bool CheckObstacleBetween()
    {
        if (playerLocation == null) return true;
        RaycastHit2D groundHit = Physics2D.Linecast(transform.position, playerLocation.position, groundLayer);
        RaycastHit2D wallHit = Physics2D.Linecast(transform.position, playerLocation.position, wallLayer);
        return (groundHit.collider != null || wallHit.collider != null) ? true : false;
    }

    public bool IsPlayerSighted()
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

    public void GroundCheck()
    {
        isGrounded = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
    }

    protected void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheckPos.position, detectionRadius);
    }

    protected abstract void FollowPlayer();
}
