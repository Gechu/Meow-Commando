using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDefaultData", menuName = "Game/Player Default Data")]
public class PlayerDefaultData : ScriptableObject
{
    [Header("Health")]
    public int defaultMaxHP = 3;
    public int defaultCurrentHP = 3;

    [Header("Resources")]
    public int defaultCatnipCount = 0;
    public int defaultCoins = 150;

    [Header("Weapons")]
    public bool[] defaultUnlockedWeapons = new bool[5]
    {
        true,   // Broń 1 odblokowana
        false,  // Broń 2 zablokowana
        false,  // Broń 3 zablokowana
        false,  // Broń 4 zablokowana
        false   // Broń 5 zablokowana
    };

    [Header("Catnip Upgrades")]
    public float defaultSpeedMultiplierBonus = 0f;
    public float defaultFireRateMultiplierBonus = 0f;
    public float defaultBulletSpeedMultiplierBonus = 0f;
}