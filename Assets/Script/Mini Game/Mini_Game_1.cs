using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Mini_Game_1 : MonoBehaviour
{
    [SerializeField] public GameObject miniGame_Panel;
    public static Mini_Game_1 Instance;
    public MiniGameQuestionData questionData;
    public Image[] questionImageUIs;
    public DraggableAnswer[] answerButtons; // Assign di Inspector
    public AnswerSlot[] answerSlots;
    public Button jawabButton;
    public float iconShowDuration = 1.5f;
    public GameObject nextUI;
    public TMP_Text[] answerOptionTexts; // Assign di Inspector, urutkan sesuai button jawaban
    public TMP_Text nextUIScoreText; // Assign di Inspector ke Text Score di Next UI

    public event System.Action OnMiniGameCompleted;

    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Dialogue")]
    public Dialogue_Materi dialogueMateri;
    public int dialogIndexCorrect = 1;
    public int dialogIndexWrong = 2;

    void Awake()
    {
        Instance = this;
        jawabButton.gameObject.SetActive(false);
        jawabButton.onClick.AddListener(CheckAnswers);
    }

    void Start()
    {
        foreach (var slot in answerSlots)
            slot.HideResult();
    }

    void SetupQuestion()
    {
        // Set gambar soal
        for (int i = 0; i < questionImageUIs.Length; i++)
            questionImageUIs[i].sprite = questionData.questionImages[i];

        // Set text jawaban pada button dan TMP_Text
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].answerText = questionData.answerOptions[i];
            answerButtons[i].SetAnswerText(questionData.answerOptions[i]);
        }
        for (int i = 0; i < answerOptionTexts.Length; i++)
        {
            answerOptionTexts[i].text = questionData.answerOptions[i];
        }

        // Mainkan voice over soal
        if (audioSource != null)
        {
            audioSource.Stop();
            if (questionData.questionVoiceOver != null)
            {
                audioSource.clip = questionData.questionVoiceOver;
                audioSource.Play();
            }
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
            miniGame_Panel.SetActive(true);
        gameObject.SetActive(true);

        ResetUI();
        SetQuestion(newData);
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
        int correctCount = 0;
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
                    correctCount++;
                }
            }
            answerSlots[i].ShowResult(isCorrect);
        }
        Debug.Log("Score: " + score);

        if (audioSource != null)
        {
            audioSource.Stop();
        }

        if (dialogueMateri != null)
        {
            if (correctCount == 3)
                dialogueMateri.ShowDialogByIndex(dialogIndexCorrect);
            else
                dialogueMateri.ShowDialogByIndex(dialogIndexWrong);
        }

        if (correctCount == 3 && ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(score);

        StartCoroutine(HideIconsAfterDelay(iconShowDuration));
        if (correctCount == 3)
        {
            StartCoroutine(ShowNextUIAfterDelay(iconShowDuration, score));
        }
    }

    public void ResetUI()
    {
        // Kosongkan semua slot dan kembalikan jawaban ke AnswerPanel
        foreach (var slot in answerSlots)
        {
            // Kembalikan semua child DraggableAnswer ke AnswerPanel
            for (int i = slot.transform.childCount - 1; i >= 0; i--)
            {
                var child = slot.transform.GetChild(i);
                var answer = child.GetComponent<DraggableAnswer>();
                if (answer != null)
                {
                    answer.transform.SetParent(answer.originalParent, false);
                    answer.transform.localPosition = Vector3.zero;
                    answer.gameObject.SetActive(true);
                }
            }
            slot.currentAnswer = null;
            slot.HideResult();
        }

        // Pastikan semua jawaban kembali ke AnswerPanel dan aktif
        foreach (var answer in answerButtons)
        {
            answer.transform.SetParent(answer.originalParent, false);
            answer.transform.localPosition = Vector3.zero;
            answer.gameObject.SetActive(true);
        }

        jawabButton.gameObject.SetActive(false);

        if (nextUI != null)
            nextUI.SetActive(false);

        if (nextUIScoreText != null)
            nextUIScoreText.text = "";
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
            AudioManager.instance.Play("Quiz_Finish");
            nextUI.SetActive(true);
            miniGame_Panel.SetActive(false);

            if (nextUIScoreText != null)
                nextUIScoreText.text = score.ToString();
        }
        OnMiniGameCompleted?.Invoke();

        ResetUI();

        if (miniGame_Panel != null)
            miniGame_Panel.SetActive(false);
        gameObject.SetActive(false);
    }
}
