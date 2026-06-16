using UnityEngine;
using System.Collections;

public class DonCorgiBossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform thronePosition;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private DonCorgiShooting shooting;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float finalPhaseSpeedMultiplier = 1.5f;
    [SerializeField] private float throneDialogueTime = 5f;

    private int currentRound = 1;
    private const int maxRounds = 4;

    private void Start()
    {
        StartCoroutine(BossCycle());
    }

    private IEnumerator BossCycle()
    {
        // --- FAZA 0: Intro ---
        transform.position = thronePosition.position;
        shooting.mode = CorgiShootMode.None;

        // TODO: UI dialog — "Moi ludzie się tobą zajmą!"
        yield return new WaitForSeconds(throneDialogueTime);

        // --- Rundy 1–4 ---
        while (currentRound <= maxRounds && bossHealth.CurrentHealth > 0)
        {
            yield return StartCoroutine(SpawnPhase());
            yield return StartCoroutine(ActivePhase());
            currentRound++;
        }

        // --- Finalna faza ---
        if (bossHealth.CurrentHealth > 0)
        {
            yield return StartCoroutine(FinalPhase());
        }
    }

    private IEnumerator SpawnPhase()
    {
        transform.position = thronePosition.position;
        shooting.mode = CorgiShootMode.None;

        int enemiesToSpawn = 5 + (currentRound - 1); // 5,6,7,8

        SpawnEnemies(enemiesToSpawn);

        // Czekamy aż wszyscy przeciwnicy zginą
        while (Object.FindObjectsByType<EnemyHP>(FindObjectsSortMode.None).Length > 0)
            yield return null;
    }

    private IEnumerator ActivePhase()
    {
        shooting.mode = CorgiShootMode.Pistol;

        float activeTime = 9f - currentRound; // 8,7,6,5

        // Wybieramy jeden losowy waypoint
        Transform target = waypoints[Random.Range(0, waypoints.Length)];

        float timer = 0f;
        while (timer < activeTime)
        {
            MoveTowards(target.position);
            timer += Time.deltaTime;
            yield return null;
        }

        // Powrót na tron
        while (Vector2.Distance(transform.position, thronePosition.position) > 0.1f)
        {
            MoveTowards(thronePosition.position);
            yield return null;
        }

        shooting.mode = CorgiShootMode.None;
    }

    private IEnumerator FinalPhase()
    {
        transform.position = thronePosition.position;
        shooting.mode = CorgiShootMode.None;

        // TODO: UI dialog — "Dobra, zrobię to sam!"
        yield return new WaitForSeconds(throneDialogueTime);

        shooting.mode = CorgiShootMode.FinalMachineGun;

        float finalSpeed = moveSpeed * finalPhaseSpeedMultiplier;

        while (bossHealth.CurrentHealth > 0)
        {
            Transform target = waypoints[Random.Range(0, waypoints.Length)];

            // Ruch do waypointu
            while (Vector2.Distance(transform.position, target.position) > 0.1f)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    target.position,
                    finalSpeed * Time.deltaTime
                );

                transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
                yield return null;
            }

            // Zatrzymanie na 2 sekundy
            yield return new WaitForSeconds(2f);

            // Spawn 3 przeciwników
            SpawnEnemiesAtPoint(target.position, 3);

            // Losowy wybór trybu strzelania
            if (Random.value < 0.4f)
                shooting.mode = CorgiShootMode.FinalWave;
            else
                shooting.mode = CorgiShootMode.FinalMachineGun;
        }
    }

    private void MoveTowards(Vector2 target)
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            target,
            moveSpeed * Time.deltaTime
        );

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
    }

    private void SpawnEnemies(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Transform wp = waypoints[i % waypoints.Length];
            SpawnEnemiesAtPoint(wp.position, 1);
        }
    }

    private void SpawnEnemiesAtPoint(Vector2 pos, int count)
    {
        float radius = 2f;

        for (int i = 0; i < count; i++)
        {
            Vector2 offset = Random.insideUnitCircle * radius;
            Vector2 spawnPos = pos + offset;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
}
