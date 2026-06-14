using UnityEngine;
using UnityEngine.UI;

public class LevelSummaryUI : MonoBehaviour
{
    public Text timeText;
    public Text damageText;
    public Text rewardText;
    public Text targetTimeText;

    void Start()
    {
        if (LevelRunManager.Instance == null)
            return;

        float time = LevelRunManager.Instance.runTime;

        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);

        timeText.text = $"{minutes:00}:{seconds:00}";

        damageText.text = LevelRunManager.Instance.damageTaken.ToString();

        int reward = LevelRunManager.Instance.CalculateReward();

        rewardText.text = reward.ToString();

        int targetMinutes = Mathf.FloorToInt(LevelRunManager.Instance.targetTimeSeconds / 60);

        int targetSeconds = LevelRunManager.Instance.targetTimeSeconds % 60;

        targetTimeText.text = $"{targetMinutes:00}:{targetSeconds:00}";
    }

    public void Continue()
    {
        int reward = LevelRunManager.Instance.CalculateReward();

        PlayerDataManager.Instance.coins += reward;

        Destroy(LevelRunManager.Instance.gameObject);

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainHub");
    }
}