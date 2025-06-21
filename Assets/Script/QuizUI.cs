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
    }

    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI timerText;
    public float timePerQuestion = 10f;

    public Image[] questionResultIcons; // Assign di Inspector, urut sesuai urutan soal
    public Sprite correctSprite; // Sprite checklist
    public Sprite wrongSprite; // Sprite silang
    public Sprite defaultSprite; // Sprite default (kosong)

    public Question[] questions;
    private int currentQuestionIndex = 0;
    private float timer;
    private bool isAnswered = false;

    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
        // Reset semua indikator ke default
        for (int i = 0; i < questionResultIcons.Length; i++)
            questionResultIcons[i].sprite = defaultSprite;
        ShowQuestion();
    }

    void Update()
    {
        if (!isAnswered)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

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
        timer = timePerQuestion;

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
            scoreManager.AddScore(1);
            questionResultIcons[currentQuestionIndex].sprite = correctSprite;
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
        }
    }
}
