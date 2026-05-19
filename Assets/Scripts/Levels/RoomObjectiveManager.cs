using UnityEngine;
using UnityEngine.UI;

public class RoomObjectiveManager : MonoBehaviour
{
    [Header("UI")]
    public Text checklistText;

    [Header("Door")]
    public Door door;

    [Header("Enemies")]
    public int totalEnemies = 1;

    private int enemiesKilled = 0;

    private bool completed = false;

    void Start()
    {
        UpdateChecklist();
    }

    public void EnemyKilled()
    {
        enemiesKilled++;

        UpdateChecklist();

        Debug.Log("Enemy killed: " + enemiesKilled);

        if (!completed && enemiesKilled >= totalEnemies)
        {
            completed = true;

            door.UnlockDoor();

            Debug.Log("Room completed!");
        }
    }

    void UpdateChecklist()
    {
        checklistText.text =
            (enemiesKilled >= totalEnemies ? "☑ " : "☐ ")
            + "Kill all ENEMIES ("
            + enemiesKilled + "/"
            + totalEnemies + ")";
    }
}