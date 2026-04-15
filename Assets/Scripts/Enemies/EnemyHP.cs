using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private int maxHP = 3;

    [Header("On Death (optional)")]
    [SerializeField] private GameObject deathExplosionPrefab;

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
        if (currentHP <= 0) Die();
    }

    public void Kill()
    {
        if (dead) return;
        Die();
    }

    private void Die()
    {
        dead = true;

        if (deathExplosionPrefab)
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}