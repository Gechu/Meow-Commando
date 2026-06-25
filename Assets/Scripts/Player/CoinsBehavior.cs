using UnityEngine;
using UnityEngine.InputSystem;

public class CoinsBehavior : MonoBehaviour
{
    public int coinCount = 0;
    private const int MaxCoins = 999;

    public CoinsUI ui;

    void Start()
    {
        if (ui == null)
            ui = FindFirstObjectByType<CoinsUI>();

        coinCount = PlayerDataManager.Instance.coins;

        ui?.UpdateUI(coinCount);
    }

    void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
        {
            AddCoins(100);
        }

        if (Keyboard.current.f6Key.wasPressedThisFrame)
        {
            PlayerDataManager.Instance.coins =
                Mathf.Max(0, PlayerDataManager.Instance.coins - 100);

            RefreshUI();
        }
    }

    public void AddCoins(int amount)
    {
        PlayerDataManager.Instance.coins = Mathf.Clamp(
            PlayerDataManager.Instance.coins + amount,
            0,
            MaxCoins);

        coinCount = PlayerDataManager.Instance.coins;

        ui?.UpdateUI(coinCount);

        Debug.Log("Coins: " + coinCount);
    }

    public bool SpendCoins(int amount)
    {
        PlayerDataManager.Instance.coins -= amount;

        coinCount = PlayerDataManager.Instance.coins;

        ui?.UpdateUI(coinCount);

        Debug.Log("Coins: " + coinCount);

        return true;
    }

    public void RefreshUI()
    {
        coinCount = PlayerDataManager.Instance.coins;

        ui?.UpdateUI(coinCount);
    }
}