using UnityEngine;
using UnityEngine.InputSystem;

public abstract class RangedWeaponBase : WeaponBase
{
    [Header("Aim")]
    [SerializeField] protected Camera cam;

    [Header("Ammo")]
    [SerializeField] protected int magazineSize = 10;
    [SerializeField] protected float reloadTime = 1.2f;

    [Header("Shot")]
    [SerializeField] protected float bulletSpeed = 12f;
    [SerializeField] protected float timeBetweenShots = 0.2f;
    [SerializeField] protected float spreadAngle = 0f;

    protected int ammoInMag;
    protected float nextShotTime;
    protected bool isReloading;
    protected float reloadEndTime;

    protected virtual void Awake()
    {
        if (!cam) cam = Camera.main;
        ammoInMag = magazineSize;
    }

    public override void OnEquipped()
    {
        // po podniesieniu broni startowo pełny mag
        ammoInMag = magazineSize;
        isReloading = false;
        nextShotTime = 0f;
    }

    public override void TryShoot()
    {
        if (Mouse.current == null || cam == null) return;
        if (!bulletPrefab || !firePoint) return;

        TickReload();

        if (isReloading) return;

        if (Time.time < nextShotTime) return;

        if (ammoInMag <= 0)
        {
            StartReload();
            return;
        }

        Vector2 dir = WeaponUtils.GetMouseAimDir(cam, firePoint.position);
        if (dir == Vector2.zero) return;

        nextShotTime = Time.time + (timeBetweenShots * FireRateMultiplier);

        Fire(dir);

        ammoInMag--;
        if (ammoInMag <= 0)
        {
            StartReload();
        }
    }

    protected void StartReload()
    {
        if (isReloading) return;

        isReloading = true;
        reloadEndTime = Time.time + reloadTime;
    }

    protected void TickReload()
    {
        if (!isReloading) return;

        if (Time.time >= reloadEndTime)
        {
            isReloading = false;
            ammoInMag = magazineSize;
        }
    }

    protected abstract void Fire(Vector2 aimDir);
}