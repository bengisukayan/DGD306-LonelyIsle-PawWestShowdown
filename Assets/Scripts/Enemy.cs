using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Enemy : MonoBehaviour
{
    public int health = 100;
    public GameObject deathEffect;
    private AudioSource audioSource;
    public AudioClip enemy_dead;
    public AudioClip enemy_hurt;

    private void Start()
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
            audioSource.PlayOneShot(enemy_dead);
        }
    }

    public virtual void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
            audioSource.PlayOneShot(enemy_dead);
        }
        if (SceneManager.GetActiveScene().name == "Godfather")
        {
            ScoreManager.Instance.AddScore(500);
            SceneManager.LoadScene("Level 2");
        }
        else if (SceneManager.GetActiveScene().name == "Butcher")
        {
            ScoreManager.Instance.AddScore(1000);
            SceneManager.LoadScene("Credits");
        }
        else if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            ScoreManager.Instance.AddScore(100);
        }

        Destroy(gameObject);
    }
}