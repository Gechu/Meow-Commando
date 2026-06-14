using UnityEngine;

public class LevelStarter : MonoBehaviour
{
    public GameObject levelRunManagerPrefab;

    [Header("Level Settings")]
    public int baseReward = 50;
    public int targetTimeSeconds = 300;

    private void Start()
    {
        if (LevelRunManager.Instance == null)
        {
            GameObject manager = Instantiate(levelRunManagerPrefab);

            LevelRunManager runManager = manager.GetComponent<LevelRunManager>();

            runManager.baseReward = baseReward;
            runManager.targetTimeSeconds = targetTimeSeconds;
        }
    }
}