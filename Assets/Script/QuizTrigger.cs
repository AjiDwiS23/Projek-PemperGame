using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    [SerializeField] private GameObject UI_QuizStarted;
    [SerializeField] private GameObject quizUI;
    [SerializeField] private float showDuration = 2f;

    [Header("Quiz Sign")]
    [SerializeField] private SpriteRenderer signRenderer;
    [SerializeField] private Sprite completedSprite;

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
        if (!hasTriggered)
        {
            hasTriggered = true;
            if (UI_QuizStarted != null)
                UI_QuizStarted.SetActive(true);

            Invoke(nameof(StartQuiz), showDuration);
        }
    }

    private void StartQuiz()
    {
        if (UI_QuizStarted != null)
            UI_QuizStarted.SetActive(false);

        if (quizUI != null)
            quizUI.SetActive(true);

        QuizUI quizUIScript = quizUI.GetComponent<QuizUI>();
        if (quizUIScript != null)
            quizUIScript.OnQuizCompleted += SetSignToCompleted;
    }

    private void SetSignToCompleted()
    {
        if (signRenderer != null && completedSprite != null)
            signRenderer.sprite = completedSprite;
    }
}