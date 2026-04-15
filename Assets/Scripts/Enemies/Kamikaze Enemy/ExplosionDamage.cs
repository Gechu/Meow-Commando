using System.Collections.Generic;
using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [Header("Damage")]
    public int damage = 1;

    [Header("Lifetime")]
    public float lifeTime = 0.15f; // krótko, żeby tylko "pyknęło"

    private readonly HashSet<GameObject> hitObjects = new HashSet<GameObject>();

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // uderz tylko raz na obiekt
        GameObject root = other.attachedRigidbody ? other.attachedRigidbody.gameObject : other.gameObject;
        if (hitObjects.Contains(root)) return;
        hitObjects.Add(root);

        if (other.CompareTag("Player"))
        {
            // TODO: jak będziesz miał PlayerHP, podmienisz na TakeDamage
            // other.GetComponentInParent<PlayerHP>()?.TakeDamage(damage);
            Debug.Log($"Explosion hit Player for {damage}");
        }
    }
}