using System.Collections.Generic;
using UnityEngine;

public class Intersection : MonoBehaviour
{
    [Header("道路設定")]
    [Tooltip("東の道路")]
    [SerializeField] private Way EastWay;
    [Tooltip("西の道路")]
    [SerializeField] private Way WestWay;
    [Tooltip("南の道路")]
    [SerializeField] private Way SouthWay;
    [Tooltip("北の道路")]
    [SerializeField] private Way NorthWay;

    [Header("流入道路（自動設定）")]
    [SerializeField] private List<Way> incomingWays = new();
    public IReadOnlyList<Way> IncomingWays => incomingWays;

    public void RegisterIncomingWay(Way _way)
    {
        if (_way == null)
        {
            return;
        }

        for (int i = 0; i < incomingWays.Count; ++i)
        {
            if (incomingWays[i] == _way)
            {
                return;
            }
        }

        incomingWays.Add(_way);
    }

    public void CleanupIncomingWays()
    {
        for (int i = incomingWays.Count - 1; i >= 0; --i)
        {
            if (incomingWays[i] == null)
            {
                incomingWays.RemoveAt(i);
            }
        }
    }

    /// <summary>
    /// 指定した方向に対応するWayスロットへWayを設定する
    /// </summary>
    /// <param name="_direction">この交差点から見た方向ベクトル</param>
    /// <param name="_way">設定するWay</param>
    public void SetWayByWorldDirection(Vector3 _direction, Way _way)
    {
        CardinalDirection direction = GetFacingDirection(_direction);
        SetWayByDirection(direction, _way);
    }

    /// <summary>
    /// 前方ベクトルとターン方向から、目的のWayを取得
    /// </summary>
    /// <param name="_forward">前方ベクトル</param>
    /// <param name="_turnDirection">ターン方向</param>
    /// <returns>目的のWay</returns>
    public Way GetWayByTurn(Vector3 _forward, TurnDirection _turnDirection)
    {
        CardinalDirection facing = GetFacingDirection(_forward);
        CardinalDirection target = RotateDirection(facing, _turnDirection);
        return GetWayByDirection(target);
    }

    /// <summary>
    /// 前方ベクトルから、現在の方角を計算
    /// </summary>
    /// <param name="_forward">前方ベクトル</param>
    /// <returns>現在の方角</returns>
    private CardinalDirection GetFacingDirection(Vector3 _forward)
    {
        Vector3 flat = new(_forward.x, 0f, _forward.z);
        if (flat.sqrMagnitude < 0.001f)
        {
            return CardinalDirection.North;
        }

        flat.Normalize();

        if (Mathf.Abs(flat.z) >= Mathf.Abs(flat.x))
        {
            return flat.z >= 0f ? CardinalDirection.North : CardinalDirection.South;
        }

        return flat.x >= 0f ? CardinalDirection.East : CardinalDirection.West;
    }

    /// <summary>
    /// 現在の方角とターン方向から、目的の方角を計算
    /// </summary>
    /// <param name="_current">現在の方角</param>
    /// <param name="_turnDirection">ターン方向</param>
    /// <returns>目的の方角</returns>
    private CardinalDirection RotateDirection(CardinalDirection _current, TurnDirection _turnDirection)
    {
        int index = (int)_current;
        int offset = _turnDirection switch
        {
            TurnDirection.Straight => 0,
            TurnDirection.Right => 1,
            TurnDirection.Back => 2,
            TurnDirection.Left => 3,
            _ => 0
        };

        int result = (index + offset) % 4;
        return (CardinalDirection)result;
    }

    /// <summary>
    /// 指定された方角に対応するWayを取得
    /// </summary>
    /// <param name="_direction">現在の方角</param>
    /// <returns>指定された方角に対応するWay</returns>
    private Way GetWayByDirection(CardinalDirection _direction)
    {
        return _direction switch
        {
            CardinalDirection.North => NorthWay,
            CardinalDirection.East => EastWay,
            CardinalDirection.South => SouthWay,
            CardinalDirection.West => WestWay,
            _ => null
        };
    }

    /// <summary>
    /// 指定された方角にWayを設定
    /// </summary>
    /// <param name="_direction">設定対象の方角</param>
    /// <param name="_way">設定するWay</param>
    private void SetWayByDirection(CardinalDirection _direction, Way _way)
    {
        switch (_direction)
        {
            case CardinalDirection.North:
                NorthWay = _way;
                break;
            case CardinalDirection.East:
                EastWay = _way;
                break;
            case CardinalDirection.South:
                SouthWay = _way;
                break;
            case CardinalDirection.West:
                WestWay = _way;
                break;
        }
    }

    #region ギズモ関係
    [Header("ギズモ設定")]
    [Tooltip("辺の長さ")]
    [SerializeField] private float sideLength = 5f;
    [Tooltip("ローカル中心位置")]
    [SerializeField] private Vector3 localCenter = Vector3.zero;
    [Tooltip("ギズモの色")]
    [SerializeField] private Color gizmoColor = Color.cyan;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        float half = sideLength * 0.5f;

        Vector3 p0 = transform.TransformPoint(localCenter + new Vector3(-half, 0f, -half));
        Vector3 p1 = transform.TransformPoint(localCenter + new Vector3(half, 0f, -half));
        Vector3 p2 = transform.TransformPoint(localCenter + new Vector3(half, 0f, half));
        Vector3 p3 = transform.TransformPoint(localCenter + new Vector3(-half, 0f, half));

        Gizmos.DrawLine(p0, p1);
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p0);
    }
    #endregion
}