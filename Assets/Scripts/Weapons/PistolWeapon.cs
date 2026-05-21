using UnityEngine;

public class PistolWeapon : RangedWeaponBase
{
    protected override void Awake()
    {
        base.Awake();
        magazineSize = 10;
        reloadTime = 1.1f;

        bulletSpeed = 14f;
        timeBetweenShots = 0.28f;   // ~3.6 strzału/s
        spreadAngle = 1.5f;         // lekki rozrzut
    }

    protected override void Fire(Vector2 aimDir)
    {
        Vector2 dir = WeaponUtils.ApplySpread(aimDir, spreadAngle);
        WeaponUtils.SpawnBullet(bulletPrefab, firePoint, dir, bulletSpeed * BulletSpeedMultiplier);
    }
}