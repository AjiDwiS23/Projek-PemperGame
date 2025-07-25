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

    public event System.Action OnMiniGameCompleted;

    // Tambahkan AudioSource untuk voice over
    [Header("Audio")]
    public AudioSource audioSource;

    // Tambahkan reference ke Dialogue_Materi
    [Header("Dialogue")]
    public Dialogue_Materi dialogueMateri;
    public int dialogIndexCorrect = 1;   // Index dialog untuk jawaban benar
    public int dialogIndexWrong = 2;     // Index dialog untuk jawaban salah

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

        // Set text jawaban pada button
        for (int i = 0; i < answerButtons.Length; i++)
            answerButtons[i].answerText = questionData.answerOptions[i];

        // Set text jawaban pada TMP_Text
        for (int i = 0; i < answerOptionTexts.Length; i++)
        {
            answerOptionTexts[i].text = questionData.answerOptions[i];
        }

        // Mainkan voice over soal (hanya satu, bukan array)
        if (audioSource != null)
        {
            audioSource.Stop();
            if (questionData.questionVoiceOver != null)
            {
                audioSource.clip = questionData.questionVoiceOver;
                audioSource.Play();
            }
        }

        // Kode dialogIndexSoal dihapus
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

        // Mainkan voice over benar/salah
        if (audioSource != null)
        {
            audioSource.Stop();
            // Tidak ada voice over correct/wrong di data, hanya stop audio
        }

        // Tampilkan dialog materi untuk benar/salah
        if (dialogueMateri != null)
        {
            if (correctCount == 3)
                dialogueMateri.ShowDialogByIndex(dialogIndexCorrect);
            else
                dialogueMateri.ShowDialogByIndex(dialogIndexWrong);
        }

        // Skor hanya bertambah jika semua benar
        if (correctCount == 3 && ScoreManager.Instance != null)
            ScoreManager.Instance.AddScore(score);

        StartCoroutine(HideIconsAfterDelay(iconShowDuration));
        if (correctCount == 3)
        {
            StartCoroutine(ShowNextUIAfterDelay(iconShowDuration, score));
        }
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
            AudioManager.instance.Play("Quiz_Finish");
            nextUI.SetActive(true);
            miniGame_Panel.SetActive(false);

            // Tampilkan skor di Next UI
            if (nextUIScoreText != null)
                nextUIScoreText.text = score.ToString();
        }
        // Panggil event di sini
        OnMiniGameCompleted?.Invoke();
    }
}
