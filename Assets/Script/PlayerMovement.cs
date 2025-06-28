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

    [SerializeField] Image[] heartIcons;      // Assign di Inspector
    [SerializeField] Sprite heartActive;      // Sprite heart aktif
    [SerializeField] Sprite heartInactive;    // Sprite heart non-aktif

    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.2f; // Tambahkan ini
    [SerializeField] private float invisibleDuration = 2f;

    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator animator;
    private bool isGrounded = false;
    private float moveHorizontal;
    private bool isFacingRight = true;
    private bool isWalkingSoundPlaying = false;
    private bool canMove = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;
        AudioManager.instance.PlayBGM();
        UpdateHeartsUI();
    }

    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");

        if (canMove)
        {
            rb.velocity = new Vector2(moveHorizontal * moveSpeed, rb.velocity.y);
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }

        FlipCharacter();
        PlayFootstepSound();
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
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth = Mathf.Max(currentHealth - damage, 0);
        FindObjectOfType<AudioManager>().Play("death");
        Debug.Log($"Player took damage. Current health: {currentHealth}");
        UpdateHeartsUI();

        StartCoroutine(FlashRed()); // Efek flash merah
        StartCoroutine(Knockback());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(KnockbackAndInvisible());
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
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

    private System.Collections.IEnumerator KnockbackAndInvisible()
    {
        isInvincible = true;
        canMove = false;

        float direction = isFacingRight ? -1f : 1f;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * knockbackForce, 0f), ForceMode2D.Impulse);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        yield return new WaitForSeconds(invisibleDuration);

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        canMove = true;
        isInvincible = false;

        gameOverUI.ShowGameOverUI();
    }

    private System.Collections.IEnumerator Knockback()
    {
        isInvincible = true;
        canMove = false;

        float direction = isFacingRight ? -1f : 1f;
        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(direction * knockbackForce, 0f), ForceMode2D.Impulse);

        yield return new WaitForSeconds(knockbackDuration); // Gunakan knockbackDuration

        canMove = true;
        isInvincible = false;
    }

    private System.Collections.IEnumerator FlashRed()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.4f); // Durasi flash merah diperpanjang
            spriteRenderer.color = originalColor;
        }
    }
}
