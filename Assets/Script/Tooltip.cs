using UnityEngine;
using TMPro;

public class Tooltip : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI starsText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [Header("Default Tooltip Content")]
    [SerializeField] private string defaultLevelName = "Level 1";
    [SerializeField] [TextArea] private string defaultDescription = "Deskripsi level di sini...";
    [SerializeField] private int defaultStars = 0;
    [SerializeField] private int defaultScore = 0;

    [SerializeField] private Vector2 cursorOffset = new Vector2(40, -40);
    private bool isTooltipActive = false;

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

    // Ambil stars dan score dari PlayerPrefs berdasarkan levelName
    public void Show(string levelName, string description, string starsKey, string scoreKey)
    {
        int stars = PlayerPrefs.GetInt(starsKey, 0);
        int score = PlayerPrefs.GetInt(scoreKey, 0);

        if (panel != null)
            panel.SetActive(true);

        if (levelNameText != null)
            levelNameText.text = $"Level: {levelName}";
        if (descriptionText != null)
            descriptionText.text = description;
        if (starsText != null)
            starsText.text = stars.ToString();
        if (scoreText != null)
            scoreText.text = score.ToString();

        isTooltipActive = true;
    }

    public void ShowDefault()
    {
        Show(defaultLevelName, defaultDescription, "Quiz_Stars", "Quiz_FinalScore");
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        isTooltipActive = false;
    }
}
