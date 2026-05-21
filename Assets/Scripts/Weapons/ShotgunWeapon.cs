using UnityEngine;

public class ShotgunWeapon : RangedWeaponBase
{
    [Header("Pellets")]
    [SerializeField] private int pellets = 8;
    [SerializeField] private float pelletSpreadAngle = 28f; // szeroko
    [SerializeField] private float pelletSpeedJitter = 0.15f;

    protected override void Awake()
    {
        base.Awake();
        magazineSize = 3;
        reloadTime = 2.0f;

        bulletSpeed = 12f;
        timeBetweenShots = 0.85f;
        spreadAngle = 0f; // używamy pelletSpreadAngle
    }

    protected override void Fire(Vector2 aimDir)
    {
        for (int i = 0; i < pellets; i++)
        {
            Vector2 dir = WeaponUtils.ApplySpread(aimDir, pelletSpreadAngle);

            float speedMul = Random.Range(1f - pelletSpeedJitter, 1f + pelletSpeedJitter);
            WeaponUtils.SpawnBullet(bulletPrefab, firePoint, dir, bulletSpeed * speedMul * BulletSpeedMultiplier);
        }
    }
}