using UnityEngine;

public static class WeaponUtils
{
    public static Vector2 GetMouseAimDir(Camera cam, Vector3 fromWorldPos)
    {
        if (!cam) return Vector2.zero;

        Vector2 mouseScreen = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        mouseWorld.z = fromWorldPos.z;

        Vector2 dir = (mouseWorld - fromWorldPos);
        if (dir.sqrMagnitude < 0.0001f) return Vector2.zero;
        return dir.normalized;
    }

    public static Vector2 ApplySpread(Vector2 dir, float spreadDegrees)
    {
        if (spreadDegrees <= 0f) return dir;

        float a = Random.Range(-spreadDegrees, spreadDegrees);
        float rad = a * Mathf.Deg2Rad;

        float sin = Mathf.Sin(rad);
        float cos = Mathf.Cos(rad);

        return new Vector2(
            cos * dir.x - sin * dir.y,
            sin * dir.x + cos * dir.y
        );
    }

    public static void SpawnBullet(GameObject bulletPrefab, Transform firePoint, Vector2 dir, float bulletSpeed)
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Object.Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * bulletSpeed;
    }
}