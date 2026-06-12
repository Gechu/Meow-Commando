using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class DonCorgiBossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform thronePosition;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private NavMeshAgent agent;

    [Header("Settings")]
    [SerializeField] private float activePhaseDuration = 10f;
    [SerializeField] private int enemiesPerWave = 4;

    private bool isOnThrone = true;

    private void Start()
    {
        StartCoroutine(BossCycle());
    }

    private IEnumerator BossCycle()
    {
        while (bossHealth != null && bossHealth.CurrentHealth > 0)
        {
            // Faza tronowa
            yield return StartCoroutine(ThronePhase());

            // Faza aktywna
            yield return StartCoroutine(ActivePhase());
        }
    }

    private IEnumerator ThronePhase()
    {
        isOnThrone = true;
        transform.position = thronePosition.position;

        SpawnEnemies();

        // Czekaj aż wszyscy przeciwnicy zginą
        while (Object.FindObjectsByType<EnemyHP>(FindObjectsSortMode.None).Length > 0)
            yield return null;
    }

    private IEnumerator ActivePhase()
    {
        isOnThrone = false;

        Transform randomPoint = waypoints[Random.Range(0, waypoints.Length)];
        agent.SetDestination(randomPoint.position);

        yield return new WaitForSeconds(activePhaseDuration);

        // powrót na tron
        agent.SetDestination(thronePosition.position);
        yield return new WaitUntil(() => Vector2.Distance(transform.position, thronePosition.position) < 0.5f);
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemiesPerWave; i++)
        {
            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Vector2 spawnPos = (Vector2)transform.position + Random.insideUnitCircle * 5f;
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
