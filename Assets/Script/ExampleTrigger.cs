using UnityEngine;

public class ExampleTrigger : MonoBehaviour
{
    public MiniGameQuestionData questionData; // Assign soal di Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Mini_Game_1.Instance.ShowMiniGame(questionData);
        }
    }
}