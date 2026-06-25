using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoSaveManager : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "MainHub")
        {
            SaveSystem.SaveGame();
            Debug.Log("Autosave.");
            SaveNotification.Instance?.Show();
        }
    }
}