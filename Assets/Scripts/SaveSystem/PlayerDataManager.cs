using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager Instance;

    [Header("Default Data")]
    public PlayerDefaultData defaultData;

    [Header("Runtime Player Stats")]
    public int maxHP;
    public int currentHP;

    [Header("Weapons")]
    public bool[] unlockedWeapons = new bool[5];

    [Header("Upgrades")]
    public float speedMultiplierBonus;
    public float fireRateMultiplierBonus;
    public float bulletSpeedMultiplierBonus;

    [Header("Resources")]
    public int catnipCount;
    public int coins;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void LoadDefaults()
    {
        maxHP = defaultData.defaultMaxHP;
        currentHP = defaultData.defaultCurrentHP;

        catnipCount = defaultData.defaultCatnipCount;
        coins = defaultData.defaultCoins;

        unlockedWeapons = (bool[])defaultData.defaultUnlockedWeapons.Clone();

        speedMultiplierBonus = defaultData.defaultSpeedMultiplierBonus;
        fireRateMultiplierBonus = defaultData.defaultFireRateMultiplierBonus;
        bulletSpeedMultiplierBonus = defaultData.defaultBulletSpeedMultiplierBonus;

        Debug.Log("Loaded Default Player Data");
    }

    void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            SaveSystem.SaveGame();
            Debug.Log("Quick Save (F5)");
        }
    }

    public bool UnlockNextWeapon()
    {
        for (int i = 0; i < unlockedWeapons.Length; i++)
        {
            if (!unlockedWeapons[i])
            {
                unlockedWeapons[i] = true;
                return true;
            }
        }

        return false;
    }

    public bool AllWeaponsUnlocked()
    {
        foreach (bool unlocked in unlockedWeapons)
        {
            if (!unlocked)
                return false;
        }

        return true;
    }
}