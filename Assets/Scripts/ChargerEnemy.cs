using UnityEngine;

public class ChargerEnemy : Enemy
{
    public float detectionRadius = 10f;
    public float attackRange = 1.2f;
    public float speed = 5f;
    public float attackCooldown = 1f;
    public int damage = 20;
    public LayerMask playerLayer;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float nextAttackTime = 0f;

    public AudioClip slapSound;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        bool playerDetected = PlayerInSight();

        if (!playerDetected)
        {
            player = FindPlayerInDetectionRadius();
            playerDetected = player != null;
        }

        if (playerDetected && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            float distance = Vector2.Distance(transform.position, player.position);

            // Face the player
            spriteRenderer.flipX = direction.x > 0;

            if (distance > attackRange)
            {
                // Run toward player
                rb.velocity = new Vector2(direction.x * speed, rb.velocity.y);
                anim.SetBool("isRunning", true);
            }
            else
            {
                // Stop and attack
                rb.velocity = Vector2.zero;
                anim.SetBool("isRunning", false);

                if (Time.time >= nextAttackTime)
                {
                    anim.SetTrigger("Hit");
                    DealDamage();
                    nextAttackTime = Time.time + attackCooldown;
                }
            }
        }
        else
        {
            // Idle
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);
        }
    }

    bool PlayerInSight()
    {
        if (player == null) return false;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance > detectionRadius) return false;

        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRadius, playerLayer);

        Debug.DrawRay(transform.position, direction * detectionRadius,
            (hit.collider != null && hit.collider.CompareTag("Player")) ? Color.green : Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private Transform FindPlayerInDetectionRadius()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Vector2 direction = (hit.transform.position - transform.position).normalized;
                RaycastHit2D ray = Physics2D.Raycast(transform.position, direction, detectionRadius, playerLayer);

                if (ray.collider == hit)
                {
                    return hit.transform;
                }
            }
        }
        return null;
    }

    void DealDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            if (player.TryGetComponent<PlayerMovement>(out var playerScript))
            {
                playerScript.TakeDamage(damage);
                audioSource.PlayOneShot(slapSound);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
