using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.Play("Splash");
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            if (player != null)
            {
                // Langsung set health ke 0 dan panggil Game Over
                var healthField = typeof(PlayerMovement).GetField("currentHealth", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (healthField != null)
                {
                    healthField.SetValue(player, 0);
                }
                player.SendMessage("Die");
            }
        }
    }
}
