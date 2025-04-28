using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform gunPoint;
    
    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isCrouching;
    private bool isFalling;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        
        float moveInput = Input.GetAxisRaw("Horizontal");
        bool jump = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool crouch = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        
        // Handle Movement
        if (!isCrouching) // Can't move while crouching
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
            if (moveInput != 0)
            {
                transform.rotation = Quaternion.Euler(0, moveInput > 0 ? 0f : 180f, 0);
            }
        }

        // Handle Jump
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }

        // Determine if falling
        isFalling = rb.velocity.y < -0.1f && !isGrounded;
        
        // Handle Crouch
        isCrouching = crouch && isGrounded;
        anim.SetBool("Crouch", isCrouching);

        // Set Animation States
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsFalling", isFalling);
    }

    public void TakeDamage(int damage)
    {
        anim.SetTrigger("Hurt"); // Play the Hurt animation
        rb.velocity = Vector2.zero; // Stop movement briefly

        health -= damage;
        if (health <= 0)
        {
            //Die();
        }

        // Optionally add knockback
        rb.AddForce(new Vector2(-5f * transform.localScale.x, 5f), ForceMode2D.Impulse);

        // Prevent movement for a short duration
        StartCoroutine(RecoverFromHurt());
    }

    private IEnumerator RecoverFromHurt()
    {
        moveSpeed = 0; // Temporarily disable movement
        yield return new WaitForSeconds(1f);
        moveSpeed = 5f; // Restore movement speed
    }
}
