using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 道路区間のまとまりを表すクラス
/// </summary>
public class Way : MonoBehaviour
{
    [Header("車線")]
    [SerializeField] private List<Lane> lanes = new();

    [Header("レーン配置")]
    [SerializeField] private float roadThickness = 7.0f;
    [SerializeField, Range(0.0f, 1.0f)] private float laneSpreadRatio = 1.0f;
    [SerializeField] private float laneGroupOffset = 0.0f;

    public IReadOnlyList<Lane> Lanes => lanes;
    public float RoadThickness => roadThickness;
    public float LaneSpreadRatio => laneSpreadRatio;
    public float LaneGroupOffset => laneGroupOffset;

    /// <summary>
    /// 指定したインデックスの車線を取得する
    /// </summary>
    public Lane GetLane(int _index)
    {
        for (int i = 0; i < lanes.Count; ++i)
        {
            if (lanes[i] != null && lanes[i].LaneIndex == _index)
            {
                return lanes[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 特に指定がない場合のデフォルトの車線を取得する
    /// </summary>
    public Lane GetDefaultLane()
    {
        if (lanes.Count == 0)
        {
            return null;
        }

        return lanes[0];
    }
}
