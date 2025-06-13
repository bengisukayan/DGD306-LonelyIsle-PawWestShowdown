using UnityEngine;

public class SentryEnemy : Enemy
{
    [Header("Sentry Settings")]
    public float detectionRadius = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public LayerMask playerLayer;

    [Header("Visuals & Effects")]
    public LineRenderer lineRenderer;
    public GameObject hitEffect;

    [Header("Audio")]
    public AudioClip shoot;

    private float nextFireTime = 0f;
    private SpriteRenderer spriteRenderer;
    private Transform player;
    private Animator anim;

    protected override void Start()
    {
        base.Start();  // initializes protected audioSource
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        bool detected = PlayerInSight();
        if (!detected)
        {
            player = FindPlayerInDetectionRadius();
            detected = player != null;
        }

        if (detected && player != null)
        {
            anim.SetBool("isIdle", false);
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

    private void ShootRay()
    {
        if (player == null) return;

        Vector2 dir = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, detectionRadius, playerLayer);

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, hit.collider ? hit.point : (Vector2)transform.position + dir * detectionRadius);

        // play shoot SFX
        if (audioSource != null && shoot != null)
            audioSource.PlayOneShot(shoot);

        // damage
        if (hit.collider != null && hit.collider.CompareTag("Player") &&
            hit.collider.TryGetComponent<PlayerMovement>(out var pm))
        {
            pm.TakeDamage(damage);
        }

        // VFX
        if (hit.collider != null && hitEffect != null)
            Instantiate(hitEffect, hit.point, Quaternion.identity);

        StartCoroutine(ShowLine());
    }

    private System.Collections.IEnumerator ShowLine()
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
