using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAimToMouse : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [SerializeField] private Transform graphicsRoot; // flip X
    [SerializeField] private Transform armPivot;     // rotate (local)

    [Header("Smoothing")]
    [SerializeField] private float turnSpeedDegPerSec = 1440f;

    private Vector3 graphicsOriginalScale;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
        if (graphicsRoot) graphicsOriginalScale = graphicsRoot.localScale;
    }

    private void Update()
    {
        if (!cam) return;
        if (Mouse.current == null) return;
        if (!graphicsRoot || !armPivot) return;

        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, 0f));
        mouseWorld.z = armPivot.position.z;

        Vector2 dirWorld = (mouseWorld - armPivot.position);
        if (dirWorld.sqrMagnitude < 0.0001f) return;

        // 1) Flip całego "graphics" po stronie myszki
        bool faceLeft = dirWorld.x < 0f;

        Vector3 s = graphicsOriginalScale;
        s.x = faceLeft ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        graphicsRoot.localScale = s;

        // 2) Celowanie ręką: obrót local (ważne przy flipie parenta)
        // kąt do myszki w świecie
        float angleWorld = Mathf.Atan2(dirWorld.y, dirWorld.x) * Mathf.Rad2Deg;

        // po flipie X, local-space jest "odwrócony" -> kąt trzeba przemapować
        float targetLocalAngle = faceLeft ? (180f - angleWorld) : angleWorld;

        float currentLocalAngle = armPivot.localEulerAngles.z;
        float newLocalAngle = Mathf.MoveTowardsAngle(currentLocalAngle, targetLocalAngle, turnSpeedDegPerSec * Time.deltaTime);
        armPivot.localRotation = Quaternion.Euler(0, 0, newLocalAngle);
    }
}