using UnityEngine;
using UnityEngine.InputSystem;

public class PistolWeapon : WeaponBase
{
    [SerializeField] private Camera cam;
    [SerializeField] private float bulletSpeed = 12f;
    [SerializeField] private float timeBetweenShots = 0.18f;
    [SerializeField] private float spreadAngle = 2f;

    private float nextShotTime;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    public override void TryShoot()
    {
        if (Time.time < nextShotTime) return;
        if (!bulletPrefab || !firePoint) return;
        if (Mouse.current == null || cam == null) return;

        nextShotTime = Time.time + timeBetweenShots;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        mouseWorld.z = firePoint.position.z;

        Vector2 dir = (mouseWorld - firePoint.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir = ApplySpread(dir.normalized, spreadAngle);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * bulletSpeed;
    }

    private static Vector2 ApplySpread(Vector2 dir, float spread)
    {
        if (spread <= 0f) return dir;
        float a = Random.Range(-spread, spread);
        float rad = a * Mathf.Deg2Rad;
        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);
        return new Vector2(cos * dir.x - sin * dir.y, sin * dir.x + cos * dir.y);
    }
}