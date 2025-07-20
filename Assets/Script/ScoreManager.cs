using UnityEngine;
using TMPro;
using UnityEngine.UI; // Added this namespace for Image type
using UnityEngine.SceneManagement; // Tambahkan ini

[System.Serializable]
public class Question
{
    public string questionText;
    public string[] choices = new string[4];
    public int correctAnswerIndex;
    public int scoreValue = 1; // Nilai default 1, bisa diubah di Inspector
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int currentScore = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ambil skor terakhir dari PlayerPrefs jika ada
        string stageKey = SceneManager.GetActiveScene().name + "_FinalScore";
        if (PlayerPrefs.HasKey(stageKey))
        {
            currentScore = PlayerPrefs.GetInt(stageKey);
        }
        else
        {
            currentScore = 0;
        }
        UpdateScoreText();
    }

    public void AddScore(int value)
    {
        currentScore += value;
        UpdateScoreText();

        // Simpan skor akhir berdasarkan nama stage/scene
        string stageKey = SceneManager.GetActiveScene().name + "_FinalScore";
        PlayerPrefs.SetInt(stageKey, currentScore);
        PlayerPrefs.Save();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }
    public int CurrentScore
    {
        get { return currentScore; }
    }

    public void CheckAnswer(int index, int correctIndex, Image[] questionResultIcons, int currentQuestionIndex, Sprite correctSprite, Question[] questions)
    {
        if (index == correctIndex)
        {
            AddScore(questions[currentQuestionIndex].scoreValue);
            questionResultIcons[currentQuestionIndex].sprite = correctSprite;
        }
    }
}