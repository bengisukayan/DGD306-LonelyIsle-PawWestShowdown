using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 8f;
    private System.Action onHitPlayer;

    public void Init(Vector2 direction, System.Action hitCallback)
    {
        GetComponent<Rigidbody2D>().velocity = direction * speed;
        onHitPlayer = hitCallback;
        Destroy(gameObject, 5f); // fallback
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerMovement>()?.TakeDamage(20);
            onHitPlayer?.Invoke();
            Destroy(gameObject);
        }
        else if (other)
        {
            Destroy(gameObject);
        }
    }
}
