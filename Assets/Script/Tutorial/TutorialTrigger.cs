using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public Tutorial_1 tutorialController; // Drag Tutorial_1 di Inspector
    public int tutorialIndex = 0;         // Index TutorialData di array
    public int sentenceIndex = 0;         // Index kalimat/animasi yang ingin ditrigger

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialController.TriggerTutorialAtIndex(tutorialIndex, sentenceIndex);
            Destroy(gameObject); // Hapus trigger dari scene setelah dipakai
        }
    }
}
