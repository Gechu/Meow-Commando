using UnityEngine;

public interface IPlayerMovement
{
    float MoveSpeed { get; set; }
    bool IsDashing { get; }
    bool IsInvincible { get; }
}