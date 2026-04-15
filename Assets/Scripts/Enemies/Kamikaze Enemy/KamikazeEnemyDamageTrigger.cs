using UnityEngine;

public class KamikazeDogDamageTrigger : MonoBehaviour
{
    [SerializeField] private KamikazeDogAI ai;
    [SerializeField] private string playerTag = "Player";

    private void Awake()
    {
        if (!ai) ai = GetComponentInParent<KamikazeDogAI>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        ai?.StartExplodeWindup();
    }
}