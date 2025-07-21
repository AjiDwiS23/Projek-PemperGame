using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using UnityEngine.UI; // Add this for Image

public class FinishUI : MonoBehaviour
{
    [SerializeField] private GameObject finishCanvas;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private int homeSceneIndex = 0;

    [Header("Star UI")]
    [SerializeField] private Image[] starImages; // Assign 3 star Image objects in Inspector
    [SerializeField] private Sprite starClosedSprite; // Sprite for closed star
    [SerializeField] private Sprite starOpenSprite;   // Sprite for open star

    private Coroutine scoreCoroutine;

    private void Start()
    {
        if (finishCanvas != null)
            finishCanvas.SetActive(false);

        // Optional: Ensure all stars are closed at start
        SetStars(0);
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

        Debug.Log("Final Score from PlayerPrefs: " + finalScore);

        // Set stars based on score
        int starCount = CalculateStars(finalScore);
        SetStars(starCount);

        if (finalScoreText != null)
        {
            if (scoreCoroutine != null)
                StopCoroutine(scoreCoroutine);
            scoreCoroutine = StartCoroutine(AnimateScore(finalScore, 1f));
        }
    }

    private int CalculateStars(int score)
    {
        if (score >= 1500) return 3;
        if (score >= 1000) return 2;
        if (score >= 500) return 1;
        return 0;
    }

    private void SetStars(int count)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
                starImages[i].sprite = (i < count) ? starOpenSprite : starClosedSprite;
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
