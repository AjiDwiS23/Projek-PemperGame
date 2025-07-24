using UnityEngine;

[CreateAssetMenu(fileName = "TutorialData", menuName = "Tutorial/Tutorial Data")]
public class TutorialData : ScriptableObject
{
    [Header("Tutorial ID")]
    public string tutorialID = "Tutorial_1";

    [Header("Tutorial Content")]
    [TextArea]
    public string[] tutorialSentences;

    [Header("Animation Clips")]
    public string[] animationClipNames = { "Tutorial_1", "Tutorial_1_2", "Tutorial_1_3", "Tutorial_1_4" };
    public int[] animationStartSentenceIndexes = { 0, 2, 3, 5 };

    [Header("Typewriter Settings")]
    public float typeSpeed = 0.05f;

    [Header("Voice Over")]
    public AudioClip[] voiceOvers;
}
