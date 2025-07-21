using UnityEngine;

public class Lever_Trigger_Mini_Game : MonoBehaviour
{
    [Header("Mini Game Integration")]
    [SerializeField] private Mini_Game_1 miniGameUI; // Drag Mini_Game_1 di Inspector
    [SerializeField] private MiniGameQuestionData miniGameData; // Drag data soal mini game di Inspector
    [SerializeField] private MovingPlatform platformToActivate; // Drag platform yang ingin diaktifkan

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon; // Drag icon di Inspector

    [Header("Lever Sprite")]
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign di Inspector
    [SerializeField] private Sprite defaultSprite;          // Assign di Inspector
    [SerializeField] private Sprite triggeredSprite;        // Assign di Inspector

    private bool playerInTrigger = false;
    private bool hasTriggered = false;
    private bool triggered = false;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInTrigger = true;
            if (interactIcon != null)
                interactIcon.SetActive(true);

            // Langsung tampilkan mini game saat player masuk trigger
            if (miniGameUI != null && miniGameData != null)
            {
                string miniGameKey = "MiniGameCompleted_" + miniGameData.name;
                bool isMiniGameCompleted = PlayerPrefs.GetInt(miniGameKey, 0) == 1;

                if (!isMiniGameCompleted)
                {
                    miniGameUI.ShowMiniGame(miniGameData);
                    miniGameUI.OnMiniGameCompleted += () =>
                    {
                        PlayerPrefs.SetInt(miniGameKey, 1); // Tandai mini game ini sudah selesai
                        PlayerPrefs.Save();
                        ActivateLeverFromMiniGame();
                        miniGameUI.OnMiniGameCompleted -= ActivateLeverFromMiniGame;
                    };
                    return;
                }

                // Jika mini game sudah benar, lever bisa diaktifkan terus
                if (platformToActivate != null && isMiniGameCompleted)
                {
                    ActivateLeverFromMiniGame();
                }
            }
            hasTriggered = true;
            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            hasTriggered = false; // Reset agar lever bisa dipakai lagi
            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }

    // Fungsi untuk mengaktifkan platform dan mengganti sprite ke triggered
    public void ActivateLeverFromMiniGame()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.Play("Lever");

        if (platformToActivate != null)
            platformToActivate.ActivatePlatform();

        triggered = true;

        SetTriggeredSprite();
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
        // Jangan reset hasTriggered di sini!
        AudioManager.instance.Play("Lever");
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }
}
