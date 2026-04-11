using UnityEngine;

public enum MonsterType
{
    Slime,
    Goblin,
    Orc,
    Skeleton,
    Boss
}

[System.Serializable]
public class EnemyStats
{
    [Header("Identity")]
    public int monsterId;
    public MonsterType monsterType;

    [Header("Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float movementSpeed = 3f;

    public void Initialize()
    {
        currentHealth = maxHealth;
    }
}

// Script utama — pasang di root GameObject enemy.
// Trigger detection ada di file terpisah: EnemyDetectionZone.cs
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController_2D : MonoBehaviour
{
    // ─────────────────────────────────────────
    // Inspector
    // ─────────────────────────────────────────
    [Header("Enemy Stats")]
    public EnemyStats stats = new EnemyStats();

    [Header("Chase Settings")]
    [Tooltip("Radius trigger area deteksi player. Ubah nilai ini → langsung terasa di game.")]
    public float detectionRadius = 5f;

    [Tooltip("Jarak berhenti mengejar player")]
    public float stopDistance = 0.5f;

    [Tooltip("Layer mask untuk player")]
    public LayerMask playerLayer;

    [Header("Patrol Settings")]
    public bool enablePatrol = true;
    public float patrolDistance = 3f;
    public float patrolSpeed = 1.5f;

    [Header("Debug")]
    public bool showGizmos = true;

    [Header("Combat")]
    [Tooltip("Damage dealt to player on contact.")]
    public int damageToPlayer = 1;
    [Tooltip("Seconds between consecutive damage applications to the same player.")]
    public float damageCooldown = 1f;

    [Header("Damage Zone (separate dari DetectionZone)")]
    [Tooltip("Enable a dedicated trigger zone used only for applying damage.")]
    public bool useDamageZone = true;
    [Tooltip("Radius of the damage trigger zone. If <= 0 the DamageZone's inspector radius is kept.")]
    public float damageRadius = 0.5f;

    [Header("Damage -> force player invincible/invisible for this duration after being damaged (0 = don't force)")]
    public float damageInvincibleDuration = 0.8f;

    // ─────────────────────────────────────────
    // Private
    // ─────────────────────────────────────────
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private EnemyDetectionZone detectionZone;

    private Transform playerTransform;
    private bool isChasing = false;
    private bool isDead = false;

    private Vector2 patrolStartPos;
    private int patrolDirection = 1;

    // damage timer to avoid spamming player every frame (kept per-player in DamageZone)
    private float lastDamageTime = -Mathf.Infinity;

    // reference to damage zone component (if created)
    private EnemyDamageZone damageZone;

    // Animator + state tracking for 'isFound' parameter
    private Animator animator;
    private bool lastIsFound = false;

    // ─────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        stats.Initialize();
        patrolStartPos = transform.position;

        SetupDetectionZone();
        if (useDamageZone)
            SetupDamageZone();

        // animator (visual child)
        animator = GetComponentInChildren<Animator>();

        Debug.Log($"[Enemy] ID:{stats.monsterId} | Type:{stats.monsterType} | HP:{stats.maxHealth} | Speed:{stats.movementSpeed} | Detection:{detectionRadius}");
    }

    void Update()
    {
        if (isDead) return;

        if (isChasing && playerTransform != null)
            ChasePlayer();
        else if (enablePatrol)
            Patrol();

        // Ensure animator's isFound reflects current chase state.
        bool shouldFound = isChasing && playerTransform != null;
        SetAnimatorIsFound(shouldFound);
    }

    // Saat nilai detectionRadius diubah di Inspector → radius collider langsung update
    void OnValidate()
    {
        if (detectionZone != null)
            detectionZone.SetRadius(detectionRadius);

        if (damageZone != null)
            damageZone.SetRadius(damageRadius);
    }

    // ─────────────────────────────────────────
    // Setup Detection Zone (Child Object)
    // ─────────────────────────────────────────

    void SetupDetectionZone()
    {
        // Cari child yang sudah ada (misal kamu buat manual di editor)
        Transform existing = transform.Find("DetectionZone");

        GameObject zoneObj;
        if (existing != null)
        {
            zoneObj = existing.gameObject;
        }
        else
        {
            // Buat child baru jika belum ada
            zoneObj = new GameObject("DetectionZone");
            zoneObj.transform.SetParent(transform);
            zoneObj.transform.localPosition = Vector3.zero;
        }

        // Pastikan layer sama dengan enemy agar tidak tabrakan dengan physics layer
        zoneObj.layer = gameObject.layer;

        detectionZone = zoneObj.GetComponent<EnemyDetectionZone>();
        if (detectionZone == null)
            detectionZone = zoneObj.AddComponent<EnemyDetectionZone>();

        // Init dengan radius dari Inspector — ini yang selalu dipakai
        detectionZone.Init(this, detectionRadius);
    }

    // ─────────────────────────────────────────
    // Setup Damage Zone (Child Object) - terpisah dari DetectionZone
    // ─────────────────────────────────────────
    void SetupDamageZone()
    {
        Transform existing = transform.Find("DamageZone");
        GameObject zoneObj;
        if (existing != null)
        {
            zoneObj = existing.gameObject;
        }
        else
        {
            zoneObj = new GameObject("DamageZone");
            zoneObj.transform.SetParent(transform);
            zoneObj.transform.localPosition = Vector3.zero;
        }

        // Put on same layer as enemy to avoid unwanted physics interactions
        zoneObj.layer = gameObject.layer;

        damageZone = zoneObj.GetComponent<EnemyDamageZone>();
        if (damageZone == null)
            damageZone = zoneObj.AddComponent<EnemyDamageZone>();

        damageZone.Init(this, damageRadius);
    }

    // ─────────────────────────────────────────
    // Dipanggil oleh EnemyDetectionZone
    // ─────────────────────────────────────────

    public void OnPlayerEnter(Transform player)
    {
        playerTransform = player;
        isChasing = true;
        SetAnimatorIsFound(true);
        Debug.Log($"[Enemy {stats.monsterId}] Player terdeteksi! Mulai mengejar...");
    }

    public void OnPlayerExit()
    {
        isChasing = false;
        playerTransform = null;
        SetAnimatorIsFound(false);
        Debug.Log($"[Enemy {stats.monsterId}] Player keluar area. Berhenti mengejar.");
    }

    // ─────────────────────────────────────────
    // Movement
    // ─────────────────────────────────────────

    void ChasePlayer()
    {
        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * stats.movementSpeed;
        FlipSprite(direction.x);
    }

    void Patrol()
    {
        Vector2 targetPos = patrolStartPos + new Vector2(patrolDistance * patrolDirection, 0f);
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * patrolSpeed;
        FlipSprite(direction.x);

        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            patrolDirection *= -1;
    }

    void FlipSprite(float horizontalDir)
    {
        if (spriteRenderer == null) return;
        if (horizontalDir > 0) spriteRenderer.flipX = false;
        else if (horizontalDir < 0) spriteRenderer.flipX = true;
    }

    // safely set animator bool only when value changes
    private void SetAnimatorIsFound(bool value)
    {
        if (animator == null) return;
        if (lastIsFound == value) return;
        animator.SetBool("isFound", value);
        lastIsFound = value;
    }

    // ─────────────────────────────────────────
    // Combat: deal damage to player on contact
    // ─────────────────────────────────────────

    // Helper to resolve root GameObject (handles attachedRigidbody setups)
    private GameObject GetRootGameObject(Collider2D other)
    {
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.gameObject;
        return other.transform.root != null ? other.transform.root.gameObject : other.gameObject;
    }

    private void TryDamagePlayer(Collider2D other)
    {
        if (isDead) return;
        if (!other.CompareTag("Player")) return;

        GameObject playerRoot = GetRootGameObject(other);
        if (playerRoot == null) return;

        var player = playerRoot.GetComponent<PlayerMovement>();
        if (player == null) return;

        if (Time.time - lastDamageTime < damageCooldown) return;

        // Apply single damage — PlayerMovement handles knockback and temporary invincibility/invisibility
        player.TakeDamage(damageToPlayer);
        if (damageInvincibleDuration > 0f)
            player.SetTemporaryInvincible(damageInvincibleDuration, true);
        lastDamageTime = Time.time;
    }

    // Only keep collision-based contact here (physical collisions)
    void OnCollisionEnter2D(Collision2D collision)
    {
        TryDamagePlayer(collision.collider);
    }

    // This method is retained for compatibility (not used by new DamageZone continuous update flow)
    public void OnDamageTriggerEnter(Collider2D other)
    {
        TryDamagePlayer(other);
    }

    // ─────────────────────────────────────────
    // Health System
    // ─────────────────────────────────────────

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        stats.currentHealth -= damage;
        Debug.Log($"[Enemy {stats.monsterId}] HP: {stats.currentHealth}/{stats.maxHealth}");

        if (stats.currentHealth <= 0)
            Die();
    }

    void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        SetAnimatorIsFound(false);
        Debug.Log($"[Enemy {stats.monsterId}] ({stats.monsterType}) Mati!");
        Destroy(gameObject, 1f);
    }

    // ─────────────────────────────────────────
    // Gizmos
    // ─────────────────────────────────────────

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Detection radius (kuning / merah saat chase)
        Gizmos.color = isChasing ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Stop distance (merah muda)
        Gizmos.color = new Color(1f, 0.3f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, stopDistance);

        // Patrol range (biru)
        if (enablePatrol)
        {
            Gizmos.color = Color.cyan;
            Vector3 start = Application.isPlaying ? (Vector3)patrolStartPos : transform.position;
            Gizmos.DrawLine(start + Vector3.left * patrolDistance, start + Vector3.right * patrolDistance);
            Gizmos.DrawWireSphere(start + Vector3.left * patrolDistance, 0.15f);
            Gizmos.DrawWireSphere(start + Vector3.right * patrolDistance, 0.15f);
        }

        // Damage zone gizmo preview
        if (useDamageZone)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, damageRadius);
        }
    }
}