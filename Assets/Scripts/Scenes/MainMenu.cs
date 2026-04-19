using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Default");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}