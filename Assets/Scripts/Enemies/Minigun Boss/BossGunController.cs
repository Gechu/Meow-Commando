using UnityEngine;

public class BossGunController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gun;        // sprite ręki z minigunem
    [SerializeField] private Transform firePoint;  // koniec lufy

    private Transform player;
    private Vector3 gunOriginalScale;

    [Header("Rotation")]
    [SerializeField] private float turnSpeedDegPerSec = 720f;
    [SerializeField] private float maxVerticalAngle = 45f;

    private float baseAngle;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (!gun)
            gun = transform.GetChild(0);

        gunOriginalScale = gun.localScale;

        baseAngle = transform.localEulerAngles.z;
    }

    private void Update()
    {
        if (!player) return;

        // kierunek do gracza
        Vector2 dir = player.position - transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // uwzględnij flip ciała (root scale.x)
        bool facingLeft = transform.root.localScale.x < 0f;
        if (facingLeft)
            targetAngle = 180f - targetAngle;

        // ograniczenie pionowe ±45°
        float delta = Mathf.DeltaAngle(baseAngle, targetAngle);
        float clamped = Mathf.Clamp(delta, -maxVerticalAngle, maxVerticalAngle);
        float finalAngle = baseAngle + clamped;

        // smooth obrót pivotu
        float currentAngle = transform.localEulerAngles.z;
        float newAngle = Mathf.MoveTowardsAngle(currentAngle, finalAngle, turnSpeedDegPerSec * Time.deltaTime);

        transform.localRotation = Quaternion.Euler(0, 0, newAngle);

        // flip sprite’a ręki
        if (newAngle > 90 || newAngle < -90)
            gun.localScale = new Vector3(gunOriginalScale.x, -gunOriginalScale.y, 1);
        else
            gun.localScale = gunOriginalScale;
    }

    public Transform GetFirePoint() => firePoint;
}