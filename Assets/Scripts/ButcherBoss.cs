using UnityEngine;

public class ButcherBoss : Enemy
{
    [Header("Settings")]
    public float detectionRadius = 8f;
    public float speed = 3f;
    public float jumpForce = 12f;
    public float attackCooldown = 2f;
    public float dazedDuration = 2f;
    public int damage = 30;
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioClip attackSound;
    
    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private AudioSource butcherAudioSource;

    private bool isGrounded = true;
    private bool isDazed = false;

    private float nextAttackTime = 0f;
    private float dazedEndTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        butcherAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        anim.SetBool("isFalling", !isGrounded && rb.velocity.y < -0.1f);

        if (isDazed)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);

            if (Time.time >= dazedEndTime)
                isDazed = false;

            return;
        }

        if (player == null || !PlayerInSight())
            player = FindPlayerInDetectionRadius();

        if (player != null)
        {
            HandleMovementAndAttack();
        }
        else
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);
        }
    }

    private void HandleMovementAndAttack()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        float distance = Vector2.Distance(transform.position, player.position);

        spriteRenderer.flipX = direction.x > 0;

        if (distance > 3f)
        {
            rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
            anim.SetBool("isRunning", true);
        }
        else if (isGrounded && Time.time >= nextAttackTime)
        {
            rb.velocity = new Vector2(0, jumpForce);
            anim.SetTrigger("Jump");

            if (attackSound != null && butcherAudioSource != null)
                butcherAudioSource.PlayOneShot(attackSound);

            nextAttackTime = Time.time + attackCooldown;
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isGrounded && collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;

            if (player != null && Vector2.Distance(transform.position, player.position) <= 1.5f)
            {
                if (player.TryGetComponent<PlayerMovement>(out var playerScript))
                {
                    playerScript.TakeDamage(damage);
                }
            }
            else
            {
                isDazed = true;
                dazedEndTime = Time.time + dazedDuration;
                anim.SetTrigger("Dazed");
            }
        }
    }

    private bool PlayerInSight()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRadius) return false;

        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRadius, playerLayer);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private Transform FindPlayerInDetectionRadius()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
                return hit.transform;
        }
        return null;
    }

    public override void TakeDamage(int amount)
    {
        if (!isDazed) return;

        health -= amount;

        if (health <= 0)
            Die();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
