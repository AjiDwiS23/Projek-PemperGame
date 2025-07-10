using UnityEngine;
using TMPro;
using System.Collections; // Tambahkan ini

public class Tooltip : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI keysText;

    [Header("Default Tooltip Content")]
    [SerializeField] private string defaultLevelName = "Level 1";
    [SerializeField] [TextArea] private string defaultDescription = "Deskripsi level di sini...";
    [SerializeField] private int defaultStars = 0;
    [SerializeField] private int defaultScore = 0;

    [SerializeField] private Vector2 cursorOffset = new Vector2(40, -40);
    private bool isTooltipActive = false;

    private Coroutine scoreAnimCoroutine; // Untuk menghentikan animasi sebelumnya jika ada

    public static Tooltip Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        Hide();
    }

    private void Start()
    {
        // ShowDefault(); // Hapus atau komentar baris ini
    }

    private void Update()
    {
        if (isTooltipActive && panel != null)
        {
            RectTransform canvasRect = panel.transform.parent as RectTransform;
            RectTransform panelRect = panel.transform as RectTransform;

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                Input.mousePosition,
                null,
                out pos
            );
            ((RectTransform)panel.transform).anchoredPosition = pos + cursorOffset;
        }
    }

    // Ambil stars, score, dan keys dari PlayerPrefs berdasarkan key
    public void Show(string levelName, string description, string starsKey, string scoreKey, string keysKey)
    {
        int stars = PlayerPrefs.GetInt(starsKey, 0);
        int score = PlayerPrefs.GetInt(scoreKey, 0);
        int keys = PlayerPrefs.GetInt(keysKey, 0);

        if (panel != null)
            panel.SetActive(true);

        if (levelNameText != null)
            levelNameText.text = $"Level: {levelName}";
        if (descriptionText != null)
            descriptionText.text = description;
        if (starsText != null)
            starsText.text = stars.ToString();
        if (keysText != null)
            keysText.text = keys.ToString();

        // Animasi angka score
        if (scoreText != null)
        {
            if (scoreAnimCoroutine != null)
                StopCoroutine(scoreAnimCoroutine);
            scoreAnimCoroutine = StartCoroutine(AnimateScoreText(scoreText, score, 1f));
        }

        isTooltipActive = true;
    }

    public void ShowDefault()
    {
        Show(defaultLevelName, defaultDescription, "Quiz_Stars", "Quiz_FinalScore", "Quiz_PlayerKeys");
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        isTooltipActive = false;

        // Hentikan animasi jika tooltip disembunyikan
        if (scoreAnimCoroutine != null)
        {
            StopCoroutine(scoreAnimCoroutine);
            scoreAnimCoroutine = null;
        }
    }

    // Coroutine animasi angka score
    private IEnumerator AnimateScoreText(TextMeshProUGUI text, int targetScore, float duration = 1f)
    {
        float elapsed = 0f;
        int displayedScore = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            displayedScore = Mathf.RoundToInt(Mathf.Lerp(0, targetScore, t));
            text.text = displayedScore.ToString();
            yield return null;
        }
        text.text = targetScore.ToString();
    }
}
