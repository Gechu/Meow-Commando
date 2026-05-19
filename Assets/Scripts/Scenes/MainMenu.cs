using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void NewGame()
    {
        PlayerDataManager.Instance.LoadDefaults();
        SceneManager.LoadScene("Intro_Cutscene");
    }

    public void ContinueGame()
    {
        SaveSystem.LoadGame();
        // SceneManager.LoadScene("Default");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}