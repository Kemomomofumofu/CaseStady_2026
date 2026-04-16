using UnityEngine;

/// <summary>
/// プレイヤーの現在のLaneとs位置を管理するクラス
/// </summary>
[System.Serializable]
public sealed class PlayerPathState
{
    [SerializeField] private Lane currentLane;
    [SerializeField] private float currentS = 0.0f;

    public Lane CurrentLane
    {
        get => currentLane;
        set => currentLane = value;
    }

    public float CurrentS
    {
        get => currentS;
        set => currentS = value;
    }

    /// <summary>
    /// リセット
    /// </summary>
    /// <param name="_lane">戻す車線</param>
    /// <param name="_s">戻す位置</param>
    public void Reset(Lane _lane, float _s = 0.0f)
    {
        currentLane = _lane;
        currentS = _s;
    }
}