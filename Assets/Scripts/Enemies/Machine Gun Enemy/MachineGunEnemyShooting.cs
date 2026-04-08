using UnityEngine;

public class MachineGunEnemyShooting : MonoBehaviour
{
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 10f;

    [Header("Range")]
    public float shootRange = 16f;

    [Header("Burst")]
    public int shotsPerBurst = 10;
    public float timeBetweenShots = 0.11f;
    public float burstCooldown = 1.6f;
    [Tooltip("Losowy jitter cooldownu: 0.15 = +/-15%")]
    public float cooldownJitterPercent = 0.15f;

    [Header("Spread (recoil)")]
    [Tooltip("Bazowy rozrzut (stopnie).")]
    public float baseSpreadAngle = 14f;

    [Tooltip("Ile stopni rozrzutu dodawać na każdy kolejny strzał w serii.")]
    public float recoilPerShot = 0.7f;

    [Header("Line of Sight (2D)")]
    public LayerMask wallMask;
    public float losExtra = 0.05f;

    private Transform player;

    private bool isBursting;
    private float nextBurstTime;

    // "last seen" aim
    private Vector3 lockedAimPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        nextBurstTime = 0f;
    }

    private void Update()
    {
        if (!player) return;
        if (isBursting) return;
        if (Time.time < nextBurstTime) return;

        // start burst only if in range + has LOS
        if (!IsPlayerInRange()) return;

        if (HasLineOfSight2D(player.position))
        {
            lockedAimPos = player.position; // lock na start
            StartCoroutine(BurstRoutine());
        }
    }

    private System.Collections.IEnumerator BurstRoutine()
    {
        isBursting = true;

        for (int i = 0; i < shotsPerBurst; i++)
        {
            // jeśli ma LOS -> aktualizuj, inaczej strzelaj w ostatnie miejsce
            if (HasLineOfSight2D(player.position))
                lockedAimPos = player.position;

            ShootOneBullet(i, lockedAimPos);

            yield return new WaitForSeconds(timeBetweenShots);
        }

        float jitter = Random.Range(1f - cooldownJitterPercent, 1f + cooldownJitterPercent);
        nextBurstTime = Time.time + burstCooldown * jitter;

        isBursting = false;
    }

    private bool IsPlayerInRange()
    {
        Vector2 origin = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        float dist = Vector2.Distance(origin, player.position);
        return dist <= shootRange;
    }

    private bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        Vector2 toTarget = (Vector2)(targetPos - (Vector3)origin);
        float dist = toTarget.magnitude;

        if (dist <= 0.0001f) return true;

        Vector2 dir = toTarget / dist;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }

    private void ShootOneBullet(int shotIndex, Vector3 aimPos)
    {
        if (!bulletPrefab || !firePoint) return;

        Vector2 toAim = (Vector2)(aimPos - firePoint.position);
        if (toAim.sqrMagnitude < 0.0001f) toAim = Vector2.right;

        Vector2 dir = toAim.normalized;

        // recoil: rozrzut rośnie z każdym strzałem
        float spread = baseSpreadAngle + recoilPerShot * shotIndex;

        float angleOffset = Random.Range(-spread, spread);
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