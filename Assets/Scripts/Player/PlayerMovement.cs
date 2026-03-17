using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 2f;
    private Rigidbody2D rb;
    private Vector2 input;

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

    }

    void FixedUpdate()
    {
        rb.linearVelocity = input.normalized * speed;
    }
}