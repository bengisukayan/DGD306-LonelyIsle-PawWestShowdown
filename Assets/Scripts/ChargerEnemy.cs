using UnityEngine;

public class ChargerEnemy : Enemy
{
    [Header("Charger Settings")]
    public float detectionRadius = 10f;
    public float attackRange = 1.2f;
    public float speed = 5f;
    public float attackCooldown = 1f;
    public int damage = 20;
    public LayerMask playerLayer;

    [Header("Audio")]
    public AudioClip slapSound;

    private Transform player;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float nextAttackTime = 0f;

    protected override void Start()
    {
        base.Start();  // initializes protected audioSource
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
            Vector2 dir = (player.position - transform.position).normalized;
            float dist = Vector2.Distance(transform.position, player.position);

            // Face
            spriteRenderer.flipX = dir.x > 0;

            if (dist > attackRange)
            {
                rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
                anim.SetBool("isRunning", true);
            }
            else
            {
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
            rb.velocity = Vector2.zero;
            anim.SetBool("isRunning", false);
        }
    }

    private bool PlayerInSight()
    {
        if (player == null) return false;
        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > detectionRadius) return false;

        Vector2 dir = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRadius, playerLayer);

        Debug.DrawRay(transform.position, dir * detectionRadius,
            (hit.collider != null && hit.collider.CompareTag("Player")) ? Color.green : Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private Transform FindPlayerInDetectionRadius()
    {
        foreach (var col in Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer))
        {
            if (col.CompareTag("Player"))
            {
                Vector2 dir = (col.transform.position - transform.position).normalized;
                var ray = Physics2D.Raycast(transform.position, dir, detectionRadius, playerLayer);
                if (ray.collider == col)
                    return col.transform;
            }
        }
        return null;
    }

    private void DealDamage()
    {
        if (player != null && Vector2.Distance(transform.position, player.position) <= attackRange)
        {
            if (player.TryGetComponent<PlayerMovement>(out var pm))
            {
                pm.TakeDamage(damage);

                // play slap SFX
                if (audioSource != null && slapSound != null)
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
