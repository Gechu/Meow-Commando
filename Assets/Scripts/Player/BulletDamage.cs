using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [SerializeField] private float damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // znikaj po uderzeniu w ścianę
        if (collision.CompareTag("Wall") || collision.CompareTag("Throne"))
        {
            Destroy(gameObject);
            return;
        }

        // najpierw sprawdź czy to boss
        BossHealth bossHp = collision.GetComponentInParent<BossHealth>();
        if (bossHp != null)
        {
            bossHp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // potem zwykły enemy (Twoje istniejące EnemyHP)
        EnemyHP enemyHp = collision.GetComponentInParent<EnemyHP>();
        if (enemyHp != null)
        {
            enemyHp.TakeDamage((int)damage);
            Destroy(gameObject);
            return;
        }

        // opcjonalnie: znikaj po trafieniu w inne obiekty z tagiem "Obstacle" itp.
    }

}