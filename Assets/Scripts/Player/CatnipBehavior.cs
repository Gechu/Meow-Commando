using UnityEngine;
using UnityEngine.InputSystem;

public class CatnipBehavior : MonoBehaviour
{
    [Header("Catnip")]
    public int catnipCount = 0;

    public float duration = 3f;

    [Header("Base Multipliers")]
    public float baseSpeedMultiplier = 2f;
    public float baseFireRateMultiplier = 0.5f;
    public float baseBulletSpeedMultiplier = 1.5f;

    private bool isActive;

    private PlayerStatSystem stats;

    public CatnipUI ui;

    void Start()
    {
        stats = GetComponent<PlayerStatSystem>();

        if (ui == null)
            ui = FindFirstObjectByType<CatnipUI>();

        catnipCount =
            PlayerDataManager.Instance.catnipCount;

        ui.UpdateUI(catnipCount);
    }

    void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            ActivateCatnip();
        }
    }

    public void AddCatnip(int amount)
    {
        PlayerDataManager.Instance.catnipCount += amount;

        catnipCount =
            PlayerDataManager.Instance.catnipCount;

        ui.UpdateUI(catnipCount);
    }

    void ActivateCatnip()
    {
        if (isActive)
            return;

        if (PlayerDataManager.Instance.catnipCount <= 0)
            return;

        PlayerDataManager.Instance.catnipCount--;

        catnipCount =
            PlayerDataManager.Instance.catnipCount;

        ui.UpdateUI(catnipCount);

        isActive = true;

        stats.AddModifier(new StatModifier
        {
            moveSpeedMult =
                baseSpeedMultiplier +
                PlayerDataManager.Instance.speedMultiplierBonus,

            fireRateMult =
                baseFireRateMultiplier -
                PlayerDataManager.Instance.fireRateMultiplierBonus,

            bulletSpeedMult =
                baseBulletSpeedMultiplier +
                PlayerDataManager.Instance.bulletSpeedMultiplierBonus,

            duration = duration
        });

        Invoke(nameof(ResetState), duration);
    }

    void ResetState()
    {
        isActive = false;
    }
}