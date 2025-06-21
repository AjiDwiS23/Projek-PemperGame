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

    public Question[] questions;
    private int currentQuestionIndex = 0;
    private float timer;
    private bool isAnswered = false;

    private ScoreManager scoreManager;

    void Start()
    {
        scoreManager = FindObjectOfType<ScoreManager>();
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
        isAnswered = true;
        if (index == questions[currentQuestionIndex].correctAnswerIndex)
        {
            scoreManager.AddScore(1); // Tambah skor jika benar
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
            // Quiz selesai, bisa tambahkan logika lain di sini
            questionText.text = "Quiz Selesai!";
            foreach (var btn in answerButtons)
                btn.gameObject.SetActive(false);
            timerText.gameObject.SetActive(false);
        }
    }
}
