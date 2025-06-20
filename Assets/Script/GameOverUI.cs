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
        gameOverPanel.SetActive(true);
        Time.timeScale = 0; // Pause the game
    }

    public void RestartGame()
    {
        Time.timeScale = 1; // Resume normal time scale
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitBtn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}