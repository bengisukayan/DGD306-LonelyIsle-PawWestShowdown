using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class Enemy : MonoBehaviour
{
    public int health = 100;
    public GameObject deathEffect;

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        if (SceneManager.GetActiveScene().name == "Godfather")
        {
            ScoreManager.Instance.AddScore(500);
            SceneManager.LoadScene("Level2");
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

        //AudioManager.Instance.PlaySound("EnemyDeath");
        Destroy(gameObject);
    }
}