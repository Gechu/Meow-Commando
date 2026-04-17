using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform visual;          // child ze spritem granatu
    [SerializeField] private GameObject shrapnelBulletPrefab;

    [Header("Flight timing")]
    [SerializeField] private float totalFlightTime = 1.4f;
    [SerializeField] private int visualBounces = 2;

    [Header("Arc / Bounce look")]
    [SerializeField] private float height = 0.6f;
    [Range(0.1f, 1f)]
    [SerializeField] private float heightDampingPerBounce = 0.6f;

    [Header("Explosion")]
    [SerializeField] private int bulletsCount = 12;
    [SerializeField] private float bulletsSpeed = 10f;

    [Header("Explode on touch")]
    [SerializeField] private bool explodeOnPlayerTouch = true;

    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionScale = 0.5f; // mniejszy niż kamikaze

    private float startTime;
    private bool exploded;

    private void Start()
    {
        if (!visual) visual = transform;
        startTime = Time.time;
    }

    private void Update()
    {
        if (exploded) return;

        float t = (Time.time - startTime) / totalFlightTime;
        if (t >= 1f)
        {
            Explode();
            return;
        }

        AnimateArcAndBounces(t);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (exploded) return;
        if (!explodeOnPlayerTouch) return;

        if (other.CompareTag("Player"))
            Explode();

        // Enemy: nic nie robimy (ma przelatywać)
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded) return;

        if (explodeOnPlayerTouch && collision.collider.CompareTag("Player"))
        {
            Explode();
            return;
        }
    }

    private void AnimateArcAndBounces(float t01)
    {
        int arcs = Mathf.Max(1, visualBounces + 1);

        float segmentLen = 1f / arcs;
        int segIndex = Mathf.Clamp(Mathf.FloorToInt(t01 / segmentLen), 0, arcs - 1);
        float segT = (t01 - segIndex * segmentLen) / segmentLen;

        float arc = Mathf.Sin(segT * Mathf.PI); // 0..1..0
        float damp = Mathf.Pow(heightDampingPerBounce, segIndex);

        float h = arc * height * damp;

        Vector3 vPos = visual.localPosition;
        vPos.y = h;
        visual.localPosition = vPos;
    }

    public void Explode()
    {
        if (exploded) return;
        exploded = true;

        if (explosionPrefab)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * explosionScale;
        }

        if (shrapnelBulletPrefab)
        {
            for (int i = 0; i < bulletsCount; i++)
            {
                float angle = (360f / bulletsCount) * i;
                Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

                GameObject bullet = Instantiate(shrapnelBulletPrefab, transform.position, Quaternion.identity);
                Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
                if (rb) rb.linearVelocity = dir * bulletsSpeed;
            }
        }

        Destroy(gameObject);
    }
}