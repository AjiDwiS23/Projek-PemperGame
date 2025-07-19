using System.Xml.Linq;
using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    [SerializeField] private MovingPlatform platformToActivate;
    [SerializeField] private SpriteRenderer spriteRenderer; // Assign di Inspector
    [SerializeField] private Sprite defaultSprite;          // Assign di Inspector
    [SerializeField] private Sprite triggeredSprite;        // Assign di Inspector
    [SerializeField] private Quiz quizUI;         // Drag komponen Quiz di Inspector
    [SerializeField] private QuizData quizData;   // Drag asset QuizData di Inspector

    private bool triggered = false;

    private void Start()
    {
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;

        if (platformToActivate != null)
            platformToActivate.OnReachedPointB += OnPlatformReachedPointB;
    }

    private void OnDestroy()
    {
        if (platformToActivate != null)
            platformToActivate.OnReachedPointB -= OnPlatformReachedPointB;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && quizUI != null && quizData != null)
        {
            if (!quizUI.IsQuizCompleted)
            {
                quizUI.SetQuiz(quizData);
                quizUI.ShowQuiz();
                quizUI.onCorrectAnswer = ActivateLeverFromQuiz;
                quizUI.onWrongAnswer = () =>
                {
                    // Quiz bisa diulang, tidak perlu lakukan apa-apa di sini
                };
                return;
            }

            // Jika quiz sudah benar, lever bisa diaktifkan terus
            if (platformToActivate != null && quizUI.IsQuizCompleted)
            {
                ActivateLeverFromQuiz();
            }
        }
    }

    private void OnPlatformReachedPointB()
    {
        // Jangan reset triggered di sini!
        AudioManager.instance.Play("Lever");
        if (spriteRenderer != null && defaultSprite != null)
            spriteRenderer.sprite = defaultSprite;
    }

    public void ActivateLeverFromQuiz()
    {
        if (platformToActivate != null)
        {
            AudioManager.instance.Play("Lever");
            platformToActivate.ActivatePlatform();
            triggered = true;

            if (spriteRenderer != null && triggeredSprite != null)
                spriteRenderer.sprite = triggeredSprite;
        }
    }
}
