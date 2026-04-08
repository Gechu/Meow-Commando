using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    private PlayerMovement movement;
    public HealthUI healthUI;
    public int maxHP = 3;
    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
        healthUI.CreateHearts(maxHP);
        healthUI.UpdateHearts(currentHP);
        movement = GetComponent<PlayerMovement>();
    }

    public void TakeDamage(int amount)
    {
        if (movement != null && movement.IsInvincible)
            return;

        currentHP -= amount;

        if (currentHP < 0)
            currentHP = 0;

        healthUI.UpdateHearts(currentHP);

        Debug.Log("HP: " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (currentHP >= maxHP)
            return;

        currentHP += amount;

        if (currentHP > maxHP)
            currentHP = maxHP;

        healthUI.UpdateHearts(currentHP);

        Debug.Log("Uleczono! HP: " + currentHP);
    }

    public void UpgradeHP(int amount)
    {
        maxHP += amount;
        currentHP = maxHP;
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
        Debug.Log("Gracz umar³");
        // jakaœ animacja, restart poziomu, czy jakiœ death screen czy coœ
    }
}