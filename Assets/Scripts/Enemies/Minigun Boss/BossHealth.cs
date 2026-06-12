using UnityEngine;
using UnityEngine.Events;

public class BossHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 300;
    private float currentHealth;
    public float CurrentHealth => currentHealth;

    [Header("Events")]
    public UnityEvent<float, float> onHealthChanged; // (current, max)
    public UnityEvent onBossDied;

    private void Start()
    {
        currentHealth = maxHealth;
        onHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth < 0) currentHealth = 0;

        onHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth == 0)
            Die();

        Debug.Log($"Boss HP: {currentHealth}/{maxHealth}");
    }

    private void Die()
    {
        onBossDied?.Invoke();
        Destroy(gameObject);
    }
}
