using UnityEngine;

public class EnemyJumpAttack : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float fallSpeed = 10f;
    [SerializeField] private float attackDelay = 0.5f;
    [SerializeField] private float attackCooldown = 1.0f; // Durasi cooldown setelah serangan
    [SerializeField] private Collider2D triggerArea; // Assign area trigger di Inspector

    [Header("Enemy Health")]
    [SerializeField] private int maxHealth = 1;
    private int currentHealth;

    private Rigidbody2D rb;
    private bool isAttacking = false;
    private bool isVulnerable = false;
    private Transform playerInArea = null;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (triggerArea != null)
            triggerArea.isTrigger = true;

        currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = other.transform;
            TryAttack();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInArea = other.transform;
            TryAttack();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerInArea == other.transform)
                playerInArea = null;
        }
    }

    private void TryAttack()
    {
        if (!isAttacking && playerInArea != null && !isVulnerable)
        {
            StartCoroutine(JumpAttack());
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAttacking && collision.gameObject.CompareTag("Player"))
        {
            PlayerMovement playerMovement = collision.gameObject.GetComponent<PlayerMovement>();
            if (playerMovement != null)
                playerMovement.TakeDamage(1);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Mulai efek penyet dan hilang
        StartCoroutine(SquashAndHide());
    }

    private System.Collections.IEnumerator SquashAndHide()
    {
        // Efek penyet: scale X membesar, Y mengecil
        Transform t = transform;
        Vector3 originalScale = t.localScale;
        Vector3 squashScale = new Vector3(originalScale.x * 1.3f, originalScale.y * 0.4f, originalScale.z);

        // Ubah scale untuk efek penyet
        t.localScale = squashScale;

        // (Opsional) Nonaktifkan collider agar tidak bisa disentuh lagi
        Collider2D[] colliders = GetComponentsInChildren<Collider2D>();
        foreach (var col in colliders)
            col.enabled = false;

        // Tunggu beberapa detik sebelum menghilang (misal 0.7 detik)
        yield return new WaitForSeconds(0.7f);

        // Hilangkan sprite (bisa destroy atau disable SpriteRenderer)
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (var sr in sprites)
            sr.enabled = false;

        // Tunggu sebentar sebelum destroy (misal 0.5 detik)
        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }

    private System.Collections.IEnumerator JumpAttack()
    {
        isAttacking = true;

        // Lompat ke atas player (ambil posisi X player saat trigger)
        Vector2 targetAbovePlayer = new Vector2(playerInArea.position.x, playerInArea.position.y + jumpHeight);
        rb.gravityScale = 0;
        float t = 0;
        Vector2 start = transform.position;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            rb.MovePosition(Vector2.Lerp(start, targetAbovePlayer, t));
            yield return null;
        }

        yield return new WaitForSeconds(attackDelay);

        // Jatuh lurus ke bawah dari posisi di atas player
        rb.gravityScale = 1;
        rb.linearVelocity = new Vector2(0f, -fallSpeed);

        // Tunggu sampai enemy menyentuh tanah (atau bisa pakai OnCollisionEnter2D)
        yield return new WaitForSeconds(1.5f);

        isAttacking = false;
        isVulnerable = true;

        // Cooldown: selama waktu ini player bisa mengalahkan musuh
        yield return new WaitForSeconds(attackCooldown);

        isVulnerable = false;

        // Jika player masih di area, serang lagi
        if (playerInArea != null)
        {
            TryAttack();
        }
    }

    public bool IsVulnerable()
    {
        return isVulnerable;
    }
}
