using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(ShotgunShooting))]
public class ShotgunEnemyAI : MonoBehaviour
{
    [Header("Distances (close range)")]
    [SerializeField] private float stopDistance = 5f;
    [SerializeField] private float resumeDistance = 6f;
    [SerializeField] private float repathInterval = 0.2f;

    [Header("Post-shot sidestep")]
    [SerializeField] private float sidestepDistance = 1.5f;
    [SerializeField] private float sidestepDuration = 0.35f;

    [Header("Line of Sight (2D)")]
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float losExtra = 0.05f;

    private NavMeshAgent agent;
    private ShotgunShooting shooting;
    private Transform player;

    private float repathTimer;
    private bool isHoldingPosition;

    // sidestep state
    private bool isSidestepping;
    private float sidestepTimer;
    private Vector3 sidestepTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        shooting = GetComponent<ShotgunShooting>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = true;
        agent.stoppingDistance = 0f;

        // event po strzale -> rozpocznij sidestep
        shooting.OnShotFired += HandleShotFired;
    }

    private void OnDestroy()
    {
        if (shooting) shooting.OnShotFired -= HandleShotFired;
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (!player) return;

        // 1) sidestep ma priorytet (po strzale)
        if (isSidestepping)
        {
            sidestepTimer -= Time.deltaTime;
            if (sidestepTimer <= 0f)
            {
                isSidestepping = false;
            }
            else
            {
                agent.SetDestination(sidestepTarget);
                transform.rotation = Quaternion.identity;
                return;
            }
        }

        // 2) jeśli aimuje -> stoi, ale może aktualizować locked aim, gdy ma LOS
        if (shooting.IsAiming)
        {
            if (HasLineOfSight2D(player.position))
                shooting.UpdateLockedAim(player.position);

            StopAgentIfNeeded();
            transform.rotation = Quaternion.identity;
            return;
        }

        // 3) repath tick
        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f) return;
        repathTimer = repathInterval;

        float dist = Vector2.Distance(transform.position, player.position);
        bool hasLOS = HasLineOfSight2D(player.position);

        // 4) start ładowania tylko gdy LOS + range + cooldown ready
        if (hasLOS && shooting.CanStartAiming(player.position))
        {
            shooting.StartAiming(player.position);
            StopAgentIfNeeded();
            return;
        }

        // 5) hold w krótkim dystansie tylko jeśli LOS
        if (isHoldingPosition)
        {
            if (hasLOS && dist <= resumeDistance)
            {
                StopAgentIfNeeded();
                return;
            }
            isHoldingPosition = false;
        }

        if (hasLOS && dist <= stopDistance)
        {
            isHoldingPosition = true;
            StopAgentIfNeeded();
            return;
        }

        // 6) chase
        if (NavMesh.SamplePosition(player.position, out var hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(player.position);

        transform.rotation = Quaternion.identity;
    }

    private void HandleShotFired()
    {
        // losowo lewo/prawo względem kierunku do gracza
        Vector2 toPlayer = (player.position - transform.position);
        Vector2 dir = (toPlayer.sqrMagnitude < 0.0001f) ? Vector2.right : toPlayer.normalized;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        float side = (Random.value > 0.5f) ? 1f : -1f;
        Vector3 desired = transform.position + (Vector3)(perp * side * sidestepDistance);

        if (NavMesh.SamplePosition(desired, out var hit, 2f, NavMesh.AllAreas))
            sidestepTarget = hit.position;
        else
            sidestepTarget = transform.position;

        isSidestepping = true;
        sidestepTimer = sidestepDuration;
    }

    private void StopAgentIfNeeded()
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