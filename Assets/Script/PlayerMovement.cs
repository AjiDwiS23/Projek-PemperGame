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

    private Vector3 checkpointPosition;

    // Untuk moving platform
    private MovingPlatform currentPlatform;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        currentHealth = maxHealth;
        AudioManager.instance.PlayBGM();
        UpdateHeartsUI();

        // Cek apakah ada checkpoint tersimpan
        if (PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            checkpointPosition = new Vector3(x, y, z);
            transform.position = checkpointPosition;
        }
        else
        {
            checkpointPosition = transform.position;
        }

        // Cek apakah PermainanManager ada dan checkpoint valid
        if (PermainanManager.Instance != null)
        {
            Vector3 checkpoint = PermainanManager.Instance.GetCheckpoint();
            transform.position = checkpoint;
        }
    }

    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal");

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump") && isGrounded && canMove)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        FlipCharacter();
        PlayFootstepSound();
        UpdateAnimationState();
    }

    void FixedUpdate()
    {
        float horizontalVelocity = moveHorizontal * moveSpeed;
        float platformVelocity = 0f;

        if (currentPlatform != null)
        {
            platformVelocity = currentPlatform.GetPlatformVelocity().x;
        }

        if (canMove)
        {
            rb.linearVelocity = new Vector2(horizontalVelocity + platformVelocity, rb.linearVelocity.y);
        }
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
        FindObjectOfType<AudioManager>().Play("Hit");
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

    public void SetCheckpoint(Vector3 pos)
    {
        checkpointPosition = pos;
        PlayerPrefs.SetFloat("CheckpointX", pos.x);
        PlayerPrefs.SetFloat("CheckpointY", pos.y);
        PlayerPrefs.SetFloat("CheckpointZ", pos.z);
        PlayerPrefs.Save();
    }

    public void RespawnAtCheckpoint()
    {
        if (PermainanManager.Instance != null)
        {
            transform.position = PermainanManager.Instance.GetCheckpoint();
            rb.linearVelocity = Vector2.zero;
            currentHealth = maxHealth;
            UpdateHeartsUI();
            canMove = true;
            isInvincible = false;
            if (spriteRenderer != null)
                spriteRenderer.enabled = true;
        }
    }

    private System.Collections.IEnumerator KnockbackAndInvisible()
    {
        isInvincible = true;
        canMove = false;

        float direction = isFacingRight ? -1f : 1f;
        rb.linearVelocity = Vector2.zero;
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
        rb.linearVelocity = Vector2.zero;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = collision.gameObject.GetComponent<MovingPlatform>();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("MovingPlatform"))
        {
            currentPlatform = null;
        }
    }
}
