using UnityEngine;

[CreateAssetMenu(fileName = "MiniGameQuestionData", menuName = "MiniGame/QuestionData")]
public class MiniGameQuestionData : ScriptableObject
{
    public Sprite[] questionImages; // Gambar yang akan ditebak
    public string[] answerOptions;  // Pilihan jawaban (3)
    public int[] correctAnswerIndices; // Index jawaban benar untuk setiap gambar
    public int scorePerCorrect = 1; // Skor per jawaban benar, bisa diatur di Inspector
}