using UnityEngine;

public class PatrolEnemy : Enemy
{
    public GameObject pointA;
    public GameObject pointB;
    public float speed;
    public float detectionRange = 10f;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        currentPoint = pointB.transform;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (player && PlayerInSight())
        {
            rb.velocity = Vector2.zero;
            anim.SetBool("isWalking", false);

            // Flip towards player
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

        Vector2 point = currentPoint.position - transform.position;
        rb.velocity = currentPoint == pointB.transform ? new Vector2(speed, 0) : new Vector2(-speed, 0);
        spriteRenderer.flipX = currentPoint == pointB.transform;

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            currentPoint = currentPoint == pointB.transform ? pointA.transform : pointB.transform;
        }
    }

    bool PlayerInSight()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, playerLayer);
        Debug.DrawRay(transform.position, direction * detectionRange, Color.green);
        return hit && hit.collider.CompareTag("Player");
    }

    void ShootRay()
    {
        Vector2 direction = player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, playerLayer);

        if (hit)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hit.point);

            if (hit.collider.CompareTag("Player") && hit.collider.TryGetComponent<PlayerMovement>(out var playerScript))
            {
                playerScript.TakeDamage(damage);
            }

            if (hitEffect)
                Instantiate(hitEffect, hit.point, Quaternion.identity);
        }
        else
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + (Vector3)direction * detectionRange);
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
        Gizmos.DrawWireSphere(pointA.transform.position, 0.5f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * detectionRange);
    }
}
