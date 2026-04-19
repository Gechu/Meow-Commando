using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementV2 : MonoBehaviour, IPlayerMovement
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastDirection = Vector2.right;
    private PlayerStatSystem stats;

    [Header("Dash")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashTime = 0.3f;
    private bool isDashing = false;
    private float dashTimer;
    public bool IsDashing => isDashing;

    [Header("Invincibility")]
    [SerializeField] private float invincibilityTime = 0.35f;
    private float invincibilityTimer = 0f;
    public bool IsInvincible => invincibilityTimer > 0f;

    private void Awake()
    {
        stats = GetComponent<PlayerStatSystem>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void Update()
    {
        moveInput = Vector2.zero;

        if (Keyboard.current == null) return;

        if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
        if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
        if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
        if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

        moveInput = moveInput.normalized;

        // Zapamiętaj ostatni kierunek (do dasha)
        if (moveInput != Vector2.zero)
        {
            lastDirection = moveInput;
        }

        // DASH (spacja)
        if (Keyboard.current.spaceKey.wasPressedThisFrame && !isDashing)
        {
            StartDash();
        }

        // Invincibility timer
        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = lastDirection * dashForce;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0f)
            {
                isDashing = false;
            }
        }
        else
        {
            rb.linearVelocity = moveInput * stats.GetMoveSpeed();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        invincibilityTimer = invincibilityTime;

        // Jeśli brak inputu — dash w prawo
        if (lastDirection == Vector2.zero)
        {
            lastDirection = Vector2.right;
        }
    }

    // Przydatne do animatora
    public Vector2 GetMoveInput() => moveInput;
}