using System.Collections.Generic;

public static class ShopDatabase
{
    public static List<ShopItemData> GenerateShop()
    {
        CatnipUpgradeType randomUpgrade =
            (CatnipUpgradeType)UnityEngine.Random.Range(1, 4);

        List<ShopItemData> items = new();

        items.Add(new ShopItemData(
            ShopItemType.UpgradeHP,
            "Max HP +1",
            "Increase maximum HP by 1. Maximum: 10 HP.",
            100));

        items.Add(new ShopItemData(
            ShopItemType.HealHP,
            "Heal",
            "Restore 1 HP.",
            30));

        items.Add(new ShopItemData(
            ShopItemType.UpgradeCatnip,
            "Catnip Upgrade",
            GetCatnipDescription(randomUpgrade),
            150,
            randomUpgrade));

        items.Add(new ShopItemData(
            ShopItemType.BuyCatnip,
            "Buy Catnip",
            "Receive one Catnip.",
            50));

        items.Add(new ShopItemData(
            ShopItemType.UnlockWeapon,
            "Unlock Weapon",
            "Unlock the next weapon.",
            250));

        return items;
    }

    static string GetCatnipDescription(CatnipUpgradeType type)
    {
        switch (type)
        {
            case CatnipUpgradeType.Speed:
                return "Increase Catnip speed bonus.";

            case CatnipUpgradeType.FireRate:
                return "Increase Catnip fire rate bonus.";

            case CatnipUpgradeType.BulletSpeed:
                return "Increase Catnip bullet speed bonus.";

            default:
                return "";
        }
    }
}