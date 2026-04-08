using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform firePoint; // opcjonalnie
    private Transform player;
    private Transform gun;

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    [Header("Last seen aim")]
    [SerializeField] private float lostTargetHoldTime = Mathf.Infinity;

    [Header("Rotation smoothing")]
    [Tooltip("Prędkość obrotu pivotu w stopniach na sekundę. Daj np. 720-1440.")]
    [SerializeField] private float turnSpeedDegPerSec = 1080f;

    private Vector3 gunOriginalScale;

    private Vector3 lastSeenPos;
    private float lastSeenTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        gun = transform.GetChild(0);
        gunOriginalScale = gun.localScale;

        if (player)
        {
            lastSeenPos = player.position;
            lastSeenTime = Time.time;
        }
    }

    void Update()
    {
        if (!player) return;

        if (HasLineOfSight2D(player.position))
        {
            lastSeenPos = player.position;
            lastSeenTime = Time.time;
        }

        if (Time.time - lastSeenTime > lostTargetHoldTime)
            return;

        Vector2 dir = (Vector2)(lastSeenPos - transform.position);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // SMOOTH: zamiast teleportu - obrót z max prędkością
        float currentAngle = transform.eulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turnSpeedDegPerSec * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0, 0, newAngle);

        // flip (możesz też użyć targetAngle zamiast newAngle — ja wolę newAngle żeby flip był spójny z ruchem)
        if (newAngle > 90 || newAngle < -90)
            gun.localScale = new Vector3(gunOriginalScale.x, -gunOriginalScale.y, 1);
        else
            gun.localScale = gunOriginalScale;
    }

    bool HasLineOfSight2D(Vector3 targetPos)
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