using UnityEngine;

public class Key_MiniGam_1 : MonoBehaviour
{
    [Header("Mini Game Integration")]
    [SerializeField] private Mini_Game_1 miniGameUI; // Assign di Inspector
    [SerializeField] private MiniGameQuestionData questionData; // Assign di Inspector

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!hasTriggered && other.CompareTag("Player"))
        {
            hasTriggered = true;
            if (miniGameUI != null && questionData != null)
            {
                // Cek PlayerPrefs agar mini game hanya muncul sekali
                string miniGameKey = "MiniGameCompleted_" + questionData.miniGameID;
                bool isMiniGameCompleted = PlayerPrefs.GetInt(miniGameKey, 0) == 1;

                if (!isMiniGameCompleted)
                {
                    miniGameUI.ShowMiniGame(questionData);
                    miniGameUI.OnMiniGameCompleted += () =>
                    {
                        CurrencyManager.Instance.AddKey(1);
                        PlayerPrefs.SetInt(miniGameKey, 1);
                        PlayerPrefs.Save();
                        Destroy(gameObject); // Hancurkan collectible setelah selesai
                    };
                }
                else
                {
                    Destroy(gameObject); // Jika sudah selesai, langsung destroy
                }
            }
        }
    }
}
