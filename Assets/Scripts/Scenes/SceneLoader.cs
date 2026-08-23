using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Map");
    }

    public void LoadGameOver()
    {
        SceneManager.LoadScene("Gameover_page");
    }

    public void LoadEnding()
    {
        SceneManager.LoadScene("Ending_page");
    }

    public void RetryGame()
    {
        SceneManager.LoadScene("Map");
    }

    public void LoadMain()
    {
        SceneManager.LoadScene("Main_page");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}