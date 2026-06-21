using System;

[Serializable]
public class SaveData
{
    // Player
    public int maxHP;
    public int currentHP;

    // Resources
    public int catnipCount;
    public int coins;

    // Scene
    public string currentScene;

    // Weapons
    public bool[] unlockedWeapons;

    // Catnip upgrades
    public float speedMultiplierBonus;
    public float fireRateMultiplierBonus;
    public float bulletSpeedMultiplierBonus;
}