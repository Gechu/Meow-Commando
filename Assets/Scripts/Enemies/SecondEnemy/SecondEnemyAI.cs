using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SecondEnemyAI : MonoBehaviour
{
    [Header("Distances")]
    [SerializeField] private float stopDistance = 6f;        // zatrzymaj się gdy <=
    [SerializeField] private float resumeDistance = 7f;      // rusz ponownie dopiero gdy >
    [SerializeField] private float repathInterval = 0.25f;

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    private NavMeshAgent agent;
    private Transform player;
    private float timer;

    private bool isHoldingPosition; // czy “stoi bo ma zasięg + LOS”

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = true;

        // stoppingDistance zostaw małe (żeby agent nie “hamował” za wcześnie po ścieżce),
        // bo i tak kontrolujemy stop logiką.
        agent.stoppingDistance = 0f;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (!player) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = repathInterval;

        float dist = Vector2.Distance(transform.position, player.position);
        bool hasLOS = HasLineOfSight2D(player.position);

        // Jeśli już trzymamy pozycję, to nie odpuszczaj przy minimalnych ruchach gracza
        if (isHoldingPosition)
        {
            // Trzymaj dopóki: nadal jest LOS i gracz nie odszedł poza resumeDistance
            if (hasLOS && dist <= resumeDistance)
            {
                StopAgentIfNeeded();
                return;
            }

            // inaczej wychodzimy z hold
            isHoldingPosition = false;
        }

        // Jeśli nie trzymamy pozycji, to przejdź w hold dopiero gdy spełnione warunki stop
        if (hasLOS && dist <= stopDistance)
        {
            isHoldingPosition = true;
            StopAgentIfNeeded();
            return;
        }

        // Chase
        if (NavMesh.SamplePosition(player.position, out var hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(player.position);

        transform.rotation = Quaternion.identity;
    }

    private void StopAgentIfNeeded()
    {
        if (agent.hasPath) agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    private bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = transform.position;
        Vector2 dir = (Vector2)(targetPos - transform.position);
        float dist = dir.magnitude;

        if (dist <= 0.0001f) return true;

        dir /= dist;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }
}