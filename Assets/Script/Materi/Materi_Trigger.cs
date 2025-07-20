using UnityEngine;

public class Materi_Trigger : MonoBehaviour
{
    [Header("Materi UI Integration")]
    [SerializeField] private MateriUI materiUI;      // Drag komponen MateriUI di Inspector
    [SerializeField] private int materiIndex = 0;    // Index materi yang ingin ditampilkan

    [Header("Interaction Icon")]
    [SerializeField] private GameObject interactIcon; // Drag icon di Inspector

    private bool playerInTrigger = false;

    private void Start()
    {
        if (interactIcon != null)
            interactIcon.SetActive(false); // Pastikan icon tidak tampil di awal
    }

    private void Update()
    {
        if (playerInTrigger && Input.GetKeyDown(KeyCode.E))
        {
            if (materiUI != null)
            {
                materiUI.ShowMateri(materiIndex);
                materiUI.UnlockMateri(materiIndex);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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