using UnityEngine;

public class LevelRunManager : MonoBehaviour
{
    public static LevelRunManager Instance;

    [Header("Run Stats")]
    public float runTime;
    public int damageTaken;

    [Header("Reward Settings")]
    public int baseReward = 50;
    public int targetTimeSeconds = 300;
    public int damagePenalty = 2;

    private bool timerRunning = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (timerRunning)
        {
            runTime += Time.unscaledDeltaTime;
        }
    }

    public void PauseTimer()
    {
        timerRunning = false;
    }

    public void ResumeTimer()
    {
        timerRunning = true;
    }

    public void AddDamageTaken(int amount)
    {
        damageTaken += amount;
    }

    public int CalculateReward()
    {
        int reward = baseReward;

        reward -= damageTaken * damagePenalty;

        int timeBonus =
            Mathf.Max(
                0,
                targetTimeSeconds -
                Mathf.RoundToInt(runTime));

        reward += timeBonus;

        return Mathf.Max(10, reward);
    }

    public void EndRun()
    {
        Instance = null;
        Destroy(gameObject);
    }
}