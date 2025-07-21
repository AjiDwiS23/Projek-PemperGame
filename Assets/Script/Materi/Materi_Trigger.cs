using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Tambahkan ini jika menggunakan TextMeshPro

public class Materi_Trigger : MonoBehaviour
{
    [Header("Materi UI Integration")]
    [SerializeField] private MateriUI materiUI;      // Drag komponen MateriUI di Inspector
    [SerializeField] private int materiIndex = 0;    // Index materi yang ingin ditampilkan

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon; // Drag icon di Inspector

    [Header("Score Integration")]
    [SerializeField] private int scoreValue = 1; // Nilai skor yang didapat saat trigger

    [Header("Popup Text")]
    [SerializeField] private TMP_Text popupText; // Drag TMP_Text dari Canvas ke sini
    [SerializeField] private string popupMessage = "Materi berhasil dibuka, Tekan  B untuk melihatnya!"; // Pesan popup
    [SerializeField] private float popupDuration = 2f; // Durasi tampil popup

    private bool playerInTrigger = false;
    private bool hasTriggered = false; // Agar skor hanya bertambah sekali per trigger

    private string playerPrefsKey;

    private void Start()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false); // Pastikan icon tidak tampil di awal

        if (popupText != null)
            popupText.gameObject.SetActive(false); // Pastikan popup tidak tampil di awal

        // Use SceneManager to get the active scene name instead of Application.loadedLevelName
        playerPrefsKey = SceneManager.GetActiveScene().name + "_MateriTriggered_" + materiIndex;

        // Cek apakah sudah pernah trigger sebelumnya
        hasTriggered = PlayerPrefs.GetInt(playerPrefsKey, 0) == 1;
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E) && !hasTriggered)
        {
            
            if (materiUI != null)
            {
                materiUI.ShowMateri(materiIndex);
                materiUI.UnlockMateri(materiIndex);
            }

            // Tambahkan skor ke ScoreManager dan simpan otomatis di PlayerPrefs
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
            }

            hasTriggered = true;
            PlayerPrefs.SetInt(playerPrefsKey, 1); // Simpan status sudah trigger
            PlayerPrefs.Save();

            // Tampilkan popup
            if (popupText != null)
                StartCoroutine(ShowPopupCoroutine());
        }
    }

    private System.Collections.IEnumerator ShowPopupCoroutine()
    {
        popupText.text = popupMessage;
        popupText.gameObject.SetActive(true);
        yield return new WaitForSeconds(popupDuration);
        popupText.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            if (interactIcon != null && !hasTriggered)
                interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }
}