using System;
using UnityEngine;

/// <summary>
/// Œğ·“_‚Å‚ÌLaneÚ‘±
/// </summary>
[Serializable]
public sealed class LaneLink
{
    [SerializeField] private TurnDirection turnDirection = TurnDirection.Straight;
    [SerializeField] private Lane nextLane;

    public TurnDirection TurnDirection => turnDirection;
    public Lane NextLane => nextLane;
}
