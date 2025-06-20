using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayBtn(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        Time.timeScale = 1;
    }

    public void ExitBtn(){
        Application.Quit();
    }

    public void BackBtn(string sceneName){
        SceneManager.LoadScene(sceneName);
    }
}
