using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MachineGunEnemyAI : MonoBehaviour
{
    [Header("Distance")]
    public float minDistance = 8f;
    public float maxDistance = 11f;

    [Header("Strafe")]
    public float strafeDistance = 1.4f;     // mniejsze niż w Uzi
    public float strafeChangeMin = 3.5f;    // dłużej trzyma jedną stronę
    public float strafeChangeMax = 6.0f;

    [Header("Movement steps")]
    [Tooltip("Jak daleko próbować odskoczyć/cofnąć się przy zbyt małym dystansie.")]
    public float retreatStep = 1.6f;

    [Tooltip("Jak daleko w bok/przód wybieramy punkt co repath.")]
    public float repathInterval = 0.2f;

    [Header("NavMesh sampling")]
    [Tooltip("Promień SamplePosition. Raczej mały przy cienkich ścianach.")]
    public float sampleRadius = 2.0f; // jeśli dalej głupieje przy ścianach, zmniejsz do 1.0–1.5

    private NavMeshAgent agent;
    private Transform player;

    private float strafeDirection = 1f;
    private float strafeTimer = 0f;
    private float repathTimer = 0f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        ResetStrafe();
    }

    private void Update()
    {
        if (!player) return;

        // zmiana kierunku strafe co jakiś czas (rzadziej niż Uzi)
        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
            ResetStrafe();

        // repath rzadziej
        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f) return;
        repathTimer = repathInterval;

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        Vector2 dir = (dist > 0.001f) ? (toPlayer / dist) : Vector2.right;

        Vector3 desiredPoint;

        if (dist < minDistance)
        {
            // wycofaj się (krótszy krok, żeby mniej wciskał w ściany)
            desiredPoint = transform.position - (Vector3)(dir * retreatStep);
        }
        else if (dist > maxDistance)
        {
            // podejdź
            desiredPoint = player.position;
        }
        else
        {
            // strafe w bok w idealnym dystansie (mniejszy niż Uzi)
            Vector2 perp = new Vector2(-dir.y, dir.x) * strafeDirection;
            desiredPoint = transform.position + (Vector3)(perp * strafeDistance);
        }

        // przyklej do navmesha
        if (NavMesh.SamplePosition(desiredPoint, out var hit, sampleRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
        else
        {
            // awaryjnie: zostań, zamiast szarpać
            agent.SetDestination(transform.position);
        }

        transform.rotation = Quaternion.identity;
    }

    private void ResetStrafe()
    {
        strafeDirection = (Random.value > 0.5f) ? 1f : -1f;
        strafeTimer = Random.Range(strafeChangeMin, strafeChangeMax);
    }
}