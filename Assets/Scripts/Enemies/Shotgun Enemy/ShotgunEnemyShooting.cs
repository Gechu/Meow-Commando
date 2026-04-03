using System;
using UnityEngine;

public class ShotgunShooting : MonoBehaviour
{
    public event Action OnShotFired;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;

    [Header("Range")]
    [SerializeField] private float shootRange = 9f;

    [Header("Aim / Windup")]
    [SerializeField] private float aimTime = 0.7f;

    [Header("Cooldown (from shot moment)")]
    [SerializeField] private float shotCooldown = 1.1f;
    [SerializeField] private float cooldownJitterPercent = 0.12f;

    [Header("Pellets Count")]
    [SerializeField] private int pelletsMin = 10;
    [SerializeField] private int pelletsMax = 12;

    [Header("Spread - Core + Outer")]
    [Tooltip("Ile procent pelletów ma być w rdzeniu (mały rozrzut).")]
    [Range(0f, 1f)]
    [SerializeField] private float corePelletRatio = 0.65f;

    [Tooltip("Rozrzut rdzenia (stopnie).")]
    [SerializeField] private float coreSpreadAngle = 10f;

    [Tooltip("Rozrzut zewnętrzny (stopnie).")]
    [SerializeField] private float outerSpreadAngle = 35f;

    [Header("Pellet Speed")]
    [SerializeField] private float pelletSpeedMin = 6f;
    [SerializeField] private float pelletSpeedMax = 9f;

    [Header("Pellet Lifetime")]
    [SerializeField] private float pelletLifetime = 1.1f;

    public bool IsAiming => isAiming;

    private bool isAiming;
    private float aimTimer;

    private Vector3 lockedAimPos;
    private float nextAllowedShotTime;

    public bool CanStartAiming(Vector3 playerPos)
    {
        if (isAiming) return false;
        if (Time.time < nextAllowedShotTime) return false;

        Vector2 origin = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        float dist = Vector2.Distance(origin, playerPos);
        return dist <= shootRange;
    }

    public void StartAiming(Vector3 initialPlayerPos)
    {
        if (!firePoint || !bulletPrefab) return;

        isAiming = true;
        aimTimer = aimTime;
        lockedAimPos = initialPlayerPos; // lock na starcie
    }

    public void UpdateLockedAim(Vector3 playerPos)
    {
        if (!isAiming) return;
        lockedAimPos = playerPos; // aktualizuj tylko gdy AI mówi, że jest LOS
    }

    private void Update()
    {
        if (!isAiming) return;

        aimTimer -= Time.deltaTime;
        if (aimTimer > 0f) return;

        FireShotgun(lockedAimPos);

        isAiming = false;

        float jitter = UnityEngine.Random.Range(1f - cooldownJitterPercent, 1f + cooldownJitterPercent);
        nextAllowedShotTime = Time.time + shotCooldown * jitter;

        OnShotFired?.Invoke();
    }

    private void FireShotgun(Vector3 aimWorldPos)
    {
        if (!firePoint || !bulletPrefab) return;

        Vector2 toAim = (aimWorldPos - firePoint.position);
        if (toAim.sqrMagnitude < 0.0001f) toAim = Vector2.right;

        Vector2 baseDir = toAim.normalized;

        int pelletCount = UnityEngine.Random.Range(pelletsMin, pelletsMax + 1);
        int coreCount = Mathf.Clamp(Mathf.RoundToInt(pelletCount * corePelletRatio), 0, pelletCount);
        int outerCount = pelletCount - coreCount;

        // 1) Rdzeń: mały rozrzut (bardziej groźny / częściej trafia)
        for (int i = 0; i < coreCount; i++)
            SpawnPellet(baseDir, coreSpreadAngle);

        // 2) Zewnętrzne: większy rozrzut (ładny “wachlarz”)
        for (int i = 0; i < outerCount; i++)
            SpawnPellet(baseDir, outerSpreadAngle);
    }

    private void SpawnPellet(Vector2 baseDir, float spreadAngle)
    {
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float angleOffset = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        float angle = baseAngle + angleOffset;

        Vector2 dir = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        float speed = UnityEngine.Random.Range(pelletSpeedMin, pelletSpeedMax);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb) rb.linearVelocity = dir * speed;

        Destroy(bullet, pelletLifetime);
    }
}