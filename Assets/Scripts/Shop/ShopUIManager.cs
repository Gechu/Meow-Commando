using System.Collections.Generic;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject shopPanel;

    [Header("Shop Items")]
    public ShopItemUI[] itemSlots = new ShopItemUI[5];

    private List<ShopItemData> items;

    [Header("Player")]
    public PlayerHealth playerHealth;

    [Header("Prices")]
    public int hpUpgradePrice = 100;
    public int healPrice = 30;
    public int catnipUpgradePrice = 150;
    public int buyCatnipPrice = 50;
    public int unlockWeaponPrice = 250;

    private readonly List<ShopItemUI> spawnedItems = new();
    private List<ShopItemData> currentItems;

    public bool IsOpen { get; private set; }

    public static ShopUIManager Instance;
    CoinsBehavior coins;

    private void Awake()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
        Instance = this;
        coins = FindFirstObjectByType<CoinsBehavior>();
    }

    public void ToggleShop()
    {
        if (IsOpen)
            CloseShop();
        else
            OpenShop();
    }

    public void OpenShop()
    {
        IsOpen = true;
        shopPanel.SetActive(true);

        items = ShopDatabase.GenerateShop();
        RefreshShop();
    }

    public void CloseShop()
    {
        if (shopPanel != null)
            shopPanel.SetActive(false);
        
        IsOpen = false;
    }

    void RefreshShop()
    {
        if (items == null)
            return;

        for (int i = 0; i < itemSlots.Length; i++)
        {
            itemSlots[i].Initialize(items[i], this);
        }
    }

    public bool CanBuy(ShopItemData item)
    {
        if (PlayerDataManager.Instance.coins < item.price)
            return false;

        switch (item.type)
        {
            case ShopItemType.UpgradeHP:
                return PlayerDataManager.Instance.maxHP < 10;

            case ShopItemType.HealHP:
                return PlayerDataManager.Instance.currentHP <
                       PlayerDataManager.Instance.maxHP;

            case ShopItemType.UpgradeCatnip:
                return true;

            case ShopItemType.BuyCatnip:
                return true;

            case ShopItemType.UnlockWeapon:
                return !PlayerDataManager.Instance.AllWeaponsUnlocked();
        }

        return false;
    }

    public string GetUnavailableReason(ShopItemData item)
    {
        if (PlayerDataManager.Instance.coins < item.price)
            return "Not enough coins";

        switch (item.type)
        {
            case ShopItemType.UpgradeHP:

                if (PlayerDataManager.Instance.maxHP >= 10)
                    return "MAX HP";

                break;

            case ShopItemType.HealHP:

                if (PlayerDataManager.Instance.currentHP >=
                    PlayerDataManager.Instance.maxHP)
                    return "FULL HP";

                break;

            case ShopItemType.UnlockWeapon:

                if (PlayerDataManager.Instance.AllWeaponsUnlocked())
                    return "SOLD OUT";

                break;
        }

        return "";
    }

    public void TryBuy(ShopItemData item)
    {
        if (!CanBuy(item))
            return;

        if (!coins.SpendCoins(item.price))
            return;

        switch (item.type)
        {
            case ShopItemType.UpgradeHP:

                PlayerDataManager.Instance.maxHP++;
                PlayerDataManager.Instance.currentHP++;

                if (playerHealth != null)
                {
                    playerHealth.maxHP =
                        PlayerDataManager.Instance.maxHP;

                    playerHealth.currentHP =
                        PlayerDataManager.Instance.currentHP;

                    playerHealth.healthUI.CreateHearts(playerHealth.maxHP);
                    playerHealth.healthUI.UpdateHearts(playerHealth.currentHP);
                }

                break;

            case ShopItemType.HealHP:

                PlayerDataManager.Instance.currentHP++;

                if (PlayerDataManager.Instance.currentHP >
                    PlayerDataManager.Instance.maxHP)
                {
                    PlayerDataManager.Instance.currentHP =
                        PlayerDataManager.Instance.maxHP;
                }

                if (playerHealth != null)
                {
                    playerHealth.currentHP =
                        PlayerDataManager.Instance.currentHP;

                    playerHealth.healthUI.UpdateHearts(playerHealth.currentHP);
                }

                break;

            case ShopItemType.BuyCatnip:

                PlayerDataManager.Instance.catnipCount++;

                break;

            case ShopItemType.UpgradeCatnip:

                ApplyCatnipUpgrade(item.catnipUpgrade);

                break;

            case ShopItemType.UnlockWeapon:

                PlayerDataManager.Instance.UnlockNextWeapon();

                break;
        }

        RefreshShop();
    }

    void ApplyCatnipUpgrade(CatnipUpgradeType type)
    {
        const float bonusStep = 0.2f;

        switch (type)
        {
            case CatnipUpgradeType.Speed:
                PlayerDataManager.Instance.speedMultiplierBonus += bonusStep;
                break;

            case CatnipUpgradeType.FireRate:
                PlayerDataManager.Instance.fireRateMultiplierBonus += bonusStep;
                break;

            case CatnipUpgradeType.BulletSpeed:
                PlayerDataManager.Instance.bulletSpeedMultiplierBonus += bonusStep;
                break;
        }
    }
}