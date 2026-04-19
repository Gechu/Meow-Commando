using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float lifeTime = 3f;
    public int damage = 1;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // tutaj możesz dodać obrażenia
            other.GetComponent<PlayerHealth>().TakeDamage(damage);

            Destroy(gameObject);
        }

        // znikaj po uderzeniu w ścianę
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
            return;
        }
    }
}