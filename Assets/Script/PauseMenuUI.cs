using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button muteButton; // Tambahkan ini
    [SerializeField] private Image muteIcon;    // (Optional) assign icon jika ingin mengganti gambar saat mute/unmute
    [SerializeField] private Sprite mutedSprite; // (Optional) icon mute
    [SerializeField] private Sprite unmutedSprite; // (Optional) icon unmute

    private bool isPaused = false;
   //rivate bool isBGMMuted = false;

    private void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

        if (exitButton != null)
            exitButton.onClick.AddListener(ExitGame);

        if (muteButton != null)
            muteButton.onClick.AddListener(ToggleMuteBGM);

        // Sync icon with current BGM state
        UpdateMuteIcon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                PauseGame();
            else
                ResumeGame();
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        if (pausePanel != null)
            pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        string levelKey = SceneManager.GetActiveScene().name;

        // Hapus semua data PlayerPrefs terkait scene ini
        PlayerPrefs.DeleteKey(levelKey + "_FinalScore");
        PlayerPrefs.DeleteKey(levelKey + "_CollectedCoins");
        PlayerPrefs.DeleteKey(levelKey + "_PlayerCurrency");
        PlayerPrefs.DeleteKey(levelKey + "_Stars");
        PlayerPrefs.DeleteKey(levelKey + "_PlayerKeys");
        PlayerPrefs.DeleteKey(levelKey + "_CheckpointX");
        PlayerPrefs.DeleteKey(levelKey + "_CheckpointY");
        PlayerPrefs.DeleteKey(levelKey + "_CheckpointZ");
        PlayerPrefs.DeleteKey(levelKey + "_LastCheckpointID");

        // Hapus semua status MiniGameCompleted untuk scene ini
        // Misal ID mini game 1 sampai 10, sesuaikan dengan jumlah mini game di scene
        for (int i = 1; i <= 10; i++)
        {
            string miniGameKey = levelKey + "_MiniGameCompleted_" + i;
            PlayerPrefs.DeleteKey(miniGameKey);
        }

        PlayerPrefs.Save();

        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        DeleteCurrentScenePlayerPrefs();
        PlayerPrefs.Save();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Ganti "Menu" jika nama scene utama berbeda
    }

    // Fungsi untuk menghapus PlayerPrefs sesuai scene aktif
    private void DeleteCurrentScenePlayerPrefs()
    {
        string levelKey = SceneManager.GetActiveScene().name;
        PlayerPrefs.DeleteKey(levelKey + "_Score");
        PlayerPrefs.DeleteKey(levelKey + "_Stars");
        PlayerPrefs.DeleteKey(levelKey + "_PlayerKeys");
        // Tambahkan key lain jika ada data lain yang ingin dihapus per scene
    }

    private void ToggleMuteBGM()
    {
        if (AudioManager.instance == null)
            return;

        bool isMuted = !AudioManager.instance.IsAllMuted();
        AudioManager.instance.SetAllVolume(isMuted ? 0f : 0.5f); // 0.5f = volume normal, sesuaikan jika perlu
        UpdateMuteIcon();
    }

    private void UpdateMuteIcon()
    {
        if (muteIcon == null) return;
        if (AudioManager.instance != null && AudioManager.instance.IsAllMuted())
        {
            if (mutedSprite != null)
                muteIcon.sprite = mutedSprite;
        }
        else
        {
            if (unmutedSprite != null)
                muteIcon.sprite = unmutedSprite;
        }
    }
}
