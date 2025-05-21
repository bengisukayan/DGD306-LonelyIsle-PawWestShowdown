using UnityEngine;

public class ButcherCatBoss : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;
    public GameObject cleaverPrefab;
    public Transform cleaverSpawn;
    public float walkSpeed = 1.5f;
    public float cleaverCooldown = 2f;
    public float chargeSpeed = 6f;
    public float chargeThreshold = 100f;

    private Transform player;
    private float lastAttackTime;
    private Rigidbody2D rb;
    private bool isCharging;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (isCharging)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = dir * chargeSpeed;
            return;
        }

        rb.velocity = new Vector2((player.position.x > transform.position.x ? 1 : -1) * walkSpeed, rb.velocity.y);

        if (Time.time - lastAttackTime >= cleaverCooldown)
        {
            ThrowCleaver();
            lastAttackTime = Time.time;
        }

        if (currentHealth <= chargeThreshold && !isCharging)
        {
            isCharging = true;
            Invoke("StopCharging", 2f);
        }
    }

    void ThrowCleaver()
    {
        Instantiate(cleaverPrefab, cleaverSpawn.position, Quaternion.identity);
    }

    void StopCharging()
    {
        isCharging = false;
        rb.velocity = Vector2.zero;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Destroy(gameObject); // Add death animation/effects here
    }
}
