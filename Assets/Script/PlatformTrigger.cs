using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private MovingPlatform platformToActivate;
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign di Inspector
    [SerializeField] private Sprite defaultSprite;          // Assign di Inspector
    [SerializeField] private Sprite triggeredSprite;        // Assign di Inspector

    private bool triggered = false;

    private void Start()
    {
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;

        if (platformToActivate != null)
            platformToActivate.OnReachedPointB += OnPlatformReachedPointB;
    }

    private void OnDestroy()
    {
        if (platformToActivate != null)
            platformToActivate.OnReachedPointB -= OnPlatformReachedPointB;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            platformToActivate.ActivatePlatform();
            triggered = true;

            // Ganti sprite jika sudah di-trigger
            if (spriteRenderer != null && triggeredSprite != null)
                spriteRenderer.sprite = triggeredSprite;
        }
    }

    private void OnPlatformReachedPointB()
    {
        triggered = false;
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }
}
