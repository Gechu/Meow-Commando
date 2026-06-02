using UnityEngine;
using UnityEngine.InputSystem;

public class ShopTrigger : MonoBehaviour
{
    [Header("UI")]
    public GameObject shopUI;
    public GameObject interactPrompt;

    private bool playerNearby = false;
    private bool shopOpen = false;

    void Start()
    {
        if (shopUI != null)
            shopUI.SetActive(false);

        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    void Update()
    {
        // Jeśli gracz jest blisko
        if (playerNearby)
        {
            // Jeśli sklep jest zamknięty → pokazujemy prompt
            if (!shopOpen)
                interactPrompt.SetActive(true);
            else
                interactPrompt.SetActive(false);

            // F otwiera lub zamyka sklep
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                if (!shopOpen)
                    OpenShop();
                else
                    CloseShop();
            }
        }
        else
        {
            interactPrompt.SetActive(false);
        }
    }

    void OpenShop()
    {
        shopOpen = true;
        shopUI.SetActive(true);

        // Pauza tylko przy otwartym sklepie
        Time.timeScale = 0f;
    }

    void CloseShop()
    {
        shopOpen = false;
        shopUI.SetActive(false);

        // Przywrócenie gry
        Time.timeScale = 1f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (shopOpen)
                CloseShop();

            interactPrompt.SetActive(false);
        }
    }
}
