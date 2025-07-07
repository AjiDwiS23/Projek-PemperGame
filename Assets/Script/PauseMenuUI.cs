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
    private bool isBGMMuted = false;

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
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Ganti "MainMenu" dengan nama scene menu utama Anda
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
