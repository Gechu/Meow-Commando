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

    [Header("Resources")]
    public int catnipCount;

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
}