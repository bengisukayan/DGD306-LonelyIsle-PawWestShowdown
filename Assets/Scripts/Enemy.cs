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

    protected virtual void Die()
    {
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }
        if (SceneManager.GetActiveScene().name != "Tutorial")
        {
            ScoreManager.Instance.AddScore(100);
        }
        //AudioManager.Instance.PlaySound("EnemyDeath");
        Destroy(gameObject);
    }
}