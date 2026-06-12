using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody2D))]
public class MinigunBossAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MinigunBossShooting shootingController;
    [SerializeField] private Rigidbody2D rb;
    private NavMeshAgent agent;
    private Transform player;

    [Header("Movement")]
    [SerializeField] private float sprayPhaseWalkSpeed = 2.5f;

    [Header("Stop Distance")]
    [SerializeField] private float stopDistance = 6f;
    [SerializeField] private float resumeDistance = 7f;

    [Header("Phase Durations")]
    [SerializeField] private float minPhaseDuration = 3.5f;
    [SerializeField] private float maxPhaseDuration = 5.5f;

    [Header("Transition")]
    [SerializeField] private float transitionDuration = 2f;

    public enum BossPhase { MinigunSpray, Grenades, ArcShotgun, Transition }
    public BossPhase CurrentPhase { get; private set; }

    private float phaseTimer;
    private bool isTransitioning;
    private bool isHoldingPosition;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.autoBraking = true;
        agent.stoppingDistance = 0f;

        if (!shootingController)
            shootingController = GetComponent<MinigunBossShooting>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        StartPhase(BossPhase.MinigunSpray);
        phaseTimer = Random.Range(minPhaseDuration, maxPhaseDuration);
    }

    private void Update()
    {
        if (!player) return;

        phaseTimer -= Time.deltaTime;

        if (phaseTimer <= 0 && !isTransitioning)
        {
            StartCoroutine(PhaseTransitionRoutine());
            return;
        }

        switch (CurrentPhase)
        {
            case BossPhase.MinigunSpray:
                MoveTowardPlayerSmooth();
                break;

            case BossPhase.Grenades:
            case BossPhase.ArcShotgun:
            case BossPhase.Transition:
                StandStill();
                break;
        }
    }

    // ---------------------------------------------------------
    //  SMOOTH MOVEMENT WITH STOP DISTANCE
    // ---------------------------------------------------------

    private void MoveTowardPlayerSmooth()
    {
        if (!agent.isOnNavMesh) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (isHoldingPosition)
        {
            if (dist <= resumeDistance)
            {
                StandStill();
                return;
            }

            isHoldingPosition = false;
        }

        if (dist <= stopDistance)
        {
            isHoldingPosition = true;
            StandStill();
            return;
        }

        agent.speed = sprayPhaseWalkSpeed;
        agent.SetDestination(player.position);

        rb.linearVelocity = agent.velocity;
    }

    private void StandStill()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
        rb.linearVelocity = Vector2.zero;
    }

    // ---------------------------------------------------------
    //  PHASE SYSTEM
    // ---------------------------------------------------------

    private IEnumerator PhaseTransitionRoutine()
    {
        isTransitioning = true;

        StartPhase(BossPhase.Transition);
        yield return new WaitForSeconds(transitionDuration);

        BossPhase next = GetWeightedRandomPhase();

        if (next == BossPhase.Grenades)
            phaseTimer = 0.1f;
        else
            phaseTimer = Random.Range(minPhaseDuration, maxPhaseDuration);

        StartPhase(next);

        isTransitioning = false;
    }

    private BossPhase GetWeightedRandomPhase()
    {
        int weightMinigun = 50;
        int weightGrenades = 25;
        int weightArc = 25;

        int total = weightMinigun + weightGrenades + weightArc;
        int roll = Random.Range(0, total);

        if (roll < weightMinigun)
            return BossPhase.MinigunSpray;

        roll -= weightMinigun;

        if (roll < weightGrenades)
            return BossPhase.Grenades;

        return BossPhase.ArcShotgun;
    }

    private void StartPhase(BossPhase newPhase)
    {
        CurrentPhase = newPhase;
        shootingController?.OnPhaseChanged(newPhase);
    }

    public void ForceEndPhase()
    {
        phaseTimer = 0f;
    }
}