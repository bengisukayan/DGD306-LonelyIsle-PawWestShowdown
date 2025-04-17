using UnityEngine;

public class ChargerEnemy : Enemy
{
    public float detectionRange = 6f;
    public float chargeSpeed = 10f;
    public float chargeCooldown = 2f;
    public int damage = 20;
    public LayerMask playerLayer;

    private float nextChargeTime = 0f;
    private Transform player;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isCharging = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        if (PlayerInRange() && Time.time >= nextChargeTime && !isCharging)
        {
            StartCharging();
        }

        animator.SetBool("isRunning", isCharging);
    }

    void StartCharging()
    {
        isCharging = true;
        Vector2 dir = (player.position - transform.position).normalized;
        rb.velocity = dir * chargeSpeed;

        spriteRenderer.flipX = dir.x > 0;

        nextChargeTime = Time.time + chargeCooldown;
        Invoke(nameof(StopCharging), 0.5f); // charge for 0.5 seconds
    }

    void StopCharging()
    {
        rb.velocity = Vector2.zero;
        isCharging = false;
    }

    bool PlayerInRange()
    {
        return Vector2.Distance(transform.position, player.position) <= detectionRange;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isCharging && collision.collider.CompareTag("Player"))
        {
            animator.SetTrigger("isAttacking");

            if (collision.collider.TryGetComponent<PlayerMovement>(out var playerHealth))
            {
                playerHealth.TakeDamage(damage);
            }

            StopCharging(); // stop movement on hit
        }
    }
}
