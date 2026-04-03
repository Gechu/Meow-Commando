using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MachineGunEnemyAI_Simple : MonoBehaviour
{
    [Header("Ideal distance")]
    [SerializeField] private float minDistance = 8f;
    [SerializeField] private float maxDistance = 11f;

    [Header("Repath (distance keeping)")]
    [SerializeField] private float repathInterval = 0.25f;

    [Header("Occasional reposition (gives skirmisher feel)")]
    [SerializeField] private float repositionEveryMin = 1.2f;
    [SerializeField] private float repositionEveryMax = 2.2f;
    [SerializeField] private float repositionDistance = 2.2f;

    private NavMeshAgent agent;
    private Transform player;

    private float repathTimer;
    private float nextRepositionTime;

    private NavMeshPath tmpPath;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = true;
        agent.stoppingDistance = 0f;

        tmpPath = new NavMeshPath();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ScheduleNextReposition();
    }

    private void Update()
    {
        if (!player) return;

        // 1) Trzymanie dystansu (NavMesh-first)
        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            KeepIdealDistance();
        }

        // 2) Co jakiś czas: spróbuj zrobić reposition w bok (jeden krok)
        if (Time.time >= nextRepositionTime)
        {
            TryRepositionSideways();
            ScheduleNextReposition();
        }

        transform.rotation = Quaternion.identity;
    }

    private void KeepIdealDistance()
    {
        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist <= 0.001f) return;

        Vector2 dir = toPlayer / dist;

        // za daleko -> podejdź do gracza
        if (dist > maxDistance)
        {
            SetDestSafe(player.position, 2f);
            return;
        }

        // za blisko -> idź "od gracza" (punkt retreat), ale tylko jeśli path complete
        if (dist < minDistance)
        {
            Vector3 retreatTarget = transform.position - (Vector3)(dir * (minDistance - dist + 1.5f));
            if (!SetDestSafe(retreatTarget, 2f))
            {
                // fallback: jak nie ma gdzie się cofnąć, to po prostu stój (lepsze niż wpychanie w ścianę)
                StopAgent();
            }
            return;
        }

        // w idealnym dystansie: nie ustawiaj co chwilę nowych celów
        StopAgent();
    }

    private void TryRepositionSideways()
    {
        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist <= 0.001f) return;

        // reposition rób głównie gdy jest w "ideal" range (żeby nie psuć cofania/podejścia)
        if (dist < minDistance || dist > maxDistance) return;

        Vector2 dir = toPlayer / dist;
        Vector2 perp = new Vector2(-dir.y, dir.x);

        // spróbuj lewo/prawo (losowo zaczynając)
        int first = (Random.value > 0.5f) ? 1 : -1;

        if (TrySetSide(perp * first)) return;
        TrySetSide(perp * -first);
    }

    private bool TrySetSide(Vector2 sideDir)
    {
        Vector3 target = transform.position + (Vector3)(sideDir.normalized * repositionDistance);

        // tylko jeśli na navmeshu i path complete
        return SetDestSafe(target, 2f);
    }

    private bool SetDestSafe(Vector3 target, float sampleRadius)
    {
        if (!NavMesh.SamplePosition(target, out var hit, sampleRadius, NavMesh.AllAreas))
            return false;

        if (!NavMesh.CalculatePath(transform.position, hit.position, NavMesh.AllAreas, tmpPath))
            return false;

        if (tmpPath.status != NavMeshPathStatus.PathComplete)
            return false;

        agent.SetDestination(hit.position);
        return true;
    }

    private void StopAgent()
    {
        if (agent.hasPath) agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    private void ScheduleNextReposition()
    {
        nextRepositionTime = Time.time + Random.Range(repositionEveryMin, repositionEveryMax);
    }
}