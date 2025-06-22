using UnityEngine;
using TMPro;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private int currentCurrency = 0;

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

    public int CurrentCurrency => currentCurrency;
}
