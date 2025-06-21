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

    [SerializeField] GameObject quizEndPanel; // Assign di Inspector

    [SerializeField] Question[] questions;
    private int currentQuestionIndex = 0;
    private float timer;
    private bool isAnswered = false;

    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        // Reset semua indikator ke default  
        for (int i = 0; i < questionResultIcons.Length; i++)
            questionResultIcons[i].sprite = defaultSprite;
        timer = timePerQuestion; // Timer hanya direset sekali di awal  
        ShowQuestion();
        if (scorePopupText != null)
            scorePopupText.gameObject.SetActive(false);
        if (quizEndPanel != null)
            quizEndPanel.SetActive(false);
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
        // timer = timePerQuestion; // HAPUS/COMMENT baris ini agar timer tidak direset  

        // Update nomor soal (index mulai dari 0, jadi +1)  
        questionNumberText.text = $"Soal No {currentQuestionIndex + 1}";

        Question q = questions[currentQuestionIndex];
        questionText.text = q.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int index = i; // Capture index for the lambda  
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = q.choices[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    void OnAnswerSelected(int index)
    {
        // Cegah error jika soal sudah habis atau index tidak valid  
        if (currentQuestionIndex >= questions.Length || index < 0 || index >= answerButtons.Length)
            return;

        isAnswered = true;

        int correctIndex = questions[currentQuestionIndex].correctAnswerIndex;
        if (index == correctIndex)
        {
            int value = questions[currentQuestionIndex].scoreValue;
            scoreManager.AddScore(value);
            questionResultIcons[currentQuestionIndex].sprite = correctSprite;
            ShowScorePopup(value);
        }
        else
        {
            questionResultIcons[currentQuestionIndex].sprite = wrongSprite;
        }

        // Disable tombol agar tidak bisa klik lagi  
        foreach (var btn in answerButtons)
            btn.interactable = false;

        // Lanjut ke soal berikutnya setelah delay  
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
        if (currentQuestionIndex < questions.Length)
        {
            ShowQuestion();
        }
        else
        {
            // Quiz selesai, disable semua tombol  
            questionText.text = "Quiz Selesai!";
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);

            // Tampilkan panel/tombol selesai
            if (quizEndPanel != null)
                quizEndPanel.SetActive(true);
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
}
