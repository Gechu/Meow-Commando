using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDefaultData", menuName = "Game/Player Default Data")]
public class PlayerDefaultData : ScriptableObject
{
    [Header("Default Stats")]
    public int defaultMaxHP = 3;
    public int defaultCurrentHP = 3;
    public int defaultCatnipCount = 0;
}