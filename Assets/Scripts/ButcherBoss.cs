using UnityEngine;
using UnityEngine.UI; // Required for UI elements like Image or Text

public class BossEnemy : Enemy // Assuming Enemy is your base class
{
    [Header("Boss Stats")]
    public float detectionRadius = 15f;
    public float jumpForce = 20f;
    public float jumpAttackDelay = 0.5f; // Delay before the jump starts after detecting player
    public int squashDamage = 30;
    public float dazeDuration = 3f;

    [Header("References")]
    public LayerMask playerLayer;
    public Transform groundCheck;
    public LayerMask groundLayer;
    public GameObject squashHitEffect; // Particle effect for when the boss lands and squashes
    public Image healthBarFill; // Reference to the fill amount of your health bar UI

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Transform player;

    private BossState currentState = BossState.Idle;
    private float dazeTimer = 0f;
    private bool isGrounded;
    private bool hasLandedAfterJump = false; // To ensure daze only triggers once per jump
    private float initialHealth;

    // Boss States
    public enum BossState
    {
        Idle,
        Chasing,
        PreparingJump,
        Jumping,
        Dazed,
        Dead 
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        initialHealth = health; // Store initial health for health bar calculations
        UpdateHealthBar(); // Initialize health bar
    }

    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        anim.SetBool("IsJumping", !isGrounded); // Assuming "IsJumping" is true when not grounded

        switch (currentState)
        {
            case BossState.Idle:
                FindPlayerAndTransition();
                break;
            case BossState.PreparingJump:
                // Boss might play an animation or wind up here
                FacePlayer();
                if (player != null)
                {
                    // Small delay before actual jump
                    if (Time.time >= dazeTimer) // Re-using dazeTimer for jump preparation delay
                    {
                        JumpAttack();
                        currentState = BossState.Jumping;
                    }
                }
                else
                {
                    currentState = BossState.Idle; // Player out of range while preparing
                }
                break;
            case BossState.Jumping:
                HandleJumpingState();
                break;
            case BossState.Dazed:
                HandleDazedState();
                break;
            case BossState.Chasing:
                // Implement chasing logic if needed. For now, directly jump.
                break;
            case BossState.Dead:
                // Handle death state (e.g., play death animation, disable collider)
                rb.velocity = Vector2.zero;
                break;
        }

        // Always face the player if detected, unless dazed
        if (player != null && currentState != BossState.Dazed)
        {
            FacePlayer();
        }
    }

    private void FindPlayerAndTransition()
    {
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, playerLayer);
        if (playerCollider != null && playerCollider.CompareTag("Player"))
        {
            player = playerCollider.transform;
            currentState = BossState.PreparingJump;
            dazeTimer = Time.time + jumpAttackDelay; // Set delay for the jump
            rb.velocity = Vector2.zero; // Stop any movement while preparing
            anim.SetBool("IsWalking", false); // Stop walking animation if any
        }
    }

    private void FacePlayer()
    {
        if (player != null)
        {
            spriteRenderer.flipX = player.position.x > transform.position.x;
        }
    }

    private void JumpAttack()
    {
        if (player == null)
        {
            currentState = BossState.Idle;
            return;
        }

        // Calculate a jump trajectory to land roughly on the player's current X position
        float playerX = player.position.x;
        float currentX = transform.position.x;
        float distance = Mathf.Abs(playerX - currentX);

        // Simple parabolic jump calculation (can be made more complex for better targeting)
        float horizontalSpeed = distance / (2 * (jumpForce / Physics2D.gravity.y)); // Estimate time in air
        horizontalSpeed = Mathf.Abs(horizontalSpeed);

        Vector2 jumpDirection = Vector2.up * jumpForce;
        if (playerX > currentX)
        {
            jumpDirection += Vector2.right * horizontalSpeed;
        }
        else
        {
            jumpDirection += Vector2.left * horizontalSpeed;
        }

        rb.velocity = jumpDirection;
        hasLandedAfterJump = false; // Reset for next jump
    }

    private void HandleJumpingState()
    {
        if (isGrounded && !hasLandedAfterJump)
        {
            // Boss has landed after a jump
            hasLandedAfterJump = true;
            anim.SetTrigger("Squash"); // Trigger squash animation
            currentState = BossState.Dazed;
            dazeTimer = Time.time + dazeDuration;
            rb.velocity = Vector2.zero; // Stop horizontal movement upon landing
            anim.SetBool("IsDazed", true);

            // Check for players directly below for squash damage
            Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(transform.position, 1.5f, playerLayer); // Small radius for squash
            foreach (Collider2D hitCollider in hitPlayers)
            {
                if (hitCollider.CompareTag("Player") && hitCollider.TryGetComponent<PlayerMovement>(out var playerScript))
                {
                    playerScript.TakeDamage(squashDamage);
                }
            }

            // Instantiate hit effect at landing spot
            if (squashHitEffect != null)
            {
                Instantiate(squashHitEffect, transform.position, Quaternion.identity);
            }
        }
    }

    private void HandleDazedState()
    {
        if (Time.time >= dazeTimer)
        {
            currentState = BossState.Idle;
            anim.SetBool("IsDazed", false);
            player = null; // Clear player reference to force re-detection
        }
    }

    // This method will be called from outside (e.g., from player attack script)
    override public void TakeDamage(int amount) // Using 'override' if Enemy has a virtual TakeDamage
    {
        if (currentState == BossState.Dazed)
        {
            health -= amount;
            UpdateHealthBar(); // Update UI
            Debug.Log($"Boss took {amount} damage. Health: {health}");

            if (health <= 0)
            {
                health = 0;
                Die();
            }
        }
        else
        {
            Debug.Log("Boss is not dazed and cannot take damage!");
            // Optionally, play a "deflect" animation or sound
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = health / initialHealth;
        }
    }

    override protected void Die()
    {
        Debug.Log("Boss has been defeated!");
        currentState = BossState.Dead;
        // Play death animation
        anim.SetTrigger("Die");
        // Disable collider, rigibody, etc.
        GetComponent<Collider2D>().enabled = false;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        // Optionally, destroy GameObject after a delay
        Destroy(gameObject, 3f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        if (groundCheck != null)
        {
            Gizmos.DrawWireSphere(groundCheck.position, 0.2f);
        }
    }
}