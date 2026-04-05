using System.Collections;
using UnityEngine;

public class GrenadierEnemyShooting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject grenadePrefab;

    [Tooltip("Obiekt (sprite) granatu w ręce. Będzie włączany/wyłączany.")]
    [SerializeField] private GameObject heldGrenadeVisual;

    [Header("Throw")]
    [SerializeField] private float throwSpeed = 9f;

    [Tooltip("Sekundy po wybuchu poprzedniego granatu zanim ma kolejny w ręce i może rzucić.")]
    [SerializeField] private float cooldownAfterExplode = 2.0f;

    [Header("Aim assist when NO LOS (corner offset)")]
    [Tooltip("Offset celu (w jednostkach świata) dodawany TYLKO gdy nie ma LOS. Pomaga rzucać 'za róg'.")]
    [SerializeField] private float noLosAimOffset = 1.2f;

    [Tooltip("Losowy jitter offsetu: 0.25 = +/-25%")]
    [Range(0f, 1f)]
    [SerializeField] private float noLosAimOffsetJitter = 0.25f;

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    [Header("Last seen aim")]
    [Tooltip("Jak długo (sekundy) po utracie LOS wolno jeszcze rzucać w last-seen. Infinity = zawsze.")]
    [SerializeField] private float lostTargetHoldTime = 4.0f;

    private Transform player;

    private Vector3 lockedAimPos;
    private float lastSeenTime;

    private bool isThrowingOrWaiting;

    public bool CanThrow => !isThrowingOrWaiting && HasValidAim();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // startowo: nie zakładamy, że widzi gracza
        lastSeenTime = -999f;

        if (heldGrenadeVisual) heldGrenadeVisual.SetActive(true);
    }

    private void Update()
    {
        if (!player) return;

        // aktualizuj last seen tylko gdy LOS
        if (HasLineOfSight2D(player.position))
        {
            lockedAimPos = player.position;
            lastSeenTime = Time.time;
        }
    }

    public void TryThrow()
    {
        if (!CanThrow) return;
        StartCoroutine(ThrowRoutine());
    }

    private IEnumerator ThrowRoutine()
    {
        isThrowingOrWaiting = true;

        // ukryj granat w ręce
        if (heldGrenadeVisual) heldGrenadeVisual.SetActive(false);

        Vector3 spawnPos = throwPoint ? throwPoint.position : transform.position;

        // Aim:
        // - jeśli aktualnie ma LOS -> rzucaj prosto w gracza
        // - jeśli nie ma LOS -> rzucaj w lockedAimPos + offset w bok (żeby częściej poleciał "za róg")
        Vector3 aimPos = lockedAimPos;

        bool hasLOSNow = player && HasLineOfSight2D(player.position);
        if (hasLOSNow)
        {
            aimPos = player.position; // prosto w gracza
        }
        else
        {
            aimPos = ApplyNoLOSAimOffset(lockedAimPos, spawnPos);
        }

        GameObject grenade = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);

        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb)
        {
            Vector2 dir = (Vector2)(aimPos - spawnPos);
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
            rb.linearVelocity = dir.normalized * throwSpeed;
        }

        // czekaj aż granat zniknie (wybuch -> Destroy)
        while (grenade != null)
            yield return null;

        yield return new WaitForSeconds(cooldownAfterExplode);

        if (heldGrenadeVisual) heldGrenadeVisual.SetActive(true);

        isThrowingOrWaiting = false;
    }

    private Vector3 ApplyNoLOSAimOffset(Vector3 baseAimPos, Vector3 fromPos)
    {
        // Kierunek "do celu"
        Vector2 toAim = (Vector2)(baseAimPos - fromPos);
        if (toAim.sqrMagnitude < 0.0001f)
            toAim = Vector2.right;

        Vector2 dir = toAim.normalized;

        // prostopadły (lewo/prawo)
        Vector2 perp = new Vector2(-dir.y, dir.x);
        float side = (Random.value > 0.5f) ? 1f : -1f;

        float jitter = Random.Range(1f - noLosAimOffsetJitter, 1f + noLosAimOffsetJitter);
        float offset = noLosAimOffset * jitter;

        return baseAimPos + (Vector3)(perp * side * offset);
    }

    private bool HasValidAim()
    {
        // nie widział gracza jeszcze nigdy
        if (lastSeenTime < -100f) return false;

        if (Time.time - lastSeenTime > lostTargetHoldTime)
            return false;

        return true;
    }

    private bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = throwPoint ? (Vector2)throwPoint.position : (Vector2)transform.position;
        Vector2 toTarget = (Vector2)(targetPos - (Vector3)origin);
        float dist = toTarget.magnitude;

        if (dist <= 0.0001f) return true;

        Vector2 dir = toTarget / dist;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }
}