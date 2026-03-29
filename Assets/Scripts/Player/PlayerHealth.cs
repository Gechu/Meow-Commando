using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public HealthUI healthUI;
    public int maxHP = 3;
    public int currentHP;

    void Start()
    {
        currentHP = maxHP;
        healthUI.CreateHearts(maxHP);
        healthUI.UpdateHearts(currentHP);
    }

    public void TakeDamage(int amount)
    {
        currentHP -= amount;
        healthUI.UpdateHearts(currentHP);

        if (currentHP < 0)
            currentHP = 0;

        Debug.Log("HP: " + currentHP);

        if (currentHP == 0)
        {
            Die();
        }
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