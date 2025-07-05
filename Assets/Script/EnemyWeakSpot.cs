using UnityEngine;

public class EnemyWeakSpot : MonoBehaviour
{
    [SerializeField] private EnemyJumpAttack enemy; // Assign parent enemy di Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryStomp(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryStomp(other);
    }

    private void TryStomp(Collider2D other)
    {
        if (enemy != null && enemy.IsVulnerable() && other.CompareTag("Player"))
        {
            // Cek player benar-benar datang dari atas
            Rigidbody2D playerRb = other.GetComponent<Rigidbody2D>();
            if (playerRb != null && playerRb.velocity.y < 0)
            {
                enemy.TakeDamage(1);
                playerRb.velocity = new Vector2(playerRb.velocity.x, 10f);
            }
        }
    }
}