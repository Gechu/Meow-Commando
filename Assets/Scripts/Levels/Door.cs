using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    public bool unlocked;

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Color lockedColor = Color.red;
    public Color unlockedColor = Color.white;

    [Header("Interaction UI")]
    public GameObject interactPrompt;

    [Header("Scene Transition")]
    public string nextSceneName;

    private bool playerNearby = false;

    void Start()
    {
        // Ustaw kolor startowy drzwi
        if (spriteRenderer != null)
        {
            if (unlocked == false)
            {
                spriteRenderer.color = lockedColor;
            }
            else if (unlocked == true)
            {
                spriteRenderer.color = unlockedColor;
            }
        }

        // Ukryj prompt interakcji
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Jeœli drzwi s¹ odblokowane i gracz jest blisko
        if (unlocked && playerNearby)
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(true);
            }

            // Naciœniêcie F = przejœcie do kolejnej sceny
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                LoadNextScene();
            }
        }
        else
        {
            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }

    // Wywo³ywane przez TutorialManager po wykonaniu zadañ
    public void UnlockDoor()
    {
        unlocked = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = unlockedColor;
        }

        Debug.Log("Drzwi zosta³y odblokowane!");
    }

    void LoadNextScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            if (interactPrompt != null)
            {
                interactPrompt.SetActive(false);
            }
        }
    }
}