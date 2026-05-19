using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private IPlayerMovement movement;
    public DeathManager deathScreen;
    public HealthUI healthUI;
    public int maxHP = 3;
    public int currentHP;

    void Start()
    {
        movement = GetComponent<IPlayerMovement>();

        // Wczytaj dane gracza
        maxHP = PlayerDataManager.Instance.maxHP;
        currentHP = PlayerDataManager.Instance.currentHP;

        healthUI.CreateHearts(maxHP);
        healthUI.UpdateHearts(currentHP);
    }

    public void TakeDamage(int amount)
    {
        if (movement != null && movement.IsInvincible)
            return;
        else
        {
            currentHP -= amount;
            PlayerDataManager.Instance.currentHP = currentHP;

            if (currentHP < 0)
                currentHP = 0;

            healthUI.UpdateHearts(currentHP);

            Debug.Log("HP: " + currentHP);

            if (currentHP == 0)
            {
                Die();
            }
        }
    }

    public void Heal(int amount)
    {
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

    void Die()
    {
        Debug.Log("Gracz umar�");

        if (deathScreen != null)
        {
            deathScreen.Die();
        }
    }
}