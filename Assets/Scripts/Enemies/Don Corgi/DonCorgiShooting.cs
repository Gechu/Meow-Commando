using UnityEngine;
using System.Collections;

public enum CorgiShootMode
{
    None,
    Pistol,
    FinalMachineGun,
    FinalWave
}

public class DonCorgiShooting : MonoBehaviour
{
    [Header("References")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    [Header("Audio Sources")]
    public AudioSource pistolSource;
    public AudioSource machineGunSource;
    public AudioSource waveSource;

    [Header("Audio Clips")]
    public AudioClip pistolSound;
    public AudioClip machineGunSound;
    public AudioClip waveSound;

    [Header("Pistol")]
    public float pistolCooldown = 0.6f;
    public float pistolBulletSpeed = 10f;

    [Header("Machine Gun")]
    public float mgCooldown = 0.1f;
    public float mgBulletSpeed = 12f;

    [Header("Wave Attack")]
    public int waveBulletCount = 12;
    public float waveCooldown = 3f;
    public float waveBulletSpeed = 8f;

    private Transform player;
    private bool isShooting = false;

    public CorgiShootMode mode = CorgiShootMode.None;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player || isShooting) return;

        switch (mode)
        {
            case CorgiShootMode.Pistol:
                StartCoroutine(PistolShot());
                break;

            case CorgiShootMode.FinalMachineGun:
                StartCoroutine(MachineGunBurst());
                break;

            case CorgiShootMode.FinalWave:
                StartCoroutine(WaveAttack());
                break;
        }
    }

    // ---------------------------------------------------------
    //  PISTOL
    // ---------------------------------------------------------

    IEnumerator PistolShot()
    {
        isShooting = true;

        // 🔊 Dźwięk pojedynczego strzału
        if (pistolSource && pistolSound)
            pistolSource.PlayOneShot(pistolSound);

        ShootTowardsPlayer(pistolBulletSpeed);

        yield return new WaitForSeconds(pistolCooldown);
        isShooting = false;
    }

    // ---------------------------------------------------------
    //  MACHINE GUN (pojedyncze strzały)
    // ---------------------------------------------------------

    IEnumerator MachineGunBurst()
    {
        isShooting = true;

        // 🔊 Dźwięk pojedynczego strzału MG
        if (machineGunSource && machineGunSound)
            machineGunSource.PlayOneShot(machineGunSound);

        ShootTowardsPlayer(mgBulletSpeed);

        yield return new WaitForSeconds(mgCooldown);
        isShooting = false;
    }

    // ---------------------------------------------------------
    //  WAVE ATTACK (360°)
    // ---------------------------------------------------------

    IEnumerator WaveAttack()
    {
        isShooting = true;

        // 🔊 Dźwięk fali — raz na całą falę
        if (waveSource && waveSound)
            waveSource.PlayOneShot(waveSound);

        float angleStep = 360f / waveBulletCount;
        float angle = 0f;

        for (int i = 0; i < waveBulletCount; i++)
        {
            float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
            float bulletDirY = Mathf.Sin(angle * Mathf.Deg2Rad);

            Vector2 dir = new Vector2(bulletDirX, bulletDirY).normalized;

            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb)
                rb.linearVelocity = dir * waveBulletSpeed;

            float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot);

            angle += angleStep;
        }

        yield return new WaitForSeconds(waveCooldown);
        isShooting = false;
    }

    // ---------------------------------------------------------
    //  COMMON BULLET SPAWN
    // ---------------------------------------------------------

    void ShootTowardsPlayer(float speed)
    {
        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb)
            rb.linearVelocity = dir * speed;

        float rot = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0f, 0f, rot);
    }
}
