using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SniperEnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SniperEnemyShooting shooting;

    [Header("Distance (keep far)")]
    [SerializeField] private float minDistance = 14f;
    [SerializeField] private float maxDistance = 20f;

    [Header("Repath")]
    [SerializeField] private float repathInterval = 0.25f;

    [Header("Reposition when no LOS")]
    [SerializeField] private float repositionInterval = 1.0f;
    [SerializeField] private float repositionDistance = 6f; // jak daleko szuka punktu

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    private NavMeshAgent agent;
    private Transform player;

    private float repathTimer;
    private float repositionTimer;

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
        if (!shooting) shooting = GetComponent<SniperEnemyShooting>();
    }

    private void Update()
    {
        if (!player) return;

        // jeśli sniper ładuje strzał -> stoi (najbardziej fair + czytelne)
        if (shooting && shooting.IsCharging)
        {
            StopAgent();
            transform.rotation = Quaternion.identity;
            return;
        }

        repathTimer -= Time.deltaTime;
        repositionTimer -= Time.deltaTime;

        if (repathTimer > 0f) return;
        repathTimer = repathInterval;

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        Vector2 dir = (dist > 0.001f) ? (toPlayer / dist) : Vector2.right;

        bool hasLOS = HasLineOfSight2D(player.position);

        // 1) jeśli za blisko -> uciekaj (priorytet)
        if (dist < minDistance)
        {
            Vector3 fleeTarget = transform.position - (Vector3)(dir * (minDistance - dist + 4f));
            SetDestinationOnNavMesh(fleeTarget, 3f);
            transform.rotation = Quaternion.identity;
            return;
        }

        // 2) jeśli za daleko -> podejdź trochę (żeby nie był “poza grą”)
        if (dist > maxDistance)
        {
            SetDestinationOnNavMesh(player.position, 3f);
            transform.rotation = Quaternion.identity;
            return;
        }

        // 3) idealny dystans:
        // - jeśli ma LOS: stoi i celuje/strzela
        // - jeśli nie ma LOS: reposition co jakiś czas
        if (hasLOS)
        {
            StopAgent();
            transform.rotation = Quaternion.identity;
            return;
        }

        if (repositionTimer <= 0f)
        {
            repositionTimer = repositionInterval;
            RepositionForLOS(dir);
        }

        transform.rotation = Quaternion.identity;
    }

    private void RepositionForLOS(Vector2 dirToPlayer)
    {
        // próbujemy kilka losowych punktów: bardziej w bok + lekko "od gracza"
        Vector2 perp = new Vector2(-dirToPlayer.y, dirToPlayer.x);

        for (int attempt = 0; attempt < 6; attempt++)
        {
            float side = Random.value > 0.5f ? 1f : -1f;

            // w bok + trochę od gracza, żeby utrzymać “sniper far”
            Vector2 moveDir = (perp * side + (-dirToPlayer) * 0.6f).normalized;

            Vector3 candidate = transform.position + (Vector3)(moveDir * repositionDistance * Random.Range(0.6f, 1.0f));

            if (SetDestinationOnNavMesh(candidate, 3f))
                return;
        }
    }

    private bool SetDestinationOnNavMesh(Vector3 target, float sampleRadius)
    {
        if (NavMesh.SamplePosition(target, out var hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            return true;
        }
        return false;
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