using UnityEngine;
using System.Collections;

public enum CorgiShootMode
{
    None,
    Pistol,
    FinalMachineGun,
    FinalWave
}

public class DonCorgiShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Pistol")]
    public float pistolCooldown = 0.6f;
    public float pistolBulletSpeed = 10f;

    [Header("Machine Gun")]
    public float mgCooldown = 0.1f;
    public float mgBulletSpeed = 12f;

    [Header("Wave Attack")]
    public int waveBulletCount = 12;
    public float waveCooldown = 3f;
    public float waveBulletSpeed = 8f;

    private Transform player;
    private bool isShooting = false;

    public CorgiShootMode mode = CorgiShootMode.None;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player || isShooting) return;

        switch (mode)
        {
            case CorgiShootMode.Pistol:
                StartCoroutine(PistolShot());
                break;

            case CorgiShootMode.FinalMachineGun:
                StartCoroutine(MachineGunBurst());
                break;

            case CorgiShootMode.FinalWave:
                StartCoroutine(WaveAttack());
                break;
        }
    }

    IEnumerator PistolShot()
    {
        isShooting = true;
        ShootTowardsPlayer(pistolBulletSpeed);
        yield return new WaitForSeconds(pistolCooldown);
        isShooting = false;
    }

    IEnumerator MachineGunBurst()
    {
        isShooting = true;
        ShootTowardsPlayer(mgBulletSpeed);
        yield return new WaitForSeconds(mgCooldown);
        isShooting = false;
    }

    IEnumerator WaveAttack()
    {
        isShooting = true;

        float angleStep = 360f / waveBulletCount;
        float angle = 0f;

        for (int i = 0; i < waveBulletCount; i++)
        {
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 dir = new Vector2(bulletDirX, bulletDirY).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb)
                rb.linearVelocity = dir * waveBulletSpeed;

            // Obrót pocisku w kierunku lotu
            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot);

            angle += angleStep;
        }

        yield return new WaitForSeconds(waveCooldown);
        isShooting = false;
    }


    void ShootTowardsPlayer(float speed)
    {
        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb)
            rb.linearVelocity = dir * speed;

        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot);
    }
}
