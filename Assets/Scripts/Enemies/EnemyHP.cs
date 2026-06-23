using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    [SerializeField] private float maxHP = 3;

    [Header("On Death (optional)")]
    [SerializeField] private GameObject deathExplosionPrefab;

    private float currentHP;
    private bool dead;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
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

        GetComponent<EnemyDeathNotifier>()?.NotifyDeath();

        if (deathExplosionPrefab)
            Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}