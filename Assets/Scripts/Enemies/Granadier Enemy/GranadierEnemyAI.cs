using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class GrenadierEnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GrenadierEnemyShooting shooting;

    [Header("Distance (keep mid range)")]
    [SerializeField] private float minDistance = 9f;
    [SerializeField] private float maxDistance = 14f;

    [Header("Throw range")]
    [SerializeField] private float minThrowDistance = 7f;
    [SerializeField] private float maxThrowDistance = 18f;

    [Header("Movement")]
    [SerializeField] private float retreatStep = 2.0f;
    [SerializeField] private float repathInterval = 0.25f;

    [Header("When NO LOS (reposition)")]
    [SerializeField] private float repositionInterval = 1.2f;
    [SerializeField] private float repositionDistance = 4.5f;

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    private NavMeshAgent agent;
    private Transform player;

    private float repathTimer;
    private float repositionTimer;
    private float strafeSide = 1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = true;
        agent.stoppingDistance = 0f;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (!shooting) shooting = GetComponentInChildren<GrenadierEnemyShooting>();

        strafeSide = (Random.value > 0.5f) ? 1f : -1f;
    }

    private void Update()
    {
        if (!player) return;

        repathTimer -= Time.deltaTime;
        repositionTimer -= Time.deltaTime;

        if (repathTimer > 0f) return;
        repathTimer = repathInterval;

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        Vector2 dir = (dist > 0.001f) ? (toPlayer / dist) : Vector2.right;

        bool hasLOS = HasLineOfSight2D(player.position);

        // 1) Trzymanie dystansu zawsze ma priorytet (żeby nie wchodził na twarz)
        if (dist < minDistance)
        {
            Vector3 desired = transform.position - (Vector3)(dir * retreatStep);
            SetDest(desired);
        }
        else if (dist > maxDistance)
        {
            // jak za daleko, podejdź (nawet jak nie ma LOS, bo może znaleźć przejście)
            SetDest(player.position);
        }
        else
        {
            // 2) Idealny dystans:
            // - jeśli ma LOS -> stoi
            // - jeśli nie ma LOS -> reposition (żeby nie stał jak słup)
            if (hasLOS)
            {
                StopAgent();
            }
            else
            {
                if (repositionTimer <= 0f)
                {
                    repositionTimer = repositionInterval;
                    Reposition(dir);
                }
            }
        }

        // 3) Rzut (nie wymuszamy LOS, bo rzuca też w last-seen przez shooting)
        if (shooting && dist >= minThrowDistance && dist <= maxThrowDistance)
            shooting.TryThrow();

        transform.rotation = Quaternion.identity;
    }

    private void Reposition(Vector2 dirToPlayer)
    {
        // idź trochę w bok (szukanie kąta na LOS)
        Vector2 perp = new Vector2(-dirToPlayer.y, dirToPlayer.x) * strafeSide;

        // raz na jakiś czas zmień stronę
        if (Random.value < 0.35f)
            strafeSide *= -1f;

        Vector3 candidate = transform.position + (Vector3)(perp.normalized * repositionDistance);
        SetDest(candidate);
    }

    private void SetDest(Vector3 desiredPoint)
    {
        if (NavMesh.SamplePosition(desiredPoint, out var hit, 3f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            StopAgent();
    }

    private void StopAgent()
    {
        if (agent.hasPath) agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    private bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = transform.position;
        Vector2 toTarget = (Vector2)(targetPos - transform.position);
        float dist = toTarget.magnitude;

        if (dist <= 0.0001f) return true;

        Vector2 dir = toTarget / dist;
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }
}