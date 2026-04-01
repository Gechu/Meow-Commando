using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class FirstEnemyNavMeshAI : MonoBehaviour
{
    [Header("Dystans")]
    public float minDistance = 6f;
    public float maxDistance = 8f;

    [Header("Strafe")]
    public float strafeDistance = 2f;          // jak daleko w bok wybiera punkt
    public float strafeChangeMin = 2f;
    public float strafeChangeMax = 4f;

    [Header("Reaction Time")]
    public float repathInterval = 0.2f;

    private NavMeshAgent agent;
    private Transform player;

    private float strafeDirection = 1f;
    private float strafeTimer = 0f;

    private float repathTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        ResetStrafe();
    }

    void Update()
    {
        if (!player) return;

        // zmiana kierunku strafe co jakiś czas
        strafeTimer -= Time.deltaTime;
        if (strafeTimer <= 0f)
            ResetStrafe();

        // repath rzadziej (płynniej i taniej)
        repathTimer -= Time.deltaTime;
        if (repathTimer > 0f) return;
        repathTimer = repathInterval;

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        Vector2 dir = (dist > 0.001f) ? (toPlayer / dist) : Vector2.right;

        Vector3 desiredPoint;

        if (dist < minDistance)
        {
            // wycofaj się
            desiredPoint = transform.position - (Vector3)(dir * 2f);
        }
        else if (dist > maxDistance)
        {
            // podejdź
            desiredPoint = player.position;
        }
        else
        {
            // strafe w bok w "idealnym dystansie"
            Vector2 perp = new Vector2(-dir.y, dir.x) * strafeDirection;
            desiredPoint = transform.position + (Vector3)(perp * strafeDistance);
        }

        // “przyklej” punkt do navmesha
        if (NavMesh.SamplePosition(desiredPoint, out var hit, 3f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(transform.position); // awaryjnie: zostań

        transform.rotation = Quaternion.identity;
    }

    void ResetStrafe()
    {
        strafeDirection = (Random.value > 0.5f) ? 1f : -1f;
        strafeTimer = Random.Range(strafeChangeMin, strafeChangeMax);
    }
}