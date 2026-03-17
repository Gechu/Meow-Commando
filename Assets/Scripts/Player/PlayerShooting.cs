using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    void Update()
    {
        if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        Vector2 direction = (mousePos - transform.position).normalized;

        Vector2 spawnPos = (Vector2)transform.position + direction * 1f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Destroy(bullet, 2f);

        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
    }
}