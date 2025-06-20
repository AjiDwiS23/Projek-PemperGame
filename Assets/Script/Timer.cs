using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public static Timer Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI bestTimeText;
    private float currentTime = 0f;
    private float bestTime = Mathf.Infinity;
    private bool isRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadBestTime(); // Memuat waktu terbaik saat permainan dimulai
        UpdateTimerText();
        UpdateBestTimeText();
        StartTimer();
    }

    private void Update()
    {
        if (isRunning)
        {
            currentTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    public void StartTimer()
    {
        isRunning = true;
        currentTime = 0f;
    }

    public void StopTimer()
    {
        isRunning = false;

        if (currentTime < bestTime)
        {
            bestTime = currentTime;
            SaveBestTime(); // Simpan waktu terbaik baru
            UpdateBestTimeText();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"Time: {minutes:00}:{seconds:00}";
        }
    }

    private void UpdateBestTimeText()
    {
        if (bestTimeText != null)
        {
            if (bestTime < Mathf.Infinity)
            {
                int bestMinutes = Mathf.FloorToInt(bestTime / 60);
                int bestSeconds = Mathf.FloorToInt(bestTime % 60);
                bestTimeText.text = $"Best Time: {bestMinutes:00}:{bestSeconds:00}";
            }
            else
            {
                bestTimeText.text = "Best Time: N/A";
            }
        }
    }

    public void SaveBestTime()
    {
        PlayerPrefs.SetFloat("BestTime", bestTime);
        PlayerPrefs.Save();
        Debug.Log("Best Time saved: " + bestTime);
    }

    public void LoadBestTime()
    {
        bestTime = PlayerPrefs.GetFloat("BestTime", Mathf.Infinity);
        Debug.Log("Loaded Best Time: " + bestTime);
    }

    public float CurrentTime => currentTime;
    public float BestTime => bestTime;
}
