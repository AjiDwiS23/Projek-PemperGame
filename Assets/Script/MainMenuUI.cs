using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void Start()
    {
        AudioManager.instance.PlayBGM();
    }

    public void PlayBtn(string sceneName)
    {
        AudioManager.instance.Play("Click");
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public void ExitBtn()
    {
        AudioManager.instance.Play("Click");
        Application.Quit();
    }

    public void BackBtn(string sceneName)
    {
        AudioManager.instance.Play("Click");
        SceneManager.LoadScene(sceneName);
    }

    public void ResetAllDataBtn()
    {
        AudioManager.instance.Play("Click");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("Semua data PlayerPrefs telah dihapus.");
    }
}
