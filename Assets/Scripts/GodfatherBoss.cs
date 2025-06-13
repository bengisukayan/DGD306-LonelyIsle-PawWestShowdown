using UnityEngine;
using System.Collections;

public class ProjectileBoss : Enemy
{
    [Header("Settings")]
    public float attackCooldown = 3f;
    public int damageIfMissed = 20;
    public Transform[] projectileSpawnPoints; // Points 0–4 are random, 5 is above boss
    public GameObject projectilePrefab;
    public LayerMask playerLayer;

    private Animator anim;
    private float nextAttackTime = 0f;
    private bool awaitingDamage = false;

    private AudioSource godfatherAudioSource;
    public AudioClip cast;

    private void Start()
    {
        anim = GetComponent<Animator>();
        anim.SetBool("isIdle", true); // Start in idle state
        godfatherAudioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Time.time >= nextAttackTime)
        {
            nextAttackTime = Time.time + attackCooldown;
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        anim.SetBool("isIdle", false);
        anim.SetTrigger("Attack");
        godfatherAudioSource.PlayOneShot(cast);

        awaitingDamage = true;

        for (int i = 0; i < 5; i++)
        {
            int index = Random.Range(0, 5);
            Transform spawnPoint = projectileSpawnPoints[index];

            var proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            var p = proj.GetComponent<Projectile>();
            p.Init(Vector2.down, OnProjectileHitPlayer);

            yield return new WaitForSeconds(0.1f);
        }

        yield return new WaitForSeconds(1f);

        if (awaitingDamage) // If none of the projectiles hit the player
        {
            Transform selfPoint = projectileSpawnPoints[5];
            var proj = Instantiate(projectilePrefab, selfPoint.position, Quaternion.identity);
            var p = proj.GetComponent<Projectile>();
            p.Init(Vector2.down, null); // No callback needed
            yield return new WaitForSeconds(0.65f);
            TakeDamage(damageIfMissed);
        }

        anim.SetBool("isIdle", true); // Return to idle
    }

    private void OnProjectileHitPlayer()
    {
        awaitingDamage = false;
    }

    public override void TakeDamage(int amount)
    {
        health -= amount;
        anim.SetTrigger("Hurt");

        if (health <= 0)
        {
            Die();
        }
    }
}
