using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip music;

    private void Start()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMusic(music);
    }
}