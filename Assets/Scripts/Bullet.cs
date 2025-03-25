using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifetime = 2f;

    void Start()
    {
        GetComponent<Rigidbody2D>().velocity = transform.right * speed; // Move bullet in direction of gun
        Destroy(gameObject, lifetime); // Destroy bullet after some time
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Add logic for hitting enemies later
        Destroy(gameObject);
    }
}