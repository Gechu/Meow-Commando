using UnityEngine;

public class SniperEnemyShooting : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public LineRenderer laser;

    [Header("Shot")]
    public float bulletSpeed = 35f;
    public float cooldownAfterShot = 2.0f;

    [Header("Charge")]
    public float chargeTime = 2.0f;

    [Header("Line of Sight (2D)")]
    public LayerMask wallMask;
    public float losExtra = 0.05f;

    public bool IsCharging => isCharging;

    private Transform player;

    private bool isCharging;
    private float chargeTimer;
    private float nextAllowedTime;

    private Vector3 lockedAimPos;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (laser) laser.enabled = false;
        nextAllowedTime = 0f;
    }

    private void Update()
    {
        if (!player || !bulletPrefab || !firePoint) return;

        if (Time.time < nextAllowedTime)
        {
            StopCharging();
            return;
        }

        bool hasLOS = HasLineOfSight2D(player.position);

        // Jeśli nie ładujemy: start tylko gdy jest LOS
        if (!isCharging)
        {
            if (!hasLOS)
            {
                if (laser) laser.enabled = false;
                return;
            }

            // start charge
            isCharging = true;
            chargeTimer = chargeTime;
            lockedAimPos = player.position; // lock na start
            UpdateLaser(lockedAimPos, true);
            return;
        }

        // Jeśli ładujemy:
        // - jak straci LOS -> przerwij całkowicie
        if (!hasLOS)
        {
            StopCharging();
            return;
        }

        // ma LOS -> aktualizuj aim i laser
        lockedAimPos = player.position;
        UpdateLaser(lockedAimPos, true);

        chargeTimer -= Time.deltaTime;
        if (chargeTimer > 0f) return;

        Fire(lockedAimPos);
        nextAllowedTime = Time.time + cooldownAfterShot;

        StopCharging();
    }

    private void Fire(Vector3 aimPos)
    {
        Vector2 toAim = (Vector2)(aimPos - firePoint.position);
        if (toAim.sqrMagnitude < 0.0001f) toAim = Vector2.right;

        Vector2 dir = toAim.normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * bulletSpeed;
    }

    private void StopCharging()
    {
        isCharging = false;
        if (laser) laser.enabled = false;
    }

    private void UpdateLaser(Vector3 aimPos, bool enable)
    {
        if (!laser) return;

        laser.enabled = enable;
        laser.positionCount = 2;
        laser.SetPosition(0, firePoint.position);
        laser.SetPosition(1, aimPos);
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
}