using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Mini_Game_1 : MonoBehaviour
{
    [SerializeField] private GameObject miniGame_Panel; // Assign di Inspector jika perlu
    public static Mini_Game_1 Instance;
    public MiniGameQuestionData questionData; // Assign di Inspector
    public Image[] questionImageUIs; // Assign UI Image untuk gambar soal
    public DraggableAnswer[] answerButtons; // Assign button jawaban
    public AnswerSlot[] answerSlots;
    public Button jawabButton;
    public float iconShowDuration = 1.5f;
    public GameObject nextUI;
    public TMP_Text[] answerOptionTexts; // Assign di Inspector, urutkan sesuai button jawaban
    public TMP_Text nextUIScoreText; // Assign di Inspector ke Text Score di Next UI

    void Awake()
    {
        Instance = this;
        jawabButton.gameObject.SetActive(false);
        jawabButton.onClick.AddListener(CheckAnswers);
    }

    void Start()
    {
        SetupQuestion();
        foreach (var slot in answerSlots)
            slot.HideResult();
    }

    void SetupQuestion()
    {
        // Set gambar soal
        for (int i = 0; i < questionImageUIs.Length; i++)
            questionImageUIs[i].sprite = questionData.questionImages[i];

        // Set text jawaban pada button
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].answerText = questionData.answerOptions[i];

        // Set text jawaban pada TMP_Text
        for (int i = 0; i < answerOptionTexts.Length; i++)
        {
            answerOptionTexts[i].text = questionData.answerOptions[i];
        }
    }

    public void SetQuestion(MiniGameQuestionData newData)
    {
        questionData = newData;
        ResetUI();
        SetupQuestion();
    }

    public void ShowWithQuestion(MiniGameQuestionData newData)
    {
        gameObject.SetActive(true);
        SetQuestion(newData);
    }

    public void ShowMiniGame(MiniGameQuestionData newData)
    {
        if (miniGame_Panel != null)
            miniGame_Panel.SetActive(true); // Tampilkan panel UI mini game

        SetQuestion(newData); // Set soal & reset UI
    }

    public void CheckAllSlotsFilled()
    {
        foreach (var slot in answerSlots)
        {
            if (slot.currentAnswer == null)
            {
                jawabButton.gameObject.SetActive(false);
                return;
            }
        }
        jawabButton.gameObject.SetActive(true);
    }

    void CheckAnswers()
    {
        int score = 0;
        bool allCorrect = true;
        for (int i = 0; i < answerSlots.Length; i++)
        {
            bool isCorrect = false;
            if (answerSlots[i].currentAnswer != null)
            {
                int selectedIndex = System.Array.IndexOf(questionData.answerOptions, answerSlots[i].currentAnswer.answerText);
                if (selectedIndex == questionData.correctAnswerIndices[i])
                {
                    score += questionData.scorePerCorrect;
                    isCorrect = true;
                }
                else
                {
                    allCorrect = false;
                }
            }
            else
            {
                allCorrect = false;
            }
            answerSlots[i].ShowResult(isCorrect);
        }
        Debug.Log("Score: " + score);

        // Skor hanya bertambah jika semua benar
        if (allCorrect && ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(score);

        StartCoroutine(HideIconsAfterDelay(iconShowDuration));
        if (allCorrect)
            StartCoroutine(ShowNextUIAfterDelay(iconShowDuration, score));
    }

    void ResetUI()
    {
        // Reset slot dan jawaban
        foreach (var slot in answerSlots)
        {
            if (slot.currentAnswer != null)
            {
                slot.currentAnswer.transform.SetParent(slot.currentAnswer.originalParent);
                slot.currentAnswer.transform.localPosition = Vector3.zero;
                slot.currentAnswer = null;
            }
            slot.HideResult();
        }
        jawabButton.gameObject.SetActive(false);
    }

    IEnumerator HideIconsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        foreach (var slot in answerSlots)
            slot.HideResult();
    }

    IEnumerator ShowNextUIAfterDelay(float delay, int score)
    {
        yield return new WaitForSeconds(delay);
        if (nextUI != null)
        {
            nextUI.SetActive(true);
            miniGame_Panel.SetActive(false);

            // Tampilkan skor di Next UI
            if (nextUIScoreText != null)
                nextUIScoreText.text = score.ToString();
        }
    }
}
