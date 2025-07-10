using UnityEngine;
using TMPro;

public class DoorFinish : MonoBehaviour
{
    [Header("Key Requirement")]
    [SerializeField] private int requiredKeys = 3;

    [Header("Door Sprites")]
    [SerializeField] private SpriteRenderer doorRenderer;
    [SerializeField] private Sprite lockedSprite;
    [SerializeField] private Sprite unlockedSprite;

    [Header("Finish UI")]
    [SerializeField] private FinishUI finishUI; // Ubah ke tipe FinishUI

    [Header("Notification")]
    [SerializeField] private TextMeshProUGUI notifText;
    [SerializeField] private float notifDuration = 2f;

    private bool isUnlocked = false;

    private void Start()
    {
        UpdateDoorSprite();
        if (notifText != null)
            notifText.gameObject.SetActive(false);
    }

    private void UpdateDoorSprite()
    {
        int playerKeys = PlayerPrefs.GetInt("PlayerKeys", 0);
        isUnlocked = playerKeys >= requiredKeys;

        if (doorRenderer != null)
        {
            doorRenderer.sprite = isUnlocked ? unlockedSprite : lockedSprite;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UpdateDoorSprite();

            if (isUnlocked)
            {
                if (finishUI != null)
                    finishUI.ShowFinishUI();
            }
            else
            {
                ShowNotif("Kunci tidak cukup untuk membuka pintu!");
            }
        }
    }

    private void ShowNotif(string message)
    {
        if (notifText != null)
        {
            notifText.text = message;
            notifText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideNotif));
            Invoke(nameof(HideNotif), notifDuration);
        }
    }

    private void HideNotif()
    {
        if (notifText != null)
            notifText.gameObject.SetActive(false);
    }
}
