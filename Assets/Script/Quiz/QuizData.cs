using UnityEngine;

[CreateAssetMenu(fileName = "QuizData", menuName = "Quiz/Quiz Data", order = 1)]
public class QuizData : ScriptableObject
{
    public Sprite questionImage;
    public string question;
    public string[] answers = new string[4];
    public int correctAnswerIndex;
    [TextArea]
    public string explanation; // Tambahkan ini untuk penjelasan/uraian hasil
}

