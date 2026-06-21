using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public Text titleText;
    public Text descriptionText;
    public Text priceText;

    public Button buyButton;

    private ShopItemData item;
    private ShopUIManager manager;

    public void Initialize(ShopItemData item, ShopUIManager manager)
    {
        this.item = item;
        this.manager = manager;

        titleText.text = item.title;
        descriptionText.text = item.description;

        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(OnBuyPressed);

        Refresh();
    }

    public void Refresh()
    {
        bool canBuy = manager.CanBuy(item);

        buyButton.interactable = canBuy;

        if (canBuy)
            priceText.text = item.price.ToString();
        else
            priceText.text = manager.GetUnavailableReason(item);
    }

    void OnBuyPressed()
    {
        manager.TryBuy(item);
    }
}