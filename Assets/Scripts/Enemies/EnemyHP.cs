using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;

    private int currentHP;
    private bool dead;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (dead) return;
        if (amount <= 0) return;

        currentHP -= amount;

        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        dead = true;
        Destroy(gameObject);
    }
}