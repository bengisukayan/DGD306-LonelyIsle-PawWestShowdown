using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [Header("Stats")]
    public int startingHealth = 100;
    public int health;
    public int lives = 3;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public float respawnDelay = 3f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isCrouching;
    private bool isFalling;
    private bool isDead;

    private float defaultMoveSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        defaultMoveSpeed = moveSpeed;
        health = startingHealth;
    }

    void Update()
    {
        if (isDead) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        float moveInput = Input.GetAxisRaw("Horizontal");
        bool jump = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);
        bool crouch = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

        // Handle Crouch
        isCrouching = crouch && isGrounded;
        anim.SetBool("Crouch", isCrouching);

        // Movement
        if (!isCrouching)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

            if (moveInput != 0)
                transform.rotation = Quaternion.Euler(0, moveInput > 0 ? 0 : 180, 0);
        }

        // Jump
        if (jump && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");
        }

        // Falling
        isFalling = rb.velocity.y < -0.1f && !isGrounded;

        // Animator
        anim.SetFloat("Speed", Mathf.Abs(moveInput));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsFalling", isFalling);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        anim.SetTrigger("Hurt");
        rb.velocity = Vector2.zero;

        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
            // Optional knockback
            rb.AddForce(new Vector2(-5f * transform.localScale.x, 5f), ForceMode2D.Impulse);
            StartCoroutine(RecoverFromHurt());
        }
    }

    private IEnumerator RecoverFromHurt()
    {
        moveSpeed = 0;
        yield return new WaitForSeconds(1f);
        moveSpeed = defaultMoveSpeed;
    }

    private IEnumerator Die()
    {
        isDead = true;
        moveSpeed = 0;
        anim.SetTrigger("Die");

        lives--;

        yield return new WaitForSeconds(respawnDelay);

        if (lives <= 0)
        {
            SceneManager.LoadScene("GameOver");
        }
        else
        {
            ResetPlayer();
        }
    }

    private void ResetPlayer()
    {
        isDead = false;
        health = startingHealth;
        moveSpeed = defaultMoveSpeed;
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;
    }
}
