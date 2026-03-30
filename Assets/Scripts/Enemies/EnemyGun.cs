using UnityEngine;

public class EnemyGun : MonoBehaviour
{
    Transform player;
    Transform gun;

    Vector3 gunOriginalScale;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        gun = transform.GetChild(0);

        gunOriginalScale = gun.localScale;
    }

    void Update()
    {
        if (player == null) return;

        Vector2 dir = player.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);

        if (angle > 90 || angle < -90)
            gun.localScale = new Vector3(gunOriginalScale.x, -gunOriginalScale.y, 1);
        else
            gun.localScale = gunOriginalScale;
    }
}