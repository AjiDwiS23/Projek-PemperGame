using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public enum MonsterType
    {
        Melee,
        Ranged,
        Tank,
        Boss,
        Neutral
    }

    [Header("Stats")]
    [Tooltip("Current health of the enemy.")]
    [SerializeField] private int health = 100;
    [Tooltip("Unique numeric ID for this monster.")]
    [SerializeField] private int monsterId = 0;
    [Tooltip("Type/class of the monster.")]
    [SerializeField] private MonsterType monsterType = MonsterType.Melee;
    [Tooltip("Movement speed in units per second.")]
    [SerializeField] private float movementSpeed = 3.5f;
    [Tooltip("If true the enemy is currently in chase state.")]
    [SerializeField] private bool isChase = false;

    [Header("Patrol (horizontal only)")]
    [Tooltip("Enable horizontal patrol when not chasing.")]
    [SerializeField] private bool patrolEnabled = true;
    [Tooltip("Total distance (world units) the enemy patrols centered on start position.")]
    [SerializeField] private float patrolDistance = 4f;
    [Tooltip("Small threshold used to switch patrol direction.")]
    [SerializeField] private float patrolSwitchThreshold = 0.05f;

    [Header("Visual (rotate this, not whole GameObject)")]
    [Tooltip("Transform that contains visuals/sprite. Rotation applied here so trigger colliders are not affected.")]
    [SerializeField] private Transform visualRoot;

    // Public read-only accessors for other systems
    public int Health => health;
    public int MonsterId => monsterId;
    public MonsterType Type => monsterType;
    public float MovementSpeed => movementSpeed;
    public bool IsChase => isChase;

    // runtime
    private Transform chaseTarget;
    private Rigidbody2D rb2d;

    // patrol runtime
    private float patrolLeftX;
    private float patrolRightX;
    private int patrolDirection = 1; // 1 = moving right, -1 = moving left
    private Vector2 startPosition;

    // tracking facing state
    private bool facingRight = true;
    private const float MIN_DELTA_TO_CHANGE_FACING = 0.001f;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            rb2d = gameObject.AddComponent<Rigidbody2D>();
        }

        // prefer kinematic so physics won't unwantedly move the enemy
        rb2d.bodyType = RigidbodyType2D.Kinematic;
        rb2d.gravityScale = 0f;

        // find visual root if not assigned (prefer a child SpriteRenderer)
        if (visualRoot == null)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null && sr.transform != transform)
            {
                visualRoot = sr.transform;
            }
            else
            {
                // fallback: pick first child that isn't a trigger collider (safer than rotating root)
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    if (child.GetComponent<Collider2D>() == null)
                    {
                        visualRoot = child;
                        break;
                    }
                }

                // last fallback: use self (warn because rotating self may affect triggers)
                if (visualRoot == null) visualRoot = transform;
            }
        }

        // determine initial facing from visualRoot local Y rotation
        if (visualRoot != null)
        {
            float yaw = visualRoot.localEulerAngles.y;
            facingRight = Mathf.Abs(Mathf.DeltaAngle(yaw, 0f)) < 90f;
        }
        else
        {
            facingRight = true;
        }

        startPosition = transform.position;
        SetupPatrolPoints();

        // ensure initial visual rotation matches facingRight
        ApplyYRotation(facingRight);
    }

    private void SetupPatrolPoints()
    {
        if (!patrolEnabled || patrolDistance <= 0f)
        {
            patrolLeftX = startPosition.x;
            patrolRightX = startPosition.x;
            return;
        }

        float half = patrolDistance * 0.5f;
        patrolLeftX = startPosition.x - half;
        patrolRightX = startPosition.x + half;
        patrolDirection = 1;
    }

    void Update()
    {
        // If chasing, move horizontally toward player's x only
        if (isChase && chaseTarget != null)
        {
            float targetX = chaseTarget.position.x;
            float newX = Mathf.MoveTowards(transform.position.x, targetX, movementSpeed * Time.deltaTime);

            // flip based on movement direction
            float delta = newX - transform.position.x;
            UpdateFacingFromDelta(delta);

            Vector2 newPos = new Vector2(newX, transform.position.y);
            rb2d.MovePosition(newPos);
            return;
        }

        // Not chasing -> patrol (horizontal only)
        if (patrolEnabled)
        {
            PatrolMovement();
        }
    }

    private void PatrolMovement()
    {
        if (Mathf.Approximately(patrolLeftX, patrolRightX)) return;

        float targetX = (patrolDirection > 0) ? patrolRightX : patrolLeftX;
        float newX = Mathf.MoveTowards(transform.position.x, targetX, movementSpeed * Time.deltaTime);

        // flip based on movement direction (use sign of delta)
        float delta = newX - transform.position.x;
        UpdateFacingFromDelta(delta);

        Vector2 newPos = new Vector2(newX, transform.position.y);
        rb2d.MovePosition(newPos);

        if (Mathf.Abs(newX - targetX) <= patrolSwitchThreshold)
        {
            patrolDirection = -patrolDirection;
            // immediately apply rotation for new direction
            ApplyYRotation(patrolDirection > 0);
        }
    }

    private void UpdateFacingFromDelta(float delta)
    {
        if (Mathf.Abs(delta) < MIN_DELTA_TO_CHANGE_FACING) return; // ignore very small jitter
        bool shouldFaceRight = delta > 0f;
        ApplyYRotation(shouldFaceRight);
    }

    private void ApplyYRotation(bool faceRight)
    {
        if (visualRoot == null) return;
        if (facingRight == faceRight) return;
        facingRight = faceRight;

        // rotate only the visual root to avoid affecting physics/trigger children
        Vector3 local = visualRoot.localEulerAngles;
        local.y = faceRight ? 0f : 180f;
        visualRoot.localEulerAngles = local;
    }

    // Public API for external trigger to control chase state
    public void StartChase(Transform playerTransform)
    {
        // avoid redundant toggles
        if (isChase && chaseTarget == playerTransform) return;
        chaseTarget = playerTransform;
        isChase = true;
    }

    public void StopChase()
    {
        if (!isChase) return;
        chaseTarget = null;
        isChase = false;
    }

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        health = Mathf.Max(0, health - amount);
        if (health == 0) Die();
    }

    private void Die()
    {
        // TODO: handle death (animation, drop, pooling, etc.)
        gameObject.SetActive(false);
    }

    // Draw patrol line in editor when selected
    private void OnDrawGizmosSelected()
    {
        if (!patrolEnabled) return;

        Vector3 center = (Application.isPlaying) ? new Vector3((patrolLeftX + patrolRightX) * 0.5f, transform.position.y, transform.position.z)
                                                : transform.position;
        float half = patrolDistance * 0.5f;
        Vector3 left = new Vector3(center.x - half, center.y, center.z);
        Vector3 right = new Vector3(center.x + half, center.y, center.z);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(left, right);
        Gizmos.DrawSphere(left, 0.05f);
        Gizmos.DrawSphere(right, 0.05f);
    }
}
