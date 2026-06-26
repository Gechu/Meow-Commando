using UnityEngine;

public class UziEnemyShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Bullet")]
    public float bulletSpeed = 10f;

    [Header("Range")]
    public float shootRange = 14f;

    [Header("Spread")]
    public float spreadAngle = 10f;

    [Header("Burst")]
    public float timeBetweenShots = 0.1f;
    public float burstCooldown = 1.4f;
    public int numofShots = 3;

    [Header("Line of Sight (2D)")]
    public LayerMask wallMask;
    public float losExtra = 0.1f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip burstSound;   // <-- dźwięk trzech strzałów

    Transform player;
    bool isShooting = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        if (!isShooting)
            StartCoroutine(ShootBurst());
    }

    System.Collections.IEnumerator ShootBurst()
    {
        isShooting = true;

        for (int i = 0; i < numofShots; i++)
        {
            // DŹWIĘK TYLKO PRZY PIERWSZYM STRZALE
            if (i == 0 && audioSource && burstSound)
                audioSource.PlayOneShot(burstSound);

            if (IsPlayerInRange() && HasLineOfSight2D(player.position))
                ShootOneBullet();

            yield return new WaitForSeconds(timeBetweenShots);
        }

        float jitter = Random.Range(burstCooldown * 0.8f, burstCooldown * 1.2f);
        yield return new WaitForSeconds(jitter);

        isShooting = false;
    }

    bool IsPlayerInRange()
    {
        Vector2 origin = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        float dist = Vector2.Distance(origin, player.position);
        return dist <= shootRange;
    }

    bool HasLineOfSight2D(Vector3 targetPos)
    {
        Vector2 origin = firePoint ? (Vector2)firePoint.position : (Vector2)transform.position;
        Vector2 toTarget = (Vector2)(targetPos - (Vector3)origin);
        float dist = toTarget.magnitude;

        if (dist <= 0.0001f) return true;

        Vector2 dir = toTarget / dist;

        RaycastHit2D hit = Physics2D.Raycast(origin, dir, dist - losExtra, wallMask);
        return hit.collider == null;
    }

    void ShootOneBullet()
    {
        if (!player || !firePoint || !bulletPrefab) return;

        Vector2 dir = (player.position - firePoint.position).normalized;

        float angleOffset = Random.Range(-spreadAngle, spreadAngle);
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;

        Vector2 finalDir = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb)
            rb.linearVelocity = finalDir * bulletSpeed;

        float rotAngle = Mathf.Atan2(finalDir.y, finalDir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, rotAngle);
    }
}
