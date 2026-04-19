using System.Collections.Generic;
using UnityEngine;

public class PlayerStatSystem : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseMoveSpeed = 7.5f;

    private float moveSpeedMultiplier = 1f;

    private List<StatModifier> modifiers = new List<StatModifier>();

    private void Update()
    {
        float delta = Time.deltaTime;

        for (int i = modifiers.Count - 1; i >= 0; i--)
        {
            StatModifier m = modifiers[i];

            if (m.duration > 0f)
            {
                m.duration -= delta;
                modifiers[i] = m;

                if (m.duration <= 0f)
                {
                    modifiers.RemoveAt(i);
                }
            }
        }

        RecalculateStats();
    }

    public void AddModifier(StatModifier mod)
    {
        modifiers.Add(mod);
        RecalculateStats();
    }

    private void RecalculateStats()
    {
        moveSpeedMultiplier = 1f;

        foreach (var m in modifiers)
        {
            moveSpeedMultiplier *= m.moveSpeedMult;
        }
    }

    public float GetMoveSpeed()
    {
        return baseMoveSpeed * moveSpeedMultiplier;
    }
}