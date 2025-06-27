using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private int currentCurrency = 0;

    [Header("Key Settings")]
    [SerializeField] private TextMeshProUGUI keyText; // Assign di Inspector
    [SerializeField] private int currentKeys = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateCurrencyText();
        UpdateKeyText();
    }

    public void AddCurrency(int value)
    {
        currentCurrency += value;
        UpdateCurrencyText();
    }

    public void SpendCurrency(int value)
    {
        currentCurrency = Mathf.Max(currentCurrency - value, 0);
        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
    {
        if (currencyText != null)
            currencyText.text = currentCurrency.ToString();
    }

    public void AddKey(int value)
    {
        currentKeys += value;
        UpdateKeyText();
    }

    public void SpendKey(int value)
    {
        currentKeys = Mathf.Max(currentKeys - value, 0);
        UpdateKeyText();
    }

    private void UpdateKeyText()
    {
        if (keyText != null)
            keyText.text = currentKeys.ToString();
    }

    public int CurrentCurrency => currentCurrency;
    public int CurrentKeys => currentKeys;
}
