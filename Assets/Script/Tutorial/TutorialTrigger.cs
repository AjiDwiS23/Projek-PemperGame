using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public Tutorial_1 tutorialController; // Drag Tutorial_1 di Inspector
    public int tutorialIndex = 0;         // Index TutorialData di array
    public int sentenceIndex = 0;         // Index kalimat/animasi yang ingin ditrigger

    private void Start()
    {
        // Cek jika tutorialController dan data valid
        if (tutorialController != null &&
            tutorialController.tutorialDataList != null &&
            tutorialIndex >= 0 &&
            tutorialIndex < tutorialController.tutorialDataList.Length)
        {
            var tutorialData = tutorialController.tutorialDataList[tutorialIndex];
            string tutorialID = tutorialData.tutorialID;
            string playerPrefKey = "Tutorial_Shown_" + tutorialID;

            if (PlayerPrefs.GetInt(playerPrefKey, 0) == 1)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            tutorialController.TriggerTutorialAtIndex(tutorialIndex, sentenceIndex);
            // Destroy(gameObject); // Hapus: destroy hanya di Start jika sudah pernah
        }
    }
}
