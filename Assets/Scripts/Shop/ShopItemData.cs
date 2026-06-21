using UnityEngine;

public enum ShopItemType
{
    UpgradeHP,
    HealHP,
    UpgradeCatnip,
    BuyCatnip,
    UnlockWeapon
}

public enum CatnipUpgradeType
{
    None,
    Speed,
    FireRate,
    BulletSpeed
}

[System.Serializable]
public class ShopItemData
{
    public ShopItemType type;

    public string title;

    [TextArea]
    public string description;

    public int price;

    public CatnipUpgradeType catnipUpgrade;

    public ShopItemData(
        ShopItemType type,
        string title,
        string description,
        int price,
        CatnipUpgradeType catnipUpgrade = CatnipUpgradeType.None)
    {
        this.type = type;
        this.title = title;
        this.description = description;
        this.price = price;
        this.catnipUpgrade = catnipUpgrade;
    }
}