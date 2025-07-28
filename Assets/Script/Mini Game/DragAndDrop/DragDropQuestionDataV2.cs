using UnityEngine;

[CreateAssetMenu(fileName = "DragDropQuestionDataV2", menuName = "MiniGame/DragDropQuestionDataV2")]
public class DragDropQuestionDataV2 : ScriptableObject
{
    [TextArea]
    public string questionText;

    [Tooltip("List of answer choices for this question.")]
    public string[] answerChoices = new string[3];

    [Tooltip("Index of the correct answer in the answerChoices array.")]
    public int correctAnswerIndex;

    [Header("Additional Data")]
    [Tooltip("Image for the question.")]
    public Sprite questionImage;

    [Tooltip("Voice over audio for the question.")]
    public AudioClip voiceOverClip;

    [Header("Question Number")]
    [Tooltip("Number of this question.")]
    public int questionNumber;

    [Header("Score Reward")]
    [Tooltip("Score yang didapat jika menjawab benar.")]
    public int scoreReward = 500;
}