using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public abstract class Enemy : MonoBehaviour
{
    [Header("Health & Effects")]
    public int health = 100;
    public GameObject deathEffect;

    [Header("Audio Clips")]
    public AudioClip enemy_hurt;
    public AudioClip enemy_dead;

    protected AudioSource audioSource;

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
        else
        {
            if (audioSource != null && enemy_hurt != null)
                audioSource.PlayOneShot(enemy_hurt);
        }
    }

    public virtual void Die()
    {
        // spawn death VFX
        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        // play death sound (if assigned)
        if (audioSource != null && enemy_dead != null)
            audioSource.PlayOneShot(enemy_dead);

        // score & scene logic
        string scene = SceneManager.GetActiveScene().name;
        if (scene == "Godfather")
        {
            ScoreManager.Instance.AddScore(500);
            SceneManager.LoadScene("Level 2");
        }
        else if (scene == "Butcher")
        {
            ScoreManager.Instance.AddScore(1000);
            SceneManager.LoadScene("Credits");
        }
        else if (scene != "Tutorial")
        {
            ScoreManager.Instance.AddScore(100);
        }

        Destroy(gameObject);
    }
}