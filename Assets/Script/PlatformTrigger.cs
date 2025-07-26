using System.Xml.Linq;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private MovingPlatform platformToActivate;
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign di Inspector
    [SerializeField] private Sprite defaultSprite;          // Assign di Inspector
    [SerializeField] private Sprite triggeredSprite;        // Assign di Inspector

    [Header("Mini Game Integration")]
    [SerializeField] private Mini_Game_1 miniGameUI; // Drag Mini_Game_1 di Inspector
    [SerializeField] private MiniGameQuestionData questionData; // Drag asset MiniGameQuestionData di Inspector

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
        if (other.CompareTag("Player") && miniGameUI != null && questionData != null)
        {
            string miniGameKey = "MiniGameCompleted_" + questionData.miniGameID;
            bool isMiniGameCompleted = PlayerPrefs.GetInt(miniGameKey, 0) == 1;

            if (!isMiniGameCompleted)
            {
                miniGameUI.ShowMiniGame(questionData);
                miniGameUI.OnMiniGameCompleted += () =>
                {
                    PlayerPrefs.SetInt(miniGameKey, 1); // Tandai mini game ini sudah selesai
                    PlayerPrefs.Save();
                    ActivateLeverFromMiniGame();
                };
                return;
            }

            // Jika mini game sudah benar, lever bisa diaktifkan terus
            if (platformToActivate != null && isMiniGameCompleted)
            {
                ActivateLeverFromMiniGame();
            }
        }
    }

    private void OnPlatformReachedPointB()
    {
        AudioManager.instance.Play("Lever");
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    public void ActivateLeverFromMiniGame()
    {
        if (platformToActivate != null)
        {
            AudioManager.instance.Play("Lever");
            platformToActivate.ActivatePlatform();
            triggered = true;

            if (spriteRenderer != null && triggeredSprite != null)
                spriteRenderer.sprite = triggeredSprite;
        }
    }
}
