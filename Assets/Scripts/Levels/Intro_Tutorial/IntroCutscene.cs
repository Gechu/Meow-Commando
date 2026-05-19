using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class IntroCutscene : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    private float holdTime = 0f;
    private float requiredHoldTime = 1.5f;
    public UnityEngine.UI.Image skipBar;

    void Start()
    {
        videoPlayer.loopPointReached += EndCutscene;
    }

    void Update()
    {
        if (Keyboard.current.fKey.isPressed)
        {
            holdTime += Time.deltaTime;
            skipBar.fillAmount = holdTime / requiredHoldTime;

            if (holdTime >= requiredHoldTime)
            {
                LoadTutorial();
            }
        }
        else
        {
            holdTime = 0f;
            skipBar.fillAmount = 0f;
        }
    }

    void EndCutscene(VideoPlayer vp)
    {
        LoadTutorial();
    }

    void LoadTutorial()
    {
        SceneManager.LoadScene("Tutorial_Movement");
    }
}