using UnityEngine;

public class EnemyDeathNotifier : MonoBehaviour
{
    public RoomObjectiveManager roomManager;

    public void NotifyDeath()
    {
        if (roomManager != null)
        {
            roomManager.EnemyKilled();
        }
    }
}