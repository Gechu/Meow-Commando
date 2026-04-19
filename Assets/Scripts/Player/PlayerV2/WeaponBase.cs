using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Common")]
    [SerializeField] protected Transform firePoint;
    [SerializeField] protected GameObject bulletPrefab;

    public float BulletSpeedMultiplier { get; set; } = 1f;
    public float FireRateMultiplier { get; set; } = 1f;

    public Transform FirePoint => firePoint;

    // Player będzie wołał to przy trzymaniu LPM
    public abstract void TryShoot();

    // (opcjonalnie) gdy zakładasz broń
    public virtual void OnEquipped() { }

    // (opcjonalnie) gdy zdejmujesz broń
    public virtual void OnUnequipped() { }
}