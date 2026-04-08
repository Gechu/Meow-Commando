using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShooting : MonoBehaviour
{
    private PlayerMovement movement;
    public GameObject bulletPrefab;
    public float bulletSpeed = 10f;

    [Header("Fire Rate")]
    public float fireRate = 0.5f;
    private float fireTimer = 0f;

    void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        fireTimer -= Time.deltaTime;

        if (Mouse.current.leftButton.isPressed && fireTimer <= 0f && !movement.IsDashing)
        {
            Shoot();
            fireTimer = fireRate;
        }
    }

    void Shoot()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 direction = (mousePos - transform.position).normalized;

        Vector2 spawnPos = (Vector2)transform.position + direction * 1f;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        Destroy(bullet, 2f);

        bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * bulletSpeed;
    }
}