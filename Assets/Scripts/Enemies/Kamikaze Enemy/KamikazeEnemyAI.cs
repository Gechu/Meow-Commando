using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class KamikazeDogAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private string playerTag = "Player";

    [Header("Chase")]
    [SerializeField] private float repathInterval = 0.2f;

    [Header("Explode")]
    [SerializeField] private float windupTime = 0.5f;

    private NavMeshAgent agent;
    private Transform player;
    private EnemyHP hp;

    private float repathTimer;
    private bool exploding;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        hp = GetComponent<EnemyHP>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        var pObj = GameObject.FindGameObjectWithTag(playerTag);
        player = pObj ? pObj.transform : null;
    }

    private void Update()
    {
        if (!player || exploding) return;

        repathTimer -= Time.deltaTime;
        if (repathTimer <= 0f)
        {
            repathTimer = repathInterval;
            agent.SetDestination(player.position);
        }
    }

    public void StartExplodeWindup()
    {
        if (exploding) return;
        StartCoroutine(ExplodeRoutine());
    }

    private IEnumerator ExplodeRoutine()
    {
        exploding = true;

        agent.ResetPath();
        agent.isStopped = true;

        // tu później animacja/efekt “charge”
        yield return new WaitForSeconds(windupTime);

        // zabij psa -> EnemyHP odpali wybuch w Die()
        if (hp != null) hp.Kill();
        else Destroy(gameObject);
    }
}