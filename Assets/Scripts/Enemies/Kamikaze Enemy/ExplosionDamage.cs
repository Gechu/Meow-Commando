using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Lifetime")]
    public float lifeTime = 0.6f;

    [Header("Audio")]
    public AudioClip explosionSound;

    private readonly HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    private void Start()
    {
        if (explosionSound)
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);

        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;

        if (hitObjects.Contains(root)) return;
        hitObjects.Add(root);

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Explosion hit Player for {damage}");

            // 🔥 ZADAJEMY OBRAŻENIA
            PlayerHealth hp = root.GetComponent<PlayerHealth>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Explosion hit Player, but PlayerHealth was not found!");
            }
        }
    }
}
