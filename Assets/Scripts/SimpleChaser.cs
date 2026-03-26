using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class SimpleChaser : MonoBehaviour
{
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    void Update()
    {
        if (!player) return;

        // “Przyklej” cel do NavMesha (żeby zawsze dało się policzyć ścieżkę)
        if (NavMesh.SamplePosition(player.position, out var hit, 2f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);
        else
            agent.SetDestination(player.position);

        transform.rotation = Quaternion.identity;
    }
}