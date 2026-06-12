using UnityEngine;

public class BossRoomManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BossHealth bossHealth;
    [SerializeField] private Door[] doorsToUnlock;

    private void Start()
    {
        if (bossHealth != null)
        {
            bossHealth.onBossDied.AddListener(OnBossDied);
        }
    }

    private void OnBossDied()
    {
        Debug.Log("Boss dead → unlocking doors!");

        foreach (Door d in doorsToUnlock)
        {
            if (d != null)
                d.UnlockDoor();
        }
    }
}