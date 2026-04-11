using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class EnemyTrigger : MonoBehaviour
{
    [Tooltip("Reference to the Enemy to notify. If null, will try to find an Enemy in parent objects.")]
    [SerializeField] private Enemy enemy;

    [Header("Box Trigger Settings")]
    [Tooltip("Size of the BoxCollider2D trigger (local space).")]
    [SerializeField] private Vector2 triggerSize = new Vector2(3f, 3f);
    [Tooltip("Offset of the BoxCollider2D trigger (local space).")]
    [SerializeField] private Vector2 triggerOffset = Vector2.zero;
    [Tooltip("If true ensure collider is set to trigger on Awake/OnValidate.")]
    [SerializeField] private bool ensureIsTrigger = true;

    [Header("Gizmo")]
    [Tooltip("Draw gizmo for trigger area when selected.")]
    [SerializeField] private bool drawGizmo = true;
    [Tooltip("Gizmo color (semi-transparent recommended).")]
    [SerializeField] private Color gizmoColor = new Color(1f, 0f, 0f, 0.35f);

    private BoxCollider2D boxCol;

    // Track unique player root objects in the trigger to avoid enter/exit jitter
    private readonly HashSet<GameObject> playersInTrigger = new HashSet<GameObject>();

    private void Awake()
    {
        boxCol = GetComponent<BoxCollider2D>();
        if (boxCol == null)
        {
            boxCol = gameObject.AddComponent<BoxCollider2D>();
        }

        // Apply inspector values to the collider
        ApplyBoxProperties();

        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }
    }

    private void ApplyBoxProperties()
    {
        if (boxCol == null) return;
        boxCol.size = triggerSize;
        boxCol.offset = triggerOffset;
        if (ensureIsTrigger) boxCol.isTrigger = true;
    }

    private GameObject GetRootGameObject(Collider2D other)
    {
        // prefer attachedRigidbody root (common for player setups), fallback to transform.root
        if (other.attachedRigidbody != null)
            return other.attachedRigidbody.gameObject;
        return other.transform.root != null ? other.transform.root.gameObject : other.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var playerRoot = GetRootGameObject(other);
        // add to set; HashSet prevents duplicates from multiple colliders
        bool added = playersInTrigger.Add(playerRoot);

        if (added && playersInTrigger.Count == 1 && enemy != null)
        {
            // first player entered -> start chase
            enemy.StartChase(playerRoot.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        var playerRoot = GetRootGameObject(other);
        bool removed = playersInTrigger.Remove(playerRoot);

        if (removed && playersInTrigger.Count == 0 && enemy != null)
        {
            // last player left -> stop chase
            enemy.StopChase();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmo) return;

        // ensure we have a reference for drawing in edit mode
        if (boxCol == null) boxCol = GetComponent<BoxCollider2D>();
        if (boxCol == null)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireCube(transform.position, new Vector3(triggerSize.x, triggerSize.y, 0.01f));
            return;
        }

        Gizmos.color = gizmoColor;

        // local center transformed to world space
        Vector3 center = transform.TransformPoint(boxCol.offset);
        Vector3 size = new Vector3(boxCol.size.x * transform.lossyScale.x, boxCol.size.y * transform.lossyScale.y, 0.01f);
        Gizmos.DrawWireCube(center, size);

#if UNITY_EDITOR
        // semi-transparent fill for clarity (editor only)
        Color fill = gizmoColor;
        fill.a *= 0.15f;
        UnityEditor.Handles.color = fill;
        UnityEditor.Handles.DrawSolidRectangleWithOutline(
            new Vector3[] {
                center + new Vector3(-size.x * 0.5f, -size.y * 0.5f, 0f),
                center + new Vector3(size.x * 0.5f, -size.y * 0.5f, 0f),
                center + new Vector3(size.x * 0.5f, size.y * 0.5f, 0f),
                center + new Vector3(-size.x * 0.5f, size.y * 0.5f, 0f)
            },
            fill,
            gizmoColor
        );
#endif
    }

    // ensure collider stays a trigger and matches inspector size/offset in edit mode
    private void OnValidate()
    {
        // get or add box collider so inspector edits apply immediately
        boxCol = GetComponent<BoxCollider2D>();
        if (boxCol == null)
        {
            boxCol = gameObject.AddComponent<BoxCollider2D>();
        }

        // clamp sizes to positive
        triggerSize.x = Mathf.Max(0f, triggerSize.x);
        triggerSize.y = Mathf.Max(0f, triggerSize.y);

        ApplyBoxProperties();
    }
}
