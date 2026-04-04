using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CatnipBehavior : MonoBehaviour
{
    public int catnipCount = 0;

    public float duration = 3f;

    public float speedMultiplier = 2f;
    public float bulletSpeedMultiplier = 2f;

    private PlayerMovement movement;
    private PlayerShooting shooting;

    private bool isActive = false;

    public CatnipUI ui;

    void Start()
    {
        // Debug.Log(ui);
        movement = GetComponent<PlayerMovement>();
        shooting = GetComponent<PlayerShooting>();

        ui.UpdateUI(catnipCount);
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && catnipCount > 0 && !isActive)
        {
            ActivateCatnip();
        }
    }

    public void AddCatnip(int amount)
    {
        catnipCount += amount;
        ui.UpdateUI(catnipCount);
    }

    void ActivateCatnip()
    {
        catnipCount--;
        ui.UpdateUI(catnipCount);

        StartCoroutine(CatnipRoutine());
    }

    IEnumerator CatnipRoutine()
    {
        isActive = true;

        movement.speed *= speedMultiplier;
        shooting.bulletSpeed *= bulletSpeedMultiplier;

        yield return new WaitForSeconds(duration);

        movement.speed /= speedMultiplier;
        shooting.bulletSpeed /= bulletSpeedMultiplier;

        isActive = false;
    }
}