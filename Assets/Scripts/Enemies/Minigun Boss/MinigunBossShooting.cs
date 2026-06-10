using System.Collections;
using UnityEngine;

public class MinigunBossShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private MinigunBossAI bossAI;
    [SerializeField] private BossGunController armController;

    private Transform player;

    [Header("Minigun Spray Phase")]
    [SerializeField] private float sprayBulletSpeed = 10f;
    [SerializeField] private float sprayFireRate = 0.07f;
    [SerializeField] private int sprayBulletsPerVolley = 1;
    [SerializeField] private float spraySpreadAngle = 18f;

    [Header("Grenades Phase")]
    [SerializeField] private int grenadeCount = 6;
    [SerializeField] private float grenadeInitialSpeed = 7f;

    [Header("Arc Shotgun Phase")]
    [SerializeField] private int arcShotgunPellets = 12;
    [SerializeField] private float arcShotgunArcAngle = 115f;
    [SerializeField] private float arcShotgunPelletSpeed = 9f;
    [SerializeField] private int arcShotgunSalvos = 2;
    [SerializeField] private float arcShotgunSalvoDelay = 0.25f;

    private float sprayNextShotTime;
    private float arcNextShotTime;
    private Coroutine arcRoutine;

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    [Header("Last Seen Aim")]
    [SerializeField] private float lostTargetHoldTime = 4.0f;

    private Vector3 lockedAimPos;
    private float lastSeenTime;

    private MinigunBossAI.BossPhase currentPhase;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        lastSeenTime = -999f;
    }

    private void Update()
    {
        if (!player) return;

        if (HasLineOfSight2D(player.position))
        {
            lockedAimPos = player.position;
            lastSeenTime = Time.time;
        }

        if (currentPhase == MinigunBossAI.BossPhase.Transition)
            return;

        switch (currentPhase)
        {
            case MinigunBossAI.BossPhase.MinigunSpray:
                UpdateMinigunSpray();
                break;

            case MinigunBossAI.BossPhase.Grenades:
                UpdateGrenades();
                break;

            case MinigunBossAI.BossPhase.ArcShotgun:
                UpdateArcShotgun();
                break;
        }
    }

    public void OnPhaseChanged(MinigunBossAI.BossPhase newPhase)
    {
        currentPhase = newPhase;

        sprayNextShotTime = Time.time + 0.2f;
        arcNextShotTime = Time.time + 0.2f;

        if (arcRoutine != null)
        {
            StopCoroutine(arcRoutine);
            arcRoutine = null;
        }
    }

    // ---------------------------------------------------------
    //  MINIGUN SPRAY
    // ---------------------------------------------------------

    private void UpdateMinigunSpray()
    {
        if (Time.time < sprayNextShotTime) return;

        Vector2 aimDir;
        if (!TryGetAimDirection(out aimDir))
            return;

        for (int i = 0; i < sprayBulletsPerVolley; i++)
        {
            Vector2 dir = ApplySpread(aimDir, spraySpreadAngle);
            SpawnBullet(dir, sprayBulletSpeed);
        }

        sprayNextShotTime = Time.time + sprayFireRate;
    }

    // ---------------------------------------------------------
    //  GRENADES
    // ---------------------------------------------------------

    private void UpdateGrenades()
    {
        ThrowGrenadeCircle();
        bossAI.ForceEndPhase();
    }

    private void ThrowGrenadeCircle()
    {
        Vector3 center = bossAI.transform.position; // 🔥 granaty ze środka

        for (int i = 0; i < grenadeCount; i++)
        {
            float angle = (i / (float)grenadeCount) * 360f;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            Vector3 spawnPos = center + (Vector3)dir * 0.3f;

            GameObject g = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = g.GetComponent<Rigidbody2D>();

            if (rb)
                rb.linearVelocity = dir * grenadeInitialSpeed;
        }
    }

    // ---------------------------------------------------------
    //  ARC SHOTGUN
    // ---------------------------------------------------------

    private void UpdateArcShotgun()
    {
        if (Time.time < arcNextShotTime) return;

        if (arcRoutine != null)
            StopCoroutine(arcRoutine);

        arcRoutine = StartCoroutine(ArcRoutine());
        arcNextShotTime = Time.time + 1.6f;
    }

    private IEnumerator ArcRoutine()
    {
        Vector2 aimDir;
        if (!TryGetAimDirection(out aimDir))
            yield break;

        float baseAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - arcShotgunArcAngle / 2f;

        for (int s = 0; s < arcShotgunSalvos; s++)
        {
            for (int i = 0; i < arcShotgunPellets; i++)
            {
                float angle = startAngle + (i / (float)(arcShotgunPellets - 1)) * arcShotgunArcAngle;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                SpawnBullet(dir, arcShotgunPelletSpeed);
            }

            yield return new WaitForSeconds(arcShotgunSalvoDelay);
        }
    }

    // ---------------------------------------------------------
    //  AIMING HELPERS
    // ---------------------------------------------------------

    private bool TryGetAimDirection(out Vector2 dir)
    {
        dir = Vector2.right;

        bool hasLOS = HasLineOfSight2D(player.position);

        if (hasLOS)
        {
            dir = (player.position - firePoint.position).normalized;
            return true;
        }

        if (!HasValidAim())
            return false;

        dir = (lockedAimPos - firePoint.position).normalized;
        return true;
    }

    private bool HasValidAim()
    {
        if (lastSeenTime < -100f) return false;
        if (Time.time - lastSeenTime > lostTargetHoldTime) return false;
        return true;
    }

    private bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = firePoint.position;
        Vector2 toTarget = (Vector2)(targetPos - (Vector3)origin);
        float dist = toTarget.magnitude;

        if (dist <= 0.0001f) return true;

        Vector2 dir = toTarget / dist;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }

    // ---------------------------------------------------------
    //  BULLET SPAWNING
    // ---------------------------------------------------------

    private void SpawnBullet(Vector2 dir, float speed)
    {
        Transform fp = armController.GetFirePoint();
        GameObject b = Instantiate(bulletPrefab, fp.position, Quaternion.identity);
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();

        if (rb)
            rb.linearVelocity = dir * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        b.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private Vector2 ApplySpread(Vector2 dir, float spread)
    {
        float a = Random.Range(-spread, spread) * Mathf.Deg2Rad;
        float sin = Mathf.Sin(a);
        float cos = Mathf.Cos(a);

        return new Vector2(
            cos * dir.x - sin * dir.y,
            sin * dir.x + cos * dir.y
        );
    }
}