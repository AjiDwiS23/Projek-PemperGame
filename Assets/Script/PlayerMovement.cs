using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public GameOverUI gameOverUI;
    public float jumpForce = 5f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public int maxHealth = 2;
    private int currentHealth;

    public Vector3 respawnPoint;
    public TextMeshProUGUI healthText;

    [SerializeField] Image[] heartIcons;      // Assign di Inspector, urut dari kiri ke kanan
    [SerializeField] Sprite heartActive;      // Sprite heart aktif
    [SerializeField] Sprite heartInactive;    // Sprite heart non-aktif

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;
    private float moveHorizontal;
    private bool isFacingRight = true;
    private bool isWalkingSoundPlaying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
        respawnPoint = transform.position;
        AudioManager.instance.PlayBGM();
        UpdateHealthText();
    }

    void Update()
    {
        // Horizontal movement
        moveHorizontal = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveHorizontal * moveSpeed, rb.linearVelocity.y);

        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jumping
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            //AudioManager.instance.Play("jump"); // Suara melompat (jika ada di AudioManager)
        }

        // Flip character if needed
        FlipCharacter();

        // Play footstep sound if walking and grounded
        PlayFootstepSound();

        // Update animations
        UpdateAnimationState();
    }

    void PlayFootstepSound()
    {
        if (moveHorizontal != 0 && isGrounded)
        {
            if (!isWalkingSoundPlaying)
            {
                FindObjectOfType<AudioManager>().Play("footstep");
                isWalkingSoundPlaying = true;
            }
        }
        else
        {
            if (isWalkingSoundPlaying)
            {
                FindObjectOfType<AudioManager>().Stop("footstep");
                isWalkingSoundPlaying = false;
            }
        }
    }

    void UpdateAnimationState()
    {
        if (moveHorizontal != 0 && isGrounded)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }

        if (!isGrounded)
        {
            animator.SetBool("isJumping", true);
        }
        else
        {
            animator.SetBool("isJumping", false);
        }
    }

    void FlipCharacter()
    {
        if (moveHorizontal > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveHorizontal < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("DeathZone"))
        {
            Debug.Log("Player touched the death zone!");
            TakeDamage(1);

            if (currentHealth > 0)
            {
                Respawn();
            }
            else
            {
                Die();
            }
        }
    }

    void Respawn()
    {
        transform.position = respawnPoint;
        rb.linearVelocity = Vector2.zero;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Max(currentHealth - damage, 0);
        FindObjectOfType<AudioManager>().Play("death");
        Debug.Log($"Player took damage. Current health: {currentHealth}");
        UpdateHealthText();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        gameOverUI.ShowGameOverUI();
        currentHealth = maxHealth;
        Respawn();
        UpdateHealthText();
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    void UpdateHealthText()
    {
        healthText.text = $"Health: {currentHealth}";
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i < currentHealth)
                heartIcons[i].sprite = heartActive;
            else
                heartIcons[i].sprite = heartInactive;
        }
    }
}
