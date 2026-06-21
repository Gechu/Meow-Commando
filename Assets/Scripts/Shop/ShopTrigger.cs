using UnityEngine;
using UnityEngine.InputSystem;

public class ShopTrigger : MonoBehaviour
{
    [Header("References")]
    public ShopUIManager shopUI;

    public GameObject interactPrompt;

    private bool playerNearby;

    void Awake()
    {
        if (shopUI == null)
            shopUI = FindFirstObjectByType<ShopUIManager>();
    }

    void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        HandleInput();
        HandlePrompt();
    }

    void HandleInput()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.fKey.wasPressedThisFrame && playerNearby)
            shopUI.ToggleShop();
    }

    void HandlePrompt()
    {
        if (interactPrompt == null) return;

        interactPrompt.SetActive(playerNearby && !shopUI.IsOpen);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = true;

        if (!shopUI.IsOpen)
            interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        playerNearby = false;
        interactPrompt.SetActive(false);

        if (shopUI == null)
            return;

        if (shopUI.IsOpen)
            shopUI.CloseShop();
    }

    public void HidePrompt()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    public void ShowPrompt()
    {
        if (playerNearby && interactPrompt != null)
            interactPrompt.SetActive(true);
    }
}