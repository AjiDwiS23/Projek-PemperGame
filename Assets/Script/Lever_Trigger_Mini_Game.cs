using UnityEngine;

public class Lever_Trigger_Mini_Game : MonoBehaviour
{
    [Header("Mini Game Integration")]
    [SerializeField] private Mini_Game_1 miniGameUI; // Drag Mini_Game_1 di Inspector
    [SerializeField] private MiniGameQuestionData miniGameData; // Drag data soal mini game di Inspector
    [SerializeField] private MovingPlatform platformToActivate; // Drag platform yang ingin diaktifkan

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon; // Drag icon di Inspector

    private bool playerInTrigger = false;
    private bool hasTriggered = false;

    private void Start()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false);
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E) && !hasTriggered)
        {
            if (miniGameUI != null && miniGameData != null)
            {
                miniGameUI.ShowMiniGame(miniGameData);
                // Subscribe ke event jawaban benar mini game
                StartCoroutine(WaitForMiniGameResult());
            }
            hasTriggered = true;
            if (interactIcon != null)
                interactIcon.SetActive(false);
        }
    }

    private System.Collections.IEnumerator WaitForMiniGameResult()
    {
        // Tunggu hingga panel nextUI aktif (berarti mini game selesai dan benar)
        while (miniGameUI.nextUI == null || !miniGameUI.nextUI.activeSelf)
            yield return null;

        // Aktifkan platform setelah mini game selesai dan benar
        if (platformToActivate != null)
            platformToActivate.ActivatePlatform();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            playerInTrigger = true;
            if (interactIcon != null)
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
