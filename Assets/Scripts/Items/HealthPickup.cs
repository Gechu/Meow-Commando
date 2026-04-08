using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public int healAmount = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerHealth player = collision.GetComponentInParent<PlayerHealth>();

        if (player != null)
        {
            if (player.currentHP < player.maxHP)
            {
                player.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}