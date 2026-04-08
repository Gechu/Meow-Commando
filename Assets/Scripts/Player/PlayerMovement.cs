using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 7.5f;
    private Rigidbody2D rb;
    private Vector2 input;
    private Vector2 lastDirection = Vector2.right;

    // Dash-related
    private bool isDashing = false;
    private float dashTimer;
    public bool IsDashing => isDashing;

    [Header("Invincibility")]
    public float invincibilityTime = 0.35f;
    private float invincibilityTimer = 0f;
    public bool IsInvincible => invincibilityTimer > 0f;

    [Header("Dash")]
    public float dashForce = 15f;
    public float dashTime = 0.3f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input = new Vector2(
            UnityEngine.InputSystem.Keyboard.current.aKey.isPressed ? -1 :
            UnityEngine.InputSystem.Keyboard.current.dKey.isPressed ? 1 : 0,

            UnityEngine.InputSystem.Keyboard.current.sKey.isPressed ? -1 :
            UnityEngine.InputSystem.Keyboard.current.wKey.isPressed ? 1 : 0
        );

        // If user moves remember it
        if (input != Vector2.zero)
        {
            lastDirection = input.normalized;
        }

        // Dashing
        if (UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame && !isDashing)
        {
            StartDash();
        }

        if (invincibilityTimer > 0f)
        {
            invincibilityTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (isDashing)
        {
            rb.linearVelocity = lastDirection * dashForce;

            dashTimer -= Time.fixedDeltaTime;
            if (dashTimer <= 0)
            {
                isDashing = false;
            }
        }
        else
        {
            rb.linearVelocity = input.normalized * speed;
        }
    }

    void StartDash()
    {
        isDashing = true;
        dashTimer = dashTime;
        invincibilityTimer = invincibilityTime;

        // If user wasn't moving anywhere dash right
        if (lastDirection == Vector2.zero)
        {
            lastDirection = Vector2.right;
        }
    }
}