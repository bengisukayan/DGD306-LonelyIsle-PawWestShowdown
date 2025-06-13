using UnityEngine;

public class SentryEnemy : Enemy
{
    public float detectionRadius = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public LayerMask playerLayer;

    public LineRenderer lineRenderer;
    public GameObject hitEffect;

    private float nextFireTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Animator anim;

    public AudioClip shoot;
    private AudioSource audioSource;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
            anim.SetBool("isIdle", false);

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
            anim.SetBool("isIdle", true);
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
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
