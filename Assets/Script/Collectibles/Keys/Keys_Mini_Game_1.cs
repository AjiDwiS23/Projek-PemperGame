using UnityEngine;

public class Keys_Mini_Game_1 : MonoBehaviour
{
    private bool playerInTrigger = false;
    private bool keyGiven = false;

    [SerializeField] private DragAndDropV2Manager miniGameManager;
    [SerializeField] private int targetQuestionNumber = 1; // Nomor soal yang harus dijawab

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            // Cari soal otomatis dari manager
            if (miniGameManager != null && miniGameManager.allQuestions != null)
            {
                DragDropQuestionDataV2 foundQuestion = null;
                foreach (var q in miniGameManager.allQuestions)
                {
                    if (q != null && q.questionNumber == targetQuestionNumber)
                    {
                        foundQuestion = q;
                        break;
                    }
                }
                if (foundQuestion != null)
                {
                    miniGameManager.questionData = foundQuestion;
                    miniGameManager.ShowMiniGame();
                }
                else
                {
                    Debug.LogWarning("Soal dengan nomor " + targetQuestionNumber + " tidak ditemukan!");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
        }
    }

    private void OnEnable()
    {
        if (miniGameManager != null)
            miniGameManager.OnMiniGameCompleted += OnMiniGameCompleted;
    }

    private void OnDisable()
    {
        if (miniGameManager != null)
            miniGameManager.OnMiniGameCompleted -= OnMiniGameCompleted;
    }

    private void OnMiniGameCompleted(bool isCorrect)
    {
        if (playerInTrigger && isCorrect && !keyGiven && miniGameManager.questionData != null && miniGameManager.questionData.questionNumber == targetQuestionNumber)
        {
            keyGiven = true;
            // Tambahkan 1 kunci dan otomatis tersimpan di PlayerPrefs sesuai scene
            if (CurrencyManager.Instance != null)
                CurrencyManager.Instance.AddKey(1);

            gameObject.SetActive(false);
        }
    }
}