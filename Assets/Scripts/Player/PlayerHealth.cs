using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    private IPlayerMovement movement;
    public DeathManager deathScreen;
    public HealthUI healthUI;
    public int maxHP = 3;
    public int currentHP;
    [SerializeField] private float damageImmunityTime = 1f;
    [SerializeField] private float blinkInterval = 0.1f;
    private SpriteRenderer[] spriteRenderers;
    private bool isDamageImmune = false;
    private Coroutine damageCoroutine;
    public bool IsDead { get; private set; }

    void Start()
    {
        IsDead = false;
        Debug.Log(IsDead);

        movement = GetComponent<IPlayerMovement>();

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        // Wczytaj dane gracza
        maxHP = PlayerDataManager.Instance.maxHP;
        currentHP = PlayerDataManager.Instance.currentHP;

        healthUI.CreateHearts(maxHP);
        healthUI.UpdateHearts(currentHP);
    }

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        if (movement != null && movement.IsInvincible)
            return;

        if (isDamageImmune)
            return;

        currentHP -= amount;
        PlayerDataManager.Instance.currentHP = currentHP;
        LevelRunManager.Instance?.AddDamageTaken(amount);

        if (currentHP < 0)
            currentHP = 0;

        healthUI.UpdateHearts(currentHP);

        Debug.Log("HP: " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }

        StartCoroutine(DamageImmunity());
    }

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        if (currentHP >= maxHP)
            return;

        currentHP += amount;

        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }

        PlayerDataManager.Instance.currentHP = currentHP;

        healthUI.UpdateHearts(currentHP);

        Debug.Log("Uleczono! HP: " + currentHP);
    }

    public void UpgradeHP(int amount)
    {
        maxHP += amount;
        currentHP = maxHP;

        PlayerDataManager.Instance.maxHP = maxHP;
        PlayerDataManager.Instance.currentHP = currentHP;

        healthUI.UpdateHearts(currentHP);
    }

    void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.qKey.wasPressedThisFrame)
        {
            TakeDamage(1);
        }
    }

    private IEnumerator DamageImmunity()
    {
        isDamageImmune = true;

        float elapsed = 0f;

        while (elapsed < damageImmunityTime)
        {
            foreach (var sr in spriteRenderers)
            {
                if (sr != null)
                    sr.enabled = !sr.enabled;
            }

            yield return new WaitForSeconds(blinkInterval);

            elapsed += blinkInterval;
        }

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
                sr.enabled = true;
        }

        isDamageImmune = false;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    void Die()
    {
        if (IsDead) return;

        IsDead = true;

        Debug.Log("Gracz umarł");

        LevelRunManager.Instance?.EndRun();

        if (deathScreen != null)
        {
            deathScreen.Die();
        }
    }
}