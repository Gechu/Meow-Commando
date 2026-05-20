using UnityEngine;

public class PlayerIdleRandomWind : MonoBehaviour
{
    [Header("References (optional)")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;

    [Header("Random wind timing (only while idle)")]
    [SerializeField] private float minDelay = 3f;
    [SerializeField] private float maxDelay = 8f;

    [Header("Idle detection")]
    [SerializeField] private float speedDeadZone = 0.05f;

    [Header("Animator parameters")]
    [SerializeField] private string isMovingParam = "IsMoving";
    [SerializeField] private string windTriggerParam = "PlayWind";

    private float timer;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!rb) rb = GetComponent<Rigidbody2D>();

        ResetTimer();
    }

    private void Update()
    {
        if (!animator || !rb) return;

        bool isMoving = rb.linearVelocity.sqrMagnitude > speedDeadZone * speedDeadZone;

        // Jeśli się rusza, nie odpalamy wiatru i resetujemy licznik
        if (isMoving)
        {
            ResetTimer();
            return;
        }

        // Idle: odliczaj do losowego odpalenia
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        // Dla pewności utrzymaj IsMoving=false (żeby Animator był w idle)
        animator.SetBool(isMovingParam, false);

        // Odpal animację wiatru
        animator.SetTrigger(windTriggerParam);

        // Wylosuj kolejny czas
        ResetTimer();
    }

    private void ResetTimer()
    {
        timer = Random.Range(minDelay, maxDelay);
    }
}