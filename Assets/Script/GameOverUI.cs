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
        string sceneName = SceneManager.GetActiveScene().name;
        string checkpointXKey = sceneName + "_CheckpointX";
        string checkpointYKey = sceneName + "_CheckpointY";
        string checkpointZKey = sceneName + "_CheckpointZ";
        string checkpointIDKey = sceneName + "_LastCheckpointID";

        // Cek apakah checkpoint tersimpan
        if (PlayerPrefs.HasKey(checkpointXKey) && PlayerPrefs.HasKey(checkpointYKey) && PlayerPrefs.HasKey(checkpointZKey))
        {
            float x = PlayerPrefs.GetFloat(checkpointXKey);
            float y = PlayerPrefs.GetFloat(checkpointYKey);
            float z = PlayerPrefs.GetFloat(checkpointZKey);
            int checkpointID = PlayerPrefs.GetInt(checkpointIDKey, 0);

            Debug.Log($"[RestartGame] Checkpoint Pos: ({x}, {y}, {z}), ID: {checkpointID}");

            // Simpan data checkpoint ke static agar bisa diakses setelah reload scene (opsional)
            // PermainanManager.Instance.SetCheckpoint(new Vector3(x, y, z), checkpointID);

            PlayerPrefs.Save(); // Pastikan data tersimpan sebelum reload
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            PlayerPrefs.Save(); // Pastikan data tersimpan sebelum reload
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void ExitBtn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}