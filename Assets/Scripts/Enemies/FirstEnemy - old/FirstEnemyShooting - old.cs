using UnityEngine;

public class FirstEnemyShootingOld : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float bulletSpeed = 10f;

    public float spreadAngle = 10f; 
    public float timeBetweenShots = 0.2f; 
    public float burstCooldown = 2f; 

    public int numofShots = 3;

    Transform player;
    bool isShooting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (!isShooting)
            StartCoroutine(ShootBurst());
    }

    System.Collections.IEnumerator ShootBurst()
    {
        isShooting = true;

        for (int i = 0; i < numofShots; i++)
        {
            ShootOneBullet();
            yield return new WaitForSeconds(timeBetweenShots);
        }

        // BURST COOLDOWN JITTER (80% – 120%)
        float jitter = Random.Range(burstCooldown * 0.8f, burstCooldown * 1.2f);
        yield return new WaitForSeconds(jitter);

        isShooting = false;
    }

    void ShootOneBullet()
    {
        if (player == null) return;

        // kierunek od końcówki lufy
        Vector2 dir = (player.position - firePoint.position).normalized;

        // rozrzut
        float angleOffset = Random.Range(-spreadAngle, spreadAngle);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;

        Vector2 finalDir = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        // pocisk
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.linearVelocity = finalDir * bulletSpeed;
    }
}