using UnityEngine;

public class MachineGunWeapon : RangedWeaponBase
{
    [Header("Recoil")]
    [SerializeField] private float recoilIncreasePerShot = 0.6f;
    [SerializeField] private float recoilMax = 10f;
    [SerializeField] private float recoilRecoverPerSec = 8f;

    private float recoil;

    protected override void Awake()
    {
        base.Awake();
        magazineSize = 30;
        reloadTime = 1.9f;

        bulletSpeed = 15f;
        timeBetweenShots = 0.10f; // 10 strzałów/s
        spreadAngle = 3.0f;       // bazowy
    }

    private void Update()
    {
        // recoil wraca powoli w dół jak nie strzelasz
        recoil = Mathf.MoveTowards(recoil, 0f, recoilRecoverPerSec * Time.deltaTime);
    }

    protected override void Fire(Vector2 aimDir)
    {
        float currentSpread = Mathf.Min(recoilMax, spreadAngle + recoil);
        Vector2 dir = WeaponUtils.ApplySpread(aimDir, currentSpread);

        WeaponUtils.SpawnBullet(bulletPrefab, firePoint, dir, bulletSpeed * BulletSpeedMultiplier);

        recoil += recoilIncreasePerShot;
    }
}