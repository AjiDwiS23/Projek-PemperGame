using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Finish : MonoBehaviour
{
    public int requiredScore = 12;
    [SerializeField] private TextMeshProUGUI notificationText;
    [SerializeField] private FinishUI finishUI;
    private float notificationDuration = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (ScoreManager.Instance != null && ScoreManager.Instance.CurrentScore >= requiredScore)
            {
                CompleteLevel();
            }
            else
            {
                ShowNotification("Kumpulkan skor minimal " + requiredScore + " untuk menyelesaikan level!");
            }
        }
    }

    private void CompleteLevel()
    {
        Debug.Log("Level Selesai!");
        if (finishUI != null)
        {
            FindFirstObjectByType<AudioManager>().Play("finish");
            finishUI.ShowFinishUI(); // Menampilkan UI selesai
            Timer.Instance.StopTimer();
            Timer.Instance.SaveBestTime();
        }
    }

    private void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            notificationText.gameObject.SetActive(true);
            Invoke("HideNotification", notificationDuration);
        }
    }

    private void HideNotification()
    {
        if (notificationText != null)
        {
            notificationText.gameObject.SetActive(false);
        }
    }
}
