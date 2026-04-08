using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CatnipBehavior : MonoBehaviour
{
    public int catnipCount = 0;

    public float duration = 3f;

    public float speedMultiplier = 2f;
    public float bulletSpeedMultiplier = 1.5f;
    public float fireRateMultiplier = 0.5f;

    private PlayerMovement movement;
    private PlayerShooting shooting;

    private bool isActive = false;

    public CatnipUI ui;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
        shooting = GetComponent<PlayerShooting>();

        if (ui == null)
            ui = FindFirstObjectByType<CatnipUI>();

        if (ui != null)
            ui.UpdateUI(catnipCount);

        Debug.Log("movement: " + movement);
        Debug.Log("shooting: " + shooting);
        Debug.Log("ui: " + ui);
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
        shooting.fireRate *= fireRateMultiplier;

        yield return new WaitForSeconds(duration);

        movement.speed /= speedMultiplier;
        shooting.bulletSpeed /= bulletSpeedMultiplier;
        shooting.fireRate /= fireRateMultiplier;

        isActive = false;
    }
}