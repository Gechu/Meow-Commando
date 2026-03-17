using UnityEngine;
using System.Collections.Generic;

public class FirstEnemyAI : MonoBehaviour
{
    [Header("Dystans")]
    public float minDistance = 6f;
    public float maxDistance = 8f;
    public float moveSpeed = 2f;

    [Header("Strafe")]
    public float baseStrafeSpeed = 1.5f;
    public float strafeChangeMin = 2f;
    public float strafeChangeMax = 4f;

    [Header("Reaction Time")]
    public float reactionInterval = 0.1f; // mikro-opóźnienie

    [Header("Separation")]
    public float separationRadius = 2f;
    public float separationForce = 2f;

    [Header("Wall Avoidance")]
    public float wallAvoidRadius = 2f;
    public float wallAvoidForce = 3f;

    Transform player;

    float strafeDirection = 1f;
    float strafeTimer = 0f;
    float currentStrafeSpeed = 1f;

    float reactionTimer = 0f;
    Vector2 cachedMoveDir = Vector2.zero;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        ResetStrafe();
    }

    void Update()
    {
        reactionTimer -= Time.deltaTime;

        if (reactionTimer <= 0f)
        {
            cachedMoveDir = CalculateMovement();
            reactionTimer = reactionInterval;
        }

        transform.position += (Vector3)(cachedMoveDir * moveSpeed * Time.deltaTime);
    }

    Vector2 CalculateMovement()
    {
        if (player == null) return Vector2.zero;

        Vector2 toPlayer = player.position - transform.position;
        float distance = toPlayer.magnitude;
        Vector2 dir = toPlayer.normalized;

        Vector2 move = Vector2.zero;

        // 1. Separation od innych przeciwników i ścian
        move += CalculateSeparation();

        // 2. Priorytet: ustawienie dystansu
        if (distance < minDistance)
        {
            move -= dir; // cofaj się
        }
        else if (distance > maxDistance)
        {
            move += dir; // podchodź
        }
        else
        {
            // 3. Strafe tylko w idealnej odległości
            strafeTimer -= Time.deltaTime;
            if (strafeTimer <= 0f)
                ResetStrafe();

            Vector2 strafe = new Vector2(-dir.y, dir.x) * strafeDirection;

            // strafe nigdy nie może pchać w stronę gracza
            if (Vector2.Dot(strafe, dir) > 0)
                strafe = Vector2.zero;

            move += strafe * currentStrafeSpeed;
        }

        return move.normalized;
    }

    Vector2 CalculateSeparation()
    {
        Vector2 force = Vector2.zero;

        // 1. Separation od innych przeciwników
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject e in enemies)
        {
            if (e == this.gameObject) continue;

            float dist = Vector2.Distance(transform.position, e.transform.position);

            if (dist < separationRadius)
            {
                Vector2 away = (Vector2)(transform.position - e.transform.position);
                force += away.normalized * (separationForce / dist);
            }
        }

        // 2. Separation od ścian
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");

        foreach (GameObject w in walls)
        {
            float dist = Vector2.Distance(transform.position, w.transform.position);

            if (dist < wallAvoidRadius)
            {
                Vector2 away = (Vector2)(transform.position - w.transform.position);
                force += away.normalized * (wallAvoidForce / dist);
            }
        }

        return force;
    }

    void ResetStrafe()
    {
        strafeDirection = Random.value > 0.5f ? 1f : -1f;
        currentStrafeSpeed = baseStrafeSpeed * Random.Range(0.8f, 1.2f);
        strafeTimer = Random.Range(strafeChangeMin, strafeChangeMax);
    }
}