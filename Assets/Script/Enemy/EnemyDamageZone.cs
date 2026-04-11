using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyDamageZone : MonoBehaviour
{
    private EnemyController_2D parentEnemy;
    private CircleCollider2D col;

    // track players currently inside the damage zone and next allowed damage time per root object
    private readonly Dictionary<GameObject, float> nextAllowedDamage = new Dictionary<GameObject, float>();

    // Initialize from parent enemy
    // If radius > 0 -> override collider radius.
    // If radius <= 0 -> keep whatever radius is set in Inspector.
    public void Init(EnemyController_2D enemy, float radius)
    {
        parentEnemy = enemy;

        col = GetComponent<CircleCollider2D>();
        if (col == null)
            col = gameObject.AddComponent<CircleCollider2D>();

        col.isTrigger = true;

        // Only override radius when parent explicitly provides a positive value.
        if (radius > 0f)
            col.radius = Mathf.Max(0f, radius);
    }

    public void SetRadius(float radius)
    {
        if (col != null && radius > 0f) col.radius = Mathf.Max(0f, radius);
    }

    private GameObject GetRootGameObject(Collider2D other)
    {
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.gameObject;
        return other.transform.root != null ? other.transform.root.gameObject : other.gameObject;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (parentEnemy == null) return;

        // Only consider objects on the configured playerLayer
        if (((1 << other.gameObject.layer) & parentEnemy.playerLayer) == 0) return;

        var root = GetRootGameObject(other);
        if (root == null) return;

        // add or reset timer so Update applies damage immediately (or on next cycle)
        nextAllowedDamage[root] = Time.time;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (parentEnemy == null) return;

        var root = GetRootGameObject(other);
        if (root == null) return;

        nextAllowedDamage.Remove(root);
    }

    void Update()
    {
        if (parentEnemy == null) return;

        float now = Time.time;
        // copy keys to avoid modifying collection while iterating
        var keys = new List<GameObject>(nextAllowedDamage.Keys);
        foreach (var root in keys)
        {
            if (root == null)
            {
                nextAllowedDamage.Remove(root);
                continue;
            }

            // ensure still on correct layer (layer might change)
            if (((1 << root.layer) & parentEnemy.playerLayer) == 0)
            {
                nextAllowedDamage.Remove(root);
                continue;
            }

            float allowedAt = nextAllowedDamage[root];
            if (now < allowedAt) continue;

            // attempt to damage
            var player = root.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // call player's TakeDamage — PlayerMovement will ignore if currently invincible
                player.TakeDamage(parentEnemy.damageToPlayer);

                // optionally force a temporary invincibility/invisibility window on the player
                if (parentEnemy.damageInvincibleDuration > 0f)
                {
                    player.SetTemporaryInvincible(parentEnemy.damageInvincibleDuration, true);
                }
            }

            // schedule next damage for this root
            float cd = Mathf.Max(0.01f, parentEnemy.damageCooldown);
            nextAllowedDamage[root] = now + cd;
        }
    }
}
