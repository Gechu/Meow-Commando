using UnityEngine;

public class CatnipPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Dotkniêto: " + collision.name);

        CatnipBehavior player = collision.GetComponentInParent<CatnipBehavior>();

        if (player != null)
        {
            // Debug.Log("Zebrano power-up! Iloœæ:" + player.catnipCount);
            player.AddCatnip(1);
            Destroy(gameObject);
        }
    }
}