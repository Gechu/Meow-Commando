using UnityEngine;

public class SniperRifleWeapon : RangedWeaponBase
{
    [Header("Sniper feel")]
    [SerializeField] private float extraPostShotDelay = 0.35f; // “bolt action feel”

    protected override void Awake()
    {
        base.Awake();
        magazineSize = 1;
        reloadTime = 1.6f;

        bulletSpeed = 28f;
        timeBetweenShots = 0.15f; // techniczny cooldown, a i tak mag 1 + reload
        spreadAngle = 0.2f;       // prawie zero
    }

    protected override void Fire(Vector2 aimDir)
    {
        Vector2 dir = WeaponUtils.ApplySpread(aimDir, spreadAngle);
        WeaponUtils.SpawnBullet(bulletPrefab, firePoint, dir, bulletSpeed * BulletSpeedMultiplier);

        // po strzale dodaj “bolt delay”
        nextShotTime = Mathf.Max(nextShotTime, Time.time + extraPostShotDelay);
    }
}