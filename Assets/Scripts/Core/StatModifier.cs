using UnityEngine;
using System;

[Serializable]
public struct StatModifier
{
    public float moveSpeedMult;
    public float fireRateMult;
    public float bulletSpeedMult;

    public float duration;
}