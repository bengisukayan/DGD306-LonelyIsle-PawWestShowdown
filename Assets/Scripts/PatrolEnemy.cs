using UnityEngine;

public class PatrolEnemy : Enemy
{
    public GameObject pointA;
    public GameObject pointB;
    public float speed;
    public float detectionRadius = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public LayerMask playerLayer;

    public LineRenderer lineRenderer;
    public GameObject hitEffect;

    private float nextFireTime = 0f;
    private Rigidbody2D rb;
    private Transform currentPoint;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Animator anim;

    public AudioClip shoot;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        bool isPlayerDetected = PlayerInSight();

        if (!isPlayerDetected)
        {
            player = FindPlayerInDetectionRadius();
            isPlayerDetected = player != null;
        }

        if (isPlayerDetected && player != null)
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            // Face the player
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

        Vector2 directionToPoint = (currentPoint.position - transform.position).normalized;
        rb.velocity = new Vector2(directionToPoint.x * speed, rb.velocity.y);

        spriteRenderer.flipX = rb.velocity.x > 0.01f;

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
        }
    }

    bool PlayerInSight()
    {
        if (player == null) return false;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer > detectionRadius) return false;

        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, playerLayer);

        Debug.DrawRay(transform.position, directionToPlayer * detectionRadius,
            (hit.collider != null && hit.collider.CompareTag("Player")) ? Color.green : Color.red);

        return hit.collider != null && hit.collider.CompareTag("Player");
    }

    private Transform FindPlayerInDetectionRadius()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);
        foreach (Collider2D collider in hitColliders)
        {
            if (collider.CompareTag("Player"))
            {
                Vector2 directionToPlayer = (collider.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, detectionRadius, playerLayer);

                if (hit.collider == collider)
                {
                    return collider.transform;
                }
            }
        }
        return null;
    }

    void ShootRay()
    {
        if (player == null) return;

        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRadius, playerLayer);

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.collider ? hit.point : transform.position + (Vector3)direction * detectionRadius);
        audioSource.PlayOneShot(shoot); 

        if (hit.collider != null && hit.collider.CompareTag("Player") &&
            hit.collider.TryGetComponent<PlayerMovement>(out var playerScript))
        {
            playerScript.TakeDamage(damage);
        }

        if (hit.collider != null && hitEffect)
        {
            Instantiate(hitEffect, hit.point, Quaternion.identity);
        }

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
        if (pointA != null) Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        if (pointB != null) Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
