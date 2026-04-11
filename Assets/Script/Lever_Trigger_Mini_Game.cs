using UnityEngine;

public class Lever_Trigger_Mini_Game : MonoBehaviour
{
    [Header("Platform Integration")]
    [SerializeField] private MovingPlatform platformToActivate; // Drag platform yang ingin diaktifkan

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon; // Drag icon di Inspector

    [Header("Lever Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign di Inspector
    [SerializeField] private Sprite defaultSprite;          // Assign di Inspector
    [SerializeField] private Sprite triggeredSprite;        // Assign di Inspector

    [Header("Input")]
    [Tooltip("Key used to activate the lever when player is in range.")]
    [SerializeField] private KeyCode activationKey = KeyCode.E;

    private bool hasTriggered = false;
    private bool playerInRange = false;

    private void Start()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false);

        SetDefaultSprite();

        // Daftarkan event platform
        if (platformToActivate != null)
            platformToActivate.OnReachedPointB += OnPlatformReachedPointB;
    }

    private void OnDestroy()
    {
        // Lepaskan event platform
        if (platformToActivate != null)
            platformToActivate.OnReachedPointB -= OnPlatformReachedPointB;
    }

    private void Update()
    {
        // Listen for activation key while player is in range
        if (playerInRange && !hasTriggered && Input.GetKeyDown(activationKey))
        {
            ActivateLever();
            hasTriggered = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = true;

        // show interact icon only if lever not yet triggered
        if (interactIcon != null && !hasTriggered)
            interactIcon.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        playerInRange = false;
        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    // Fungsi untuk mengaktifkan platform dan mengganti sprite ke triggered
    public void ActivateLever()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Lever");

        if (platformToActivate != null)
            platformToActivate.ActivatePlatform();

        SetTriggeredSprite();

        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    // Fungsi untuk mengatur sprite ke triggered
    public void SetTriggeredSprite()
    {
        if (spriteRenderer != null && triggeredSprite != null)
            spriteRenderer.sprite = triggeredSprite;
    }

    // Fungsi untuk mengatur sprite ke default
    public void SetDefaultSprite()
    {
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    private void OnPlatformReachedPointB()
    {
        // ketika platform mencapai titik B, kembalikan sprite ke default
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Lever");

        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;

        // allow re-triggering once platform completed if desired
        hasTriggered = false;
    }
}
