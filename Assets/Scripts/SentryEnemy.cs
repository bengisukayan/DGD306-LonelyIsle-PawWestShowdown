using UnityEngine;

public class SentryEnemy : Enemy
{
    public float detectionRange = 10f;
    public float fireRate = 1f;
    public int damage = 10;
    public LayerMask playerLayer;
    private Animator anim;

    private float nextFireTime = 0f;
    private Transform player;
    private SpriteRenderer spriteRenderer;
    public LineRenderer lineRenderer;
    public GameObject hitEffect;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player && PlayerInSight())
        {
            anim.SetBool("isIdle", false);
            float dir = player.position.x - transform.position.x;
            spriteRenderer.flipX = dir > 0;

            if (Time.time >= nextFireTime)
            {
                anim.SetTrigger("Shoot");
                ShootRay();
                nextFireTime = Time.time + fireRate;
            }
        } else {
            anim.SetBool("isIdle", true);
        }
    }

    bool PlayerInSight()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, playerLayer);
        Debug.DrawRay(transform.position, direction * detectionRange, Color.red);
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
}