using UnityEngine;

public class BossBodyFlip : MonoBehaviour
{
    private Transform player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void LateUpdate()
    {
        if (!player) return;

        bool facingLeft = player.position.x < transform.position.x;

        Vector3 s = transform.localScale;
        s.x = facingLeft ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
        transform.localScale = s;
    }
}
