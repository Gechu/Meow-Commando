using UnityEngine;

public class CoinsBehavior : MonoBehaviour
{
    public int coinCount = 0;

    public CoinsUI ui;

    void Start()
    {
        if (ui == null)
            ui = FindFirstObjectByType<CoinsUI>();

        coinCount = PlayerDataManager.Instance.coins;

        ui?.UpdateUI(coinCount);
    }

    public void AddCoins(int amount)
    {
        PlayerDataManager.Instance.coins += amount;

        coinCount = PlayerDataManager.Instance.coins;

        ui?.UpdateUI(coinCount);

        Debug.Log("Coins: " + coinCount);
    }

    public bool SpendCoins(int amount)
    {
        if (PlayerDataManager.Instance.coins < amount)
            return false;

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