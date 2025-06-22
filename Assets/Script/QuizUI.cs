using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizUI : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] choices = new string[4];
        public int correctAnswerIndex;
        public int scoreValue = 1; // Nilai default 1, bisa diubah di Inspector
    }

    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] Button[] answerButtons;
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float timePerQuestion = 10f;

    [SerializeField] Image[] questionResultIcons; // Assign di Inspector, urut sesuai urutan soal  
    [SerializeField] Sprite correctSprite; // Sprite checklist  
    [SerializeField] Sprite wrongSprite; // Sprite silang  
    [SerializeField] Sprite defaultSprite; // Sprite default (kosong)  

    [SerializeField] TextMeshProUGUI questionNumberText; // Assign di Inspector  

    [SerializeField] TextMeshProUGUI scorePopupText; // Assign di Inspector
    [SerializeField] float scorePopupDuration = 1.0f; // Durasi tampil (detik)

    [SerializeField] GameObject scoreCalculationPanel; // Assign di Inspector

    [SerializeField] TextMeshProUGUI finalScoreText; // Assign di Inspector (untuk skor akhir)
    [SerializeField] Image[] starImages;             // Assign di Inspector (3 bintang)
    [SerializeField] Sprite starOnSprite;            // Sprite bintang aktif
    [SerializeField] Sprite starOffSprite;           // Sprite bintang non-aktif

    [SerializeField] Question[] questions;
    private Question[] selectedQuestions; // Soal yang akan dimainkan (maksimal 3)
    private int maxQuestions = 3;         // Jumlah soal yang harus dijawab
    private int currentQuestionIndex = 0;
    private float timer;
    private bool isAnswered = false;

    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();

        // Pilih soal acak sebanyak maxQuestions
        int total = Mathf.Min(maxQuestions, questions.Length);
        selectedQuestions = new Question[total];
        var indices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < questions.Length; i++) indices.Add(i);
        for (int i = 0; i < total; i++)
        {
            int rand = Random.Range(0, indices.Count);
            selectedQuestions[i] = questions[indices[rand]];
            indices.RemoveAt(rand);
        }

        // Reset semua indikator ke default  
        for (int i = 0; i < questionResultIcons.Length; i++)
            questionResultIcons[i].sprite = defaultSprite;
        timer = timePerQuestion;
        ShowQuestion();
        if (scorePopupText != null)
            scorePopupText.gameObject.SetActive(false);
        if (scoreCalculationPanel != null)
            scoreCalculationPanel.SetActive(false);
    }

    void Update()
    {
        if (!isAnswered)
        {
            timer -= Time.deltaTime;
            int timeLeft = Mathf.CeilToInt(timer);
            timerText.text = timeLeft.ToString();

            // Ubah warna timer sesuai sisa waktu  
            if (timeLeft <= 10 && timeLeft > 0)
            {
                timerText.color = Color.red;
            }
            else if (timeLeft <= 20 && timeLeft >= 11)
            {
                timerText.color = Color.yellow;
            }
            else
            {
                timerText.color = Color.white;
            }

            if (timer <= 0)
            {
                isAnswered = true;
                NextQuestion();
            }
        }
    }

    void ShowQuestion()
    {
        isAnswered = false;
        questionNumberText.text = $"Soal No {currentQuestionIndex + 1}";
        Question q = selectedQuestions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i;
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.choices[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        if (currentQuestionIndex >= selectedQuestions.Length || index < 0 || index >= answerButtons.Length)
            return;

        isAnswered = true;

        int correctIndex = selectedQuestions[currentQuestionIndex].correctAnswerIndex;
        if (index == correctIndex)
        {
            int value = selectedQuestions[currentQuestionIndex].scoreValue;
            scoreManager.AddScore(value);
            questionResultIcons[currentQuestionIndex].sprite = correctSprite;
            ShowScorePopup(value);
        }
        else
        {
            questionResultIcons[currentQuestionIndex].sprite = wrongSprite;
        }

        foreach (var btn in answerButtons)
            btn.interactable = false;

        Invoke(nameof(NextQuestionWithReset), 1.2f);
    }

    void NextQuestionWithReset()
    {
        // Aktifkan kembali tombol jawaban  
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;
        }
        NextQuestion();
    }

    void NextQuestion()
    {
        currentQuestionIndex++;
        if (currentQuestionIndex < selectedQuestions.Length)
        {
            ShowQuestion();
        }
        else
        {
            // Quiz selesai, tampilkan hasil
            questionText.text = "Quiz Selesai!";
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);

            if (scoreCalculationPanel != null)
                scoreCalculationPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = scoreManager.CurrentScore.ToString();

            ShowStars(scoreManager.CurrentScore);
        }
    }

    void ShowStars(int score)
    {
        int starCount = 0;
        if (score >= 2000)
            starCount = 3;
        else if (score >= 1500)
            starCount = 2;
        else if (score >= 1000)
            starCount = 1;

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
                starImages[i].sprite = (i < starCount) ? starOnSprite : starOffSprite;
        }
    }

    void ShowScorePopup(int value)
    {
        if (scorePopupText != null)
        {
            scorePopupText.text = $"+{value}";
            scorePopupText.gameObject.SetActive(true);
            CancelInvoke(nameof(HideScorePopup));
            Invoke(nameof(HideScorePopup), scorePopupDuration);
        }
    }

    void HideScorePopup()
    {
        if (scorePopupText != null)
            scorePopupText.gameObject.SetActive(false);
    }

    public void SaveScoreAndStars()
    {
        int score = scoreManager.CurrentScore;
        int starCount = 0;
        if (score >= 2000)
            starCount = 3;
        else if (score >= 1500)
            starCount = 2;
        else if (score >= 1000)
            starCount = 1;

        PlayerPrefs.SetInt("LastQuizScore", score);
        PlayerPrefs.SetInt("LastQuizStars", starCount);
        PlayerPrefs.Save();
        gameObject.SetActive(false); // Menyembunyikan QuizUI setelah menyimpan

        Debug.Log($"Score ({score}) dan Bintang ({starCount}) telah disimpan.");
    }
}
