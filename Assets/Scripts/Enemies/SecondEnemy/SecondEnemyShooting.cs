using UnityEngine;

public class EnemyShootingLOS : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 7f;

    [Header("Aim")]
    public float spreadAngle = 10f;

    [Header("Fire Rate (single shots)")]
    public float timeBetweenShots = 0.6f;
    [Tooltip("Losowy jitter: 0.1 = +/-10%")]
    public float shotJitterPercent = 0.15f;

    [Header("Line of Sight (2D)")]
    public LayerMask wallMask;     // ustaw warstwę ścian (2D collidery)
    public float losExtra = 0.05f; // mały zapas

    private Transform player;
    private float nextShotTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ScheduleNextShot();
    }

    void Update()
    {
        if (!player) return;
        if (Time.time < nextShotTime) return;

        // Strzelaj tylko jeśli widzi gracza (brak ściany)
        if (HasLineOfSight2D(player.position))
        {
            ShootOneBullet();
        }

        ScheduleNextShot();
    }

    void ScheduleNextShot()
    {
        float jitter = Random.Range(1f - shotJitterPercent, 1f + shotJitterPercent);
        nextShotTime = Time.time + timeBetweenShots * jitter;
    }

    bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        Vector2 toTarget = (Vector2)(targetPos - (Vector3)origin);
        float dist = toTarget.magnitude;

        if (dist <= 0.0001f) return true;

        Vector2 dir = toTarget / dist;

        // jeśli raycast trafi w ścianę zanim dotrze do gracza -> brak LOS
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }

    void ShootOneBullet()
    {
        if (!player || !bulletPrefab || !firePoint) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        float angleOffset = Random.Range(-spreadAngle, spreadAngle);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;

        Vector2 finalDir = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb)
            rb.linearVelocity = finalDir * bulletSpeed;
    }
}