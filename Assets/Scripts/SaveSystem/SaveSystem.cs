using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SaveSystem
{
    private static string savePath => Application.persistentDataPath + "/save.json";

    public static void SaveGame()
    {
        SaveData data = new SaveData();

        data.maxHP = PlayerDataManager.Instance.maxHP;
        data.currentHP = PlayerDataManager.Instance.currentHP;

        data.catnipCount = PlayerDataManager.Instance.catnipCount;

        data.currentScene = SceneManager.GetActiveScene().name;

        string json = JsonUtility.ToJson(data, true);

        File.WriteAllText(savePath, json);

        Debug.Log("Game Saved: " + savePath);
    }

    public static void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("Save file does not exist!");
            return;
        }

        string json = File.ReadAllText(savePath);

        SaveData data = JsonUtility.FromJson<SaveData>(json);

        PlayerDataManager.Instance.maxHP = data.maxHP;
        PlayerDataManager.Instance.currentHP = data.currentHP;

        PlayerDataManager.Instance.catnipCount = data.catnipCount;

        Debug.Log("Game Loaded!");

        SceneManager.LoadScene(data.currentScene);
    }

    public static bool SaveExists()
    {
        return File.Exists(savePath);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);

            Debug.Log("Save Deleted");
        }
    }
}