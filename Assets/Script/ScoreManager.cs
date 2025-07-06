using UnityEngine;
using TMPro;
using UnityEngine.UI; // Added this namespace for Image type

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

    public void AddScore(int value)
    {
        currentScore += value;
        UpdateScoreText();
        PlayerPrefs.SetInt("LastQuizScore", currentScore); // Simpan ke PlayerPrefs
        PlayerPrefs.Save();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString(); // Fixed the issue by using 'scoreText.text' instead of 'currentScore.text'  
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