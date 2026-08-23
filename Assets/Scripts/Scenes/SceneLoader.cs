using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map");
    }

    public void LoadGameOver()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Gameover_page");
    }

    public void LoadEnding()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Ending_page");
    }

    public void RetryGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Map");
    }

    public void LoadMain()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main_page");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}