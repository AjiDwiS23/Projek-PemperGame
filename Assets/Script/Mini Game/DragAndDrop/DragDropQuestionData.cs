using UnityEngine;

[CreateAssetMenu(fileName = "DragDropQuestionData", menuName = "Quiz/DragDropQuestionData")]
public class DragDropQuestionData : ScriptableObject
{
    [TextArea]
    public string sentenceTemplate; // Contoh: "Kenapa aku {0} sadasds {1}"
    public string[] correctAnswers; // Jawaban benar untuk tiap slot
    public string[] answerChoices;  // Semua pilihan jawaban (acak)
    public AudioClip voiceOverClip; // Tambahkan ini untuk voice over
}
