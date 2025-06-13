using UnityEngine;

public class PatrolEnemy : Enemy
{
    [Header("Patrol Points")]
    public GameObject pointA;
    public GameObject pointB;
    public float speed = 2f;

    [Header("Detection & Shooting")]
    public float detectionRadius = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public LayerMask playerLayer;
    public AudioClip shoot;

    [Header("References")]
    public LineRenderer lineRenderer;
    public GameObject hitEffect;

    private float nextFireTime;
    private Rigidbody2D rb;
    private Transform currentPoint;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Animator anim;

    protected override void Start()
    {
        base.Start();  // <-- initializes audioSource
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
    }

    void Update()
    {
        bool sawPlayer = PlayerInSight();

        if (!sawPlayer)
        {
            player = FindPlayerInDetectionRadius();
            sawPlayer = player != null;
        }

        if (sawPlayer && player != null)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            // face the player
            spriteRenderer.flipX = player.position.x > transform.position.x;

            if (Time.time >= nextFireTime)
            {
                anim.SetTrigger("Shoot");
                ShootRay();
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        anim.SetBool("isWalking", true);

        Vector2 dir = (currentPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(dir.x * speed, rb.velocity.y);
        spriteRenderer.flipX = rb.velocity.x > 0.01f;

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
            currentPoint = currentPoint == pointB.transform
                ? pointA.transform
                : pointB.transform;
    }

    bool PlayerInSight()
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

    Transform FindPlayerInDetectionRadius()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (var col in cols)
        {
            if (col.CompareTag("Player"))
            {
                Vector2 dir = (col.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRadius, playerLayer);
                if (hit.collider == col)
                    return col.transform;
            }
        }
        return null;
    }

    void ShootRay()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRadius, playerLayer);

        // draw beam
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.collider ? hit.point
            : (Vector2)transform.position + dir * detectionRadius);

        // play shoot SFX
        if (audioSource != null && shoot != null)
            audioSource.PlayOneShot(shoot);

        // apply damage
        if (hit.collider != null && hit.collider.CompareTag("Player") &&
            hit.collider.TryGetComponent<PlayerMovement>(out var pm))
        {
            pm.TakeDamage(damage);
        }

        // hit VFX
        if (hit.collider != null && hitEffect != null)
            Instantiate(hitEffect, hit.point, Quaternion.identity);

        StartCoroutine(ShowLine());
    }

    System.Collections.IEnumerator ShowLine()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(0.05f);
        lineRenderer.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (pointA) Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        if (pointB) Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
