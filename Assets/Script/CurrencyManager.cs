using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private int currentCurrency = 0;

    [Header("Key Settings")]
    [SerializeField] private TextMeshProUGUI keyText; // Assign di Inspector
    [SerializeField] private int currentKeys = 0;

    private const string CurrencyKey = "PlayerCurrency";
    private const string KeyKey = "PlayerKeys";
    private const string CollectedCoinsKey = "CollectedCoins";

    private HashSet<string> collectedCoinIds = new HashSet<string>();

    private int lastCurrency = -1;
    private int lastKeys = -1;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        LoadData();
    }

    private void Start()
    {
        UpdateCurrencyText();
        UpdateKeyText();
    }

    private void Update()
    {
        if (Application.isPlaying)
        {
            if (currentCurrency != lastCurrency)
            {
                UpdateCurrencyText();
                lastCurrency = currentCurrency;
            }
            if (currentKeys != lastKeys)
            {
                UpdateKeyText();
                lastKeys = currentKeys;
            }
        }
    }

    public void AddCurrency(int value)
    {
        currentCurrency += value;
        UpdateCurrencyText();
        SaveData();
    }

    public void SpendCurrency(int value)
    {
        currentCurrency = Mathf.Max(currentCurrency - value, 0);
        UpdateCurrencyText();
        SaveData();
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
        SaveData();
    }

    public void SpendKey(int value)
    {
        currentKeys = Mathf.Max(currentKeys - value, 0);
        UpdateKeyText();
        SaveData();
    }

    private void UpdateKeyText()
    {
        if (keyText != null)
            keyText.text = currentKeys.ToString();
    }

    public int CurrentCurrency => currentCurrency;
    public int CurrentKeys => currentKeys;

    // --- Penyimpanan Data ---
    private void SaveData()
    {
        PlayerPrefs.SetInt(CurrencyKey, currentCurrency);
        PlayerPrefs.SetInt(KeyKey, currentKeys);
        PlayerPrefs.SetString(CollectedCoinsKey, string.Join(",", collectedCoinIds));
        PlayerPrefs.Save();
        Debug.Log($"[SaveData] Currency: {currentCurrency}, Keys: {currentKeys}");
    }

    private void LoadData()
    {
        currentCurrency = PlayerPrefs.GetInt(CurrencyKey, 0);
        currentKeys = PlayerPrefs.GetInt(KeyKey, 0);

        string collected = PlayerPrefs.GetString(CollectedCoinsKey, "");
        collectedCoinIds = new HashSet<string>(collected.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));
        Debug.Log($"[LoadData] Currency: {currentCurrency}, Keys: {currentKeys}");
    }

    // --- Untuk Coin Collectible ---
    public void RegisterCollectedCoin(string coinId)
    {
        if (!collectedCoinIds.Contains(coinId))
        {
            collectedCoinIds.Add(coinId);
            SaveData();
        }
    }

    public bool IsCoinCollected(string coinId)
    {
        return collectedCoinIds.Contains(coinId);
    }

    private void OnValidate()
    {
        UpdateCurrencyText();
        UpdateKeyText();
        // Jangan panggil SaveData di sini!
    }
}