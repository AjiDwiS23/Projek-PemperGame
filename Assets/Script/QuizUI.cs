using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

// Explicitly qualify 'Object' with 'UnityEngine.Object' to resolve ambiguity
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

    [SerializeField] TextMeshProUGUI keyCountText; // Tambahkan di field

    [SerializeField] Question[] questions;
    private Question[] selectedQuestions; // Soal yang akan dimainkan (maksimal 3)
    private int maxQuestions = 3;         // Jumlah soal yang harus dijawab
    private int currentQuestionIndex = 0;
    private float timer;
    private bool isAnswered = false;

    private ScoreManager scoreManager;
    private int correctAnswersCount = 0; // Tambahkan ini
    private int tempScore = 0; // Skor sementara

    public event Action OnQuizCompleted; // Tambahkan ini

    void Start()
    {
        // Explicitly qualify 'Object' with 'UnityEngine.Object'
        scoreManager = UnityEngine.Object.FindFirstObjectByType<ScoreManager>();

        // Pilih soal acak sebanyak maxQuestions
        int total = Mathf.Min(maxQuestions, questions.Length);
        selectedQuestions = new Question[total];
        var indices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < questions.Length; i++) indices.Add(i);
        for (int i = 0; i < total; i++)
        {
            int rand = UnityEngine.Random.Range(0, indices.Count); // Explicitly qualify 'Random'
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
            tempScore += value; // Skor sementara
            questionResultIcons[currentQuestionIndex].sprite = correctSprite;
            ShowScorePopup(value);

            correctAnswersCount++;

            if (AudioManager.instance != null)
                AudioManager.instance.Play("Correct");
        }
        else
        {
            questionResultIcons[currentQuestionIndex].sprite = wrongSprite;

            if (AudioManager.instance != null)
                AudioManager.instance.Play("Wrong");
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
                finalScoreText.text = tempScore.ToString();

            ShowStars(tempScore);

            // Tambahkan logika pemberian kunci di sini
            if (correctAnswersCount >= 3)
            {
                // Contoh: simpan jumlah kunci di PlayerPrefs
                int currentKeys = PlayerPrefs.GetInt("PlayerKeys", 0);
                PlayerPrefs.SetInt("PlayerKeys", currentKeys + 1);
                PlayerPrefs.Save();

                Debug.Log("Selamat! Anda mendapatkan 1 kunci karena menjawab minimal 3 soal dengan benar.");

                CurrencyManager.Instance.AddKey(1); // Tambahkan ini
            }

            UpdateKeyUI(); // Tambahkan ini untuk memperbarui UI kunci

            // Play quiz finish sound effect
            if (AudioManager.instance != null)
                AudioManager.instance.Play("Quiz_Finish");

            // Gabungkan skor quiz ke skor utama
            if (scoreManager != null)
                scoreManager.AddScore(tempScore);

            tempScore = 0; // Reset skor sementara

            // Tambahkan trigger event
            OnQuizCompleted?.Invoke();
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

    void UpdateKeyUI()
    {
        if (keyCountText != null)
        {
            int keyCount = PlayerPrefs.GetInt("PlayerKeys", 0);
            keyCountText.text = keyCount.ToString();
        }
    }

    public void ResetQuiz()
    {
        tempScore = 0;
        correctAnswersCount = 0;
        currentQuestionIndex = 0;
        isAnswered = false;
        timer = timePerQuestion;

        // Pilih soal acak sebanyak maxQuestions
        int total = Mathf.Min(maxQuestions, questions.Length);
        selectedQuestions = new Question[total];
        var indices = new System.Collections.Generic.List<int>();
        for (int i = 0; i < questions.Length; i++) indices.Add(i);
        for (int i = 0; i < total; i++)
        {
            int rand = UnityEngine.Random.Range(0, indices.Count);
            selectedQuestions[i] = questions[indices[rand]];
            indices.RemoveAt(rand);
        }

        // Reset indikator
        for (int i = 0; i < questionResultIcons.Length; i++)
            questionResultIcons[i].sprite = defaultSprite;

        // Aktifkan kembali UI yang perlu
        foreach (var btn in answerButtons)
            btn.gameObject.SetActive(true);
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        if (scorePopupText != null)
            scorePopupText.gameObject.SetActive(false);
        if (scoreCalculationPanel != null)
            scoreCalculationPanel.SetActive(false);

        ShowQuestion();
    }

    void OnEnable()
    {
        ResetQuiz();
    }
}
