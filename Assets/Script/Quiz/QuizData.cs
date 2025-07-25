using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "Quiz/Quiz Data", order = 1)]
public class QuizData : ScriptableObject
{
    public string quizID; // Tambahkan ini, isi di Inspector
    public Sprite questionImage;
    public string question;
    public string[] answers = new string[4];
    public int correctAnswerIndex;
    [TextArea]
    public string explanation;
    public int scoreValue = 100; // Skor untuk quiz ini

    // Tambahkan field untuk voice over
    public AudioClip voiceOverClip;
}

