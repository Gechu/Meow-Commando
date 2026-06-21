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
        data.coins = PlayerDataManager.Instance.coins;

        data.currentScene = SceneManager.GetActiveScene().name;

        data.unlockedWeapons = (bool[])PlayerDataManager.Instance.unlockedWeapons.Clone();

        data.speedMultiplierBonus = PlayerDataManager.Instance.speedMultiplierBonus;
        data.fireRateMultiplierBonus = PlayerDataManager.Instance.fireRateMultiplierBonus;
        data.bulletSpeedMultiplierBonus = PlayerDataManager.Instance.bulletSpeedMultiplierBonus;

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
        PlayerDataManager.Instance.coins = data.coins;

        PlayerDataManager.Instance.unlockedWeapons = (bool[])data.unlockedWeapons.Clone();

        PlayerDataManager.Instance.speedMultiplierBonus = data.speedMultiplierBonus;
        PlayerDataManager.Instance.fireRateMultiplierBonus = data.fireRateMultiplierBonus;
        PlayerDataManager.Instance.bulletSpeedMultiplierBonus = data.bulletSpeedMultiplierBonus;

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