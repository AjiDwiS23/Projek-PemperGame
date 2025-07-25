using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quiz : MonoBehaviour
{
    [Header("UI References")]
    public Image quizImage;
    public TMP_Text questionText;
    public Button[] answerButtons;
    public TMP_Text[] answerTexts;
    public GameObject[] feedbackIcons; // 0: correct, 1: incorrect for each button
    public TMP_Text scoreText;
    public GameObject quizPanel;
    public GameObject resultPanel;
    public Image resultImage;
    public TMP_Text resultText;
    public Button closeResultButton;

    [Header("Quiz Data")]
    public QuizData quiz;

    public int scoreValue = 100;

    private int score = 0;
    private bool answered = false;

    public bool IsQuizCompleted { get; private set; } = false;

    public System.Action onResultClosed;
    public System.Action onCorrectAnswer;
    public System.Action onWrongAnswer;

    // Tambahkan reference ke Dialogue_Materi
    [Header("Dialogue")]
    public Dialogue_Materi dialogueMateri;
    public int dialogIndexCorrect = 2; // Index dialog untuk jawaban benar
    public int dialogIndexWrong = 3;   // Index dialog untuk jawaban salah

    // Tambahkan AudioSource untuk voice over quiz
    [Header("Audio")]
    public AudioSource audioSource;

    void SetupQuiz()
    {
        // Handle image
        if (quiz.questionImage != null)
        {
            quizImage.sprite = quiz.questionImage;
            quizImage.gameObject.SetActive(true);
        }
        else
        {
            quizImage.gameObject.SetActive(false);
        }

        // Set question and answers
        questionText.text = quiz.question;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerTexts[i].text = quiz.answers[i];
            int idx = i; // Capture index for listener
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(idx));
            feedbackIcons[i * 2].SetActive(false);     // correct icon
            feedbackIcons[i * 2 + 1].SetActive(false); // incorrect icon
            answerButtons[i].interactable = true;
        }

        answered = false;
        UpdateScore();
    }

    public void SetQuiz(QuizData newQuiz)
    {
        quiz = newQuiz;
        IsQuizCompleted = false;
        SetupQuiz();
    }

    void OnAnswerSelected(int index)
    {
        if (answered) return;
        answered = true;

        // Show feedback
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
            if (i == quiz.correctAnswerIndex)
                feedbackIcons[i * 2].SetActive(true); // correct
            else if (i == index)
                feedbackIcons[i * 2 + 1].SetActive(true); // incorrect
        }

        bool isCorrect = index == quiz.correctAnswerIndex;
        if (isCorrect)
        {
            score += quiz.scoreValue; // Gunakan variabel dari QuizData
            if (ScoreManager.Instance != null)
                ScoreManager.Instance.AddScore(quiz.scoreValue); // Gunakan variabel dari QuizData

            // Trigger dialog untuk jawaban benar
            if (dialogueMateri != null)
                dialogueMateri.ShowDialogByIndex(dialogIndexCorrect);
        }
        else
        {
            // Trigger dialog untuk jawaban salah
            if (dialogueMateri != null)
                dialogueMateri.ShowDialogByIndex(dialogIndexWrong);
        }

        UpdateScore();
        IsQuizCompleted = isCorrect;

        if (isCorrect)
            onCorrectAnswer?.Invoke();
        else
            onWrongAnswer?.Invoke();

        StartCoroutine(ShowResultWithDelay(isCorrect));
    }

    public IEnumerator ShowResultWithDelay(bool isCorrect)
    {
        yield return new WaitForSeconds(1.5f); // Jeda 1.5 detik

        if (isCorrect)
        {
            ShowResult(
                quiz.questionImage,
                quiz.explanation,
                OnResultPanelClosed
            );
        }
        else
        {
            // Jika salah, langsung tutup quiz panel
            HideQuiz();
        }
    }

    void UpdateScore()
    {
        scoreText.text = "+" + score.ToString();
    }

    public void ShowQuiz()
    {
        quizPanel.SetActive(true);

        // Mainkan voice over quiz jika ada
        if (audioSource != null)
        {
            audioSource.Stop();
            if (quiz != null && quiz.voiceOverClip != null)
            {
                audioSource.clip = quiz.voiceOverClip;
                audioSource.Play();
            }
        }

        // Tidak ada lagi pemanggilan ShowDialogByIndex(dialogIndex)
    }

    public void HideQuiz()
    {
        quizPanel.SetActive(false);

        // Stop voice over jika quiz ditutup
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void ShowResult(Sprite image, string text, System.Action onClose = null)
    {
        AudioManager.instance.Play("Checkpoint");
        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultImage != null)
            resultImage.sprite = image;

        if (resultText != null)
            resultText.text = text;

        if (closeResultButton != null)
        {
            closeResultButton.onClick.RemoveAllListeners();
            closeResultButton.onClick.AddListener(() => {
                resultPanel.SetActive(false);
                HideQuiz(); // Tutup quiz panel juga
                onClose?.Invoke();
                onResultClosed?.Invoke();
            });
        }
    }

    private void OnResultPanelClosed()
    {
        if (onResultClosed != null)
            onResultClosed.Invoke();
    }
}
