using UnityEngine;
using TMPro;

public class LoadRecord : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI bestTimeText; // Tambahkan untuk menampilkan best time
    private float currentTime = 0f;
    private float bestTime = Mathf.Infinity; // Mulai dengan nilai yang sangat besa


    private void Start()
    {
        LoadBestTime(); // Memuat waktu terbaik saat permainan dimulai
    }


    private void UpdateBestTimeText()
    {
        if (bestTimeText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            bestTimeText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    private void SaveBestTime()
    {
        PlayerPrefs.SetFloat("BestTime", bestTime);
        PlayerPrefs.Save();
    }

    private void LoadBestTime()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity); // Memuat best time, default Infinity jika belum ada
        UpdateBestTimeText(); // Update UI best time saat dimuat
    }

    public float CurrentTime
    {
        get { return currentTime; }
    }

    public float BestTime
    {
        get { return bestTime; }
    }
}
