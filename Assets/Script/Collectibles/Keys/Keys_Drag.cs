using UnityEngine;

public class Keys_Drag : MonoBehaviour
{
    [SerializeField] private DragAndDropManager dragAndDropManager;
    [SerializeField] private int questionIndex = 0; // Index soal DragAndDrop

    private bool hasTriggered = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Cek apakah kunci sudah pernah didapat di scene ini
        // Gunakan PlayerPrefs key yang sama dengan CurrencyManager
        string keyKey = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + "_PlayerKeys";
        int keys = PlayerPrefs.GetInt(keyKey, 0);
        if (keys > 0)
        {
            Destroy(gameObject); // Jangan munculkan lagi
        }
    }


    // Trigger mini game saat player masuk ke area trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            if (dragAndDropManager != null)
            {
                dragAndDropManager.SetQuestionByIndex(questionIndex); // Set soal sesuai index
                dragAndDropManager.StartMiniGame(OnMiniGameCompleted);
            }
        }
    }

    // Callback saat mini game selesai
    private void OnMiniGameCompleted()
    {
        CurrencyManager.Instance.AddKey(1);
        Debug.Log("Mini game selesai, kunci ditambahkan!");
        hasTriggered = false; // Reset jika ingin bisa di-trigger lagi
        Destroy(gameObject); // Hancurkan GameObject ini setelah selesai
    }
}
