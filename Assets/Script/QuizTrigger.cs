using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    [SerializeField] private GameObject UI_QuizStarted;
    [SerializeField] private GameObject quizUI;

    [Header("Quiz Sign")]
    [SerializeField] private SpriteRenderer signRenderer;
    [SerializeField] private Sprite completedSprite;

    [Header("Quiz Sign ID")]
    [SerializeField] private string signId = "Sign1"; // Tambahkan ini

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("OnTriggerEnter2D called by: " + other.name);
        if (!hasTriggered && other.CompareTag("Player"))
        {
            Debug.Log("Quiz Triggered!");
            hasTriggered = true;
            if (UI_QuizStarted != null)
            {
                UI_QuizStarted.SetActive(true);
                Debug.Log("UI_QuizStarted set active");
            }
            else
            {
                Debug.LogWarning("UI_QuizStarted is not assigned!");
            }
        }
    }

    // Method ini dipanggil oleh UI Button
    public void OnQuizButtonPressed()
    {
        Debug.Log("OnQuizButtonPressed called");
        if (!hasTriggered)
        {
            hasTriggered = true;
            if (UI_QuizStarted != null)
                UI_QuizStarted.SetActive(true);

            StartQuiz();    
        }
    }

    private void StartQuiz()
    {
        if (UI_QuizStarted != null) 
            UI_QuizStarted.SetActive(false);

        Debug.Log("StartQuiz called");
        if (quizUI != null)
        {
            quizUI.SetActive(true);
            Debug.Log("quizUI set active: " + quizUI.name);
        }
        else
        {
            Debug.LogWarning("quizUI is not assigned!");
        }

        QuizUI quizUIScript = quizUI != null ? quizUI.GetComponent<QuizUI>() : null;
        if (quizUIScript != null)
            quizUIScript.OnQuizCompleted += SetSignToCompleted;
        else
            Debug.LogWarning("QuizUI script not found on quizUI!");
    }

    private void SetSignToCompleted()
    {
        if (signRenderer != null && completedSprite != null)
            signRenderer.sprite = completedSprite;

        // Simpan status sign ke PlayerPrefs
        if (!string.IsNullOrEmpty(signId))
        {
            PlayerPrefs.SetInt("QuizSign_" + signId, 1); // 1 = completed
            PlayerPrefs.Save();
            Debug.Log($"Quiz sign {signId} marked as completed in PlayerPrefs.");
        }
    }

    // Optional: method untuk cek status sign dari PlayerPrefs
    public bool IsSignCompleted()
    {
        return PlayerPrefs.GetInt("QuizSign_" + signId, 0) == 1;
    }
}