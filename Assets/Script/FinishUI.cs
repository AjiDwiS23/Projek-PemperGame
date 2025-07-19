using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class FinishUI : MonoBehaviour
{
    [SerializeField] private GameObject finishCanvas;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private int homeSceneIndex = 0;

    private Coroutine scoreCoroutine;

    private void Start()
    {
        if (finishCanvas != null)
            finishCanvas.SetActive(false);
    }

    public void ShowFinishUI()
    {
        if (finishCanvas != null)
            finishCanvas.SetActive(true);

        AudioManager.instance.Play("Win");
        int finalScore = 0;
        ScoreManager scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
            finalScore = scoreManager.CurrentScore;
        else
            finalScore = PlayerPrefs.GetInt("LastQuizScore", 0);

        // Debugging line to check the value retrieved from PlayerPrefs
        Debug.Log("Final Score from PlayerPrefs: " + finalScore);

        if (finalScoreText != null)
        {
            if (scoreCoroutine != null)
                StopCoroutine(scoreCoroutine);
            scoreCoroutine = StartCoroutine(AnimateScore(finalScore, 1f));
        }
    }

    private IEnumerator AnimateScore(int targetScore, float duration = 1f)
    {
        if (finalScoreText == null)
            yield break;

        float elapsed = 0f;
        int displayedScore = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, targetScore, t));
            finalScoreText.text = "Skor Akhir: " + displayedScore;
            yield return null;
        }
        finalScoreText.text = "Skor Akhir: " + targetScore;
    }

    public void NextLevelBtn()
    {
        SaveCurrentLevelPrefs();
        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextScene < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(nextScene);
        else
            Debug.Log("Sudah di level terakhir.");
    }

    public void HomeBtn()
    {
        SaveCurrentLevelPrefs();
        SceneManager.LoadScene(homeSceneIndex);
    }

    private void SaveCurrentLevelPrefs()
    {
        string levelKey = SceneManager.GetActiveScene().name;

        // Contoh: Ambil data dari ScoreManager, QuizUI, dsb
        int score = PlayerPrefs.GetInt(levelKey + "_Score", 0);
        int stars = PlayerPrefs.GetInt(levelKey + "_Stars", 0);
        int keys = PlayerPrefs.GetInt(levelKey + "_PlayerKeys", 0);

        // Jika ada data terbaru dari manager, update di sini
        ScoreManager scoreManager = Object.FindFirstObjectByType<ScoreManager>();
        if (scoreManager != null)
            score = scoreManager.CurrentScore;

        // Simpan ulang ke PlayerPrefs
        PlayerPrefs.SetInt(levelKey + "_Score", score);
        PlayerPrefs.SetInt(levelKey + "_Stars", stars);
        PlayerPrefs.SetInt(levelKey + "_PlayerKeys", keys);
        PlayerPrefs.Save();
    }
}
