using UnityEngine;

public class UZIWeapon : RangedWeaponBase
{
    protected override void Awake()
    {
        base.Awake();
        magazineSize = 30;
        reloadTime = 1.6f;

        bulletSpeed = 13f;
        timeBetweenShots = 0.08f;   // ~12.5 strzału/s
        spreadAngle = 5.0f;         // UZI ma “spray”
    }

    protected override void Fire(Vector2 aimDir)
    {
        Vector2 dir = WeaponUtils.ApplySpread(aimDir, spreadAngle);
        WeaponUtils.SpawnBullet(bulletPrefab, firePoint, dir, bulletSpeed * BulletSpeedMultiplier);
    }
}