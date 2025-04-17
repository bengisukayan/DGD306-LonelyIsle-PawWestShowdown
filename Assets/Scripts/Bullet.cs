using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;
    public int damage = 50;
    public GameObject hitEffect;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed; // Move bullet in direction of gun
        Destroy(gameObject, lifetime); // Destroy bullet after some time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Instantiate(hitEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}