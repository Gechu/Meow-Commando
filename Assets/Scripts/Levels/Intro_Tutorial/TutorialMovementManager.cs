using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TutorialMovementManager : MonoBehaviour
{
    [Header("UI")]
    public Text checklistText;

    [Header("Door")]
    public Door tutorialDoor;

    private bool moved = false;
    private bool dashed = false;

    private bool tutorialCompleted = false;

    void Update()
    {
        if (!moved)
        {
            CheckMovement();
        }

        if (!dashed)
        {
            CheckDash();
        }

        UpdateChecklist();

        // Odblokuj drzwi tylko raz
        if (!tutorialCompleted && moved && dashed)
        {
            tutorialCompleted = true;

            tutorialDoor.UnlockDoor();

            Debug.Log("Tutorial ukończony!");
        }
    }

    void CheckMovement()
    {
        if (
            Keyboard.current.wKey.wasPressedThisFrame ||
            Keyboard.current.aKey.wasPressedThisFrame ||
            Keyboard.current.sKey.wasPressedThisFrame ||
            Keyboard.current.dKey.wasPressedThisFrame
        )
        {
            moved = true;
        }
    }

    void CheckDash()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            dashed = true;
        }
    }

    void UpdateChecklist()
    {
        checklistText.text =
            (moved ? "☑ " : "☐ ") + "Move using WASD\n" +
            (dashed ? "☑ " : "☐ ") + "Dash using SPACE";
    }
}