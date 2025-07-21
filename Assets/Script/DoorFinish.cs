using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Tambahkan ini

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
    private bool hasShownOpenNotif = false; // Tambahkan flag agar notif hanya muncul sekali
    private bool playerInTrigger = false; // Tambahkan flag ini

    private void Start()
    {
        UpdateDoorSprite();
        if (notifText != null)
            notifText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Selalu cek status kunci dan update sprite jika berubah
        bool prevUnlocked = isUnlocked;
        UpdateDoorSprite();
        // Reset notif jika status unlock berubah dari false ke true
        if (!prevUnlocked && isUnlocked)
            hasShownOpenNotif = false;

        // Cek input E jika player di trigger
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (isUnlocked)
            {
                // Tampilkan notif "Gerbang telah dibuka!" hanya sekali
                if (!hasShownOpenNotif)
                {
                    AudioManager.instance.Play("Lever");
                    ShowNotif("Gerbang telah dibuka!");
                    hasShownOpenNotif = true;
                }

                if (finishUI != null)
                    finishUI.ShowFinishUI();
            }
            else
            {
                ShowNotif("Kunci tidak cukup untuk membuka pintu!");
            }
        }
    }

    private void UpdateDoorSprite()
    {
        string keyKey = SceneManager.GetActiveScene().name + "_PlayerKeys";
        int playerKeys = PlayerPrefs.GetInt(keyKey, 0);
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
            playerInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
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
