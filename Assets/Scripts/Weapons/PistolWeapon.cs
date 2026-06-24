using UnityEngine;

public class PistolWeapon : RangedWeaponBase
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    protected override void Awake()
    {
        base.Awake();
        magazineSize = 10;
        reloadTime = 1.1f;

        bulletSpeed = 14f;
        timeBetweenShots = 0.28f;
        spreadAngle = 1.5f;
    }

    protected override void Fire(Vector2 aimDir)
    {
        // Strzał pocisku
        Vector2 dir = WeaponUtils.ApplySpread(aimDir, spreadAngle);
        WeaponUtils.SpawnBullet(bulletPrefab, firePoint, dir, bulletSpeed * BulletSpeedMultiplier);

        // Dźwięk strzału
        if (audioSource && shootSound)
            audioSource.PlayOneShot(shootSound);
    }
}
