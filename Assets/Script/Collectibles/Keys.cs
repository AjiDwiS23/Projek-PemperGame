using UnityEngine;

public class Keys : MonoBehaviour
{
    [Header("Quiz Integration")]
    [SerializeField] private Quiz quizUI;         // Drag komponen Quiz di Inspector
    [SerializeField] private QuizData quizData;   // Drag asset QuizData di Inspector

    private bool collected = false;
    private string keyPrefs;

    private void Awake()
    {
        // Buat key unik berdasarkan scene dan nama gameObject
        keyPrefs = $"{UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}_Key_{gameObject.name}";
        // Jika sudah pernah diambil, langsung hilangkan
        if (PlayerPrefs.GetInt(keyPrefs, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

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
                PlayerPrefs.SetInt(keyPrefs, 1);    // Simpan status kunci sudah diambil
                PlayerPrefs.Save();
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
