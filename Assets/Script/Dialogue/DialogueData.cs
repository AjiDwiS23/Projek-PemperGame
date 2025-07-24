using UnityEngine;

[CreateAssetMenu(fileName = "DialogueData", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string dialogId; // Tambahkan ID unik
    [TextArea]
    public string dialogText;
    public AudioClip voiceOverClip;
    public float hideDelay = 1.5f;
}