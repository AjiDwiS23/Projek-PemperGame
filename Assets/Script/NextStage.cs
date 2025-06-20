using UnityEngine;
using UnityEngine.SceneManagement; // Import SceneManager

public class NextStage : MonoBehaviour
{
    public string nextLevelName; // Specify the next level name

    // Function to detect collision with the player
    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that touches this trigger is the player
        if (other.CompareTag("Player"))
        {
            // Load the next level
            SceneManager.LoadScene(nextLevelName);
        }
    }
}
