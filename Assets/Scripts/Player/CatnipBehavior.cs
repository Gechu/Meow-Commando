using UnityEngine;
using UnityEngine.InputSystem;

public class CatnipBehavior : MonoBehaviour
{
    [Header("Catnip")]
    public int catnipCount = 0;
    public float duration = 3f;

    public float speedMultiplier = 2f;
    public float fireRateMultiplier = 0.5f;
    public float bulletSpeedMultiplier = 1.5f;

    private bool isActive = false;

    private PlayerStatSystem stats;

    public CatnipUI ui;

    void Start()
    {
        stats = GetComponent<PlayerStatSystem>();

        if (ui == null)
            ui = FindFirstObjectByType<CatnipUI>();

        ui?.UpdateUI(catnipCount);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.eKey.wasPressedThisFrame && catnipCount > 0 && !isActive)
        {
            ActivateCatnip();
        }
    }

    public void AddCatnip(int amount)
    {
        catnipCount += amount;
        ui?.UpdateUI(catnipCount);
    }

    void ActivateCatnip()
    {
        catnipCount--;
        ui?.UpdateUI(catnipCount);

        isActive = true;

        stats.AddModifier(new StatModifier
        {
            moveSpeedMult = speedMultiplier,
            fireRateMult = fireRateMultiplier,
            bulletSpeedMult = bulletSpeedMultiplier,
            duration = duration
        });

        Invoke(nameof(ResetState), duration);
    }

    void ResetState()
    {
        isActive = false;
    }
}