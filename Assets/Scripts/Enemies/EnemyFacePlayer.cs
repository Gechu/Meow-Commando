// name=Assets/Scripts/Enemies/EnemyFacePlayerFlipX.cs
using UnityEngine;

public class EnemyFacePlayerFlipX : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private SpriteRenderer sr;

    private Transform player;

    private void Awake()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag(playerTag)?.transform;
    }

    private void LateUpdate()
    {
        if (!player || !sr) return;

        // gracz po lewej => flip
        sr.flipX = player.position.x < transform.position.x;
    }
}