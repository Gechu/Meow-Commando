using UnityEngine;

public class PlayerAnimatorDriver : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float speedDeadZone = 0.05f;

    private void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        bool moving = rb && rb.linearVelocity.sqrMagnitude > speedDeadZone * speedDeadZone;
        if (animator) animator.SetBool("IsMoving", moving);
    }
}