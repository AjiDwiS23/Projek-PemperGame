using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button quitButton;

    void Awake()
    {
        gameOverPanel.SetActive(false);
    }

    void Start()
    {
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowGameOverUI()
    {
        AudioManager.instance.Play("GameOver");
        gameOverPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void RestartGame()
    {
        // Ambil nama scene sebagai key prefix (jika Anda menyimpan per scene)
        string levelKey = SceneManager.GetActiveScene().name;

        // Ambil data dari PlayerPrefs
        int score = PlayerPrefs.GetInt(levelKey + "_Score", 0);
        int keys = PlayerPrefs.GetInt(levelKey + "_PlayerKeys", 0);
        int coins = PlayerPrefs.GetInt(levelKey + "_Coins", 0);
        string checkpoint = PlayerPrefs.GetString(levelKey + "_Checkpoint", "Belum Ada");

        // Tampilkan di konsol
        Debug.Log($"[RestartGame] Score: {score}, Kunci: {keys}, Coin: {coins}, Checkpoint: {checkpoint}");

        Time.timeScale = 1; // Resume normal time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitBtn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}