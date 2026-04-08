using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    [SerializeField] private int damage = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // znikaj po uderzeniu w ścianę
        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.CompareTag("Enemy"))
        {
            // Spróbuj znaleźć HP na trafionym obiekcie albo jego rodzicu
            EnemyHP hp = collision.GetComponentInParent<EnemyHP>();
            if (hp != null)
                hp.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}