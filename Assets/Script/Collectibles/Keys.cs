using UnityEngine;

public class Keys : MonoBehaviour
{
    [Header("Quiz Integration")]
    [SerializeField] private Quiz quizUI;         // Drag komponen Quiz di Inspector
    [SerializeField] private QuizData quizData;   // Drag asset QuizData di Inspector

    private bool collected = false;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;
        if (other.CompareTag("Player") && quizUI != null && quizData != null)
        {
            collected = true; // Supaya tidak double trigger

            // Set quiz dan tampilkan
            quizUI.SetQuiz(quizData);
            quizUI.ShowQuiz();

            // Callback jika jawaban benar
            quizUI.onCorrectAnswer = () =>
            {
                CurrencyManager.Instance.AddKey(1); // Tambah 1 kunci & simpan PlayerPrefs
                Destroy(gameObject); // Hapus kunci dari scene
            };

            // Callback jika jawaban salah
            quizUI.onWrongAnswer = () =>
            {
                collected = false; // Boleh trigger lagi jika salah
            };
        }
    }
}
