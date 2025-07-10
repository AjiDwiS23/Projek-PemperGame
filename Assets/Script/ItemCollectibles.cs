using UnityEngine;

public class ItemCollectibles : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;

    private void Awake()
    {
        string coinId = gameObject.name; // Pastikan nama unik, atau gunakan ID lain
        if (CurrencyManager.Instance != null && CurrencyManager.Instance.IsCoinCollected(coinId))
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string coinId = gameObject.name; // Pastikan nama unik, atau gunakan ID lain
            if (!CurrencyManager.Instance.IsCoinCollected(coinId))
            {
                AudioManager.instance.Play("Coin");
                CurrencyManager.Instance.AddCurrency(1);
                CurrencyManager.Instance.RegisterCollectedCoin(coinId);
                Destroy(gameObject);
            }
        }
    }
}