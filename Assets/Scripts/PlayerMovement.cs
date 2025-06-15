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

    [Header("Audio")]
    public AudioClip walkSound;
    public AudioClip jumpSound;
    public AudioClip hurtSound;

    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;
    private bool isGrounded;
    private bool isCrouching;
    private bool isFalling;
    private bool isDead;
    private PlayerShooting shooting;

    private float defaultMoveSpeed;
    private Vector2 moveInput;
    private bool jumpPressed;
    private bool crouchPressed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        shooting = GetComponent<PlayerShooting>();
        audioSource = GetComponent<AudioSource>();
        defaultMoveSpeed = moveSpeed;
        health = startingHealth;
    }

    void Update()
    {
        if (isDead) return;

        // Poll input from InputManager
        moveInput = new Vector2(Input.GetAxis("p_horizontal"), Input.GetAxis("p_vertical"));
        crouchPressed = moveInput.y < -0.5f;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        isCrouching = crouchPressed && isGrounded;
        anim.SetBool("Crouch", isCrouching);

        if (!isCrouching)
        {
            rb.velocity = new Vector2(moveInput.x * moveSpeed, rb.velocity.y);

            if (isGrounded && Mathf.Abs(moveInput.x) > 0.1f)
            {
                if (audioSource.clip != walkSound || !audioSource.isPlaying)
                {
                    audioSource.clip = walkSound;
                    audioSource.loop = true;
                    audioSource.Play();
                }
            }
            else if (audioSource.loop && audioSource.clip == walkSound)
            {
                audioSource.Stop();
                audioSource.loop = false;
            }

            if (moveInput.x != 0)
            {
                transform.rotation = Quaternion.Euler(0, moveInput.x > 0 ? 0 : 180, 0);
            }
        }

        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("Jump");

            if (jumpSound != null)
                audioSource.PlayOneShot(jumpSound);
            jumpPressed = false;
        }

        isFalling = rb.velocity.y < -0.1f && !isGrounded;

        anim.SetFloat("Speed", Mathf.Abs(moveInput.x));
        anim.SetBool("IsGrounded", isGrounded);
        anim.SetBool("IsFalling", isFalling);
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        anim.SetTrigger("Hurt");
        rb.velocity = Vector2.zero;

        if (hurtSound != null)
            audioSource.PlayOneShot(hurtSound);

        health -= damage;
        if (health <= 0)
        {
            StartCoroutine(Die());
        }
        else
        {
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
        shooting.enabled = false;
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
        shooting.enabled = true;
    }
    
    public void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
        }
    }
}
