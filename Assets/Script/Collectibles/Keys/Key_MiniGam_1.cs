using UnityEngine;
using UnityEngine.SceneManagement;

public class Key_MiniGam_1 : MonoBehaviour
{
    [Header("Mini Game Integration")]
    [SerializeField] private Mini_Game_1 miniGameUI; // Assign di Inspector
    [SerializeField] private MiniGameQuestionData questionData; // Assign di Inspector

    private bool hasTriggered = false;

    private void Start()
    {
        if (questionData != null)
        {
            string miniGameKey = SceneManager.GetActiveScene().name + "_MiniGameCompleted_" + questionData.miniGameID;
            if (PlayerPrefs.HasKey(miniGameKey) && PlayerPrefs.GetInt(miniGameKey) == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            if (miniGameUI != null && questionData != null)
            {
                // Cek PlayerPrefs agar mini game hanya muncul sekali
                string miniGameKey = SceneManager.GetActiveScene().name + "_MiniGameCompleted_" + questionData.miniGameID;
                bool isMiniGameCompleted = PlayerPrefs.GetInt(miniGameKey, 0) == 1;

                if (!isMiniGameCompleted)
                {
                    miniGameUI.OnMiniGameCompleted -= HandleMiniGameCompleted;
                    miniGameUI.ShowMiniGame(questionData);
                    miniGameUI.OnMiniGameCompleted += HandleMiniGameCompleted;
                }
                else
                {
                    Destroy(gameObject); // Jika sudah selesai, langsung destroy
                }
            }
        }
    }

    private void HandleMiniGameCompleted()
    {
        string miniGameKey = SceneManager.GetActiveScene().name + "_MiniGameCompleted_" + questionData.miniGameID;
        CurrencyManager.Instance.AddKey(1);
        PlayerPrefs.SetInt(miniGameKey, 1);
        PlayerPrefs.Save();
        miniGameUI.OnMiniGameCompleted -= HandleMiniGameCompleted;
        Destroy(gameObject); // Hancurkan collectible setelah selesai
    }
}
