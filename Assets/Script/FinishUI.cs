using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishUI : MonoBehaviour
{
    [SerializeField] private GameObject finishCanvas;

    private void Start()
    {
        if (finishCanvas != null)
        {
            finishCanvas.SetActive(false); // Sembunyikan Canvas saat level dimulai
        }
    }

    public void ShowFinishUI()
    {
        if (finishCanvas != null)
        {
            finishCanvas.SetActive(true); // Tampilkan Canvas saat level selesai
        }
    }

    public void NextLevelBtn()
    {
        SceneManager.LoadScene(2);
    }

}
