using UnityEngine;

public class ItemCollectibles : MonoBehaviour
{
    [SerializeField] private int scoreValue = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    private void CollectItem()
    {
        CurrencyManager.Instance.AddCurrency(1);
        Destroy(gameObject);
    }
}