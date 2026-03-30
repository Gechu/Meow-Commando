using UnityEngine;

public class BulletDamage : MonoBehaviour
{
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
            Destroy(collision.gameObject);
            Destroy(gameObject);
        }
    }
}