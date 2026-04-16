using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Way内の車線を表すクラス
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public class Lane : MonoBehaviour
{
    [Header("所属")]
    [SerializeField] private Way parentWay;

    [Header("車線情報")]
    [SerializeField] private int laneIndex = 0;

    [Header("中心線")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("交差点接続")]
    [SerializeField] private List<LaneLink> nextLaneLinks = new();

    [Header("障害物")]
    [SerializeField] private List<LaneObstacle> obstacles = new();

    [Header("描画")]
    [SerializeField] private bool useLineRenderer = true;
    [SerializeField] private float lineWidth = 0.15f;
    [SerializeField] private Color laneColor = new(0.4f, 0.9f, 1f, 1f);
    [SerializeField] private LineRenderer lineRenderer;

    public Way ParentWay => parentWay;
    public int LaneIndex => laneIndex;
    public Transform StartPoint => startPoint;
    public Transform EndPoint => endPoint;

    private void OnEnable()
    {
        InitializeLineRenderer();
        UpdateLineRenderer();
    }

    private void Awake()
    {
        InitializeLineRenderer();
        UpdateLineRenderer();
    }

    private void Update()
    {
        UpdateLineRenderer();
    }

    private void OnValidate()
    {
        InitializeLineRenderer();
        UpdateLineRenderer();
    }

    /// <summary>
    /// LineRendererコンポーネントを初期化
    /// </summary>
    private void InitializeLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        if (lineRenderer == null)
        {
            return;
        }

        lineRenderer.useWorldSpace = true;
        lineRenderer.positionCount = 2;
    }

    /// <summary>
    /// LineRendererの位置や色を更新
    /// </summary>
    private void UpdateLineRenderer()
    {
        if (lineRenderer == null)
        {
            return;
        }

        if (!useLineRenderer)
        {
            lineRenderer.enabled = false;
            return;
        }

        if (!TryGetLinePoints(out Vector3 start, out Vector3 end))
        {
            lineRenderer.enabled = false;
            return;
        }

        lineRenderer.enabled = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.startColor = laneColor;
        lineRenderer.endColor = laneColor;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

    /// <summary>
    /// startPointとendPointから描画用の線の始点と終点を計算する
    /// </summary>
    /// <param name="_start">計算結果の始点</param>
    /// <param name="_end">計算結果の終点</param>
    /// <returns>計算が成功したかどうか</returns>
    private bool TryGetLinePoints(out Vector3 _start, out Vector3 _end)
    {
        _start = default;
        _end = default;

        if (startPoint == null || endPoint == null)
        {
            return false;
        }

        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;

        Vector3 forward = end - start;
        if (forward.sqrMagnitude <= 1e-6f)
        {
            return false;
        }

        // 描画と移動で共通の横オフセット
        Vector3 offset = GetLateralOffsetVector(forward);

        _start = start + offset;
        _end = end + offset;
        return true;
    }

    /// <summary>
    /// forwardベクトルに対して、この車線の横オフセットを加算するためのベクトルを計算して返す
    /// </summary>
    /// <param name="_forward">基準となる前方ベクトル</param>
    /// <returns>横オフセットベクトル</returns>
    private Vector3 GetLateralOffsetVector(Vector3 _forward)
    {
        Vector3 flatForward = new(_forward.x, 0f, _forward.z);
        if (flatForward.sqrMagnitude <= 1e-6f)
        {
            return Vector3.zero;
        }

        float lateralOffset = GetLateralOffset();
        Vector3 right = Vector3.Cross(Vector3.up, flatForward.normalized);
        return right * lateralOffset;
    }

    /// <summary>
    /// この車線の長さを返す(水平距離のみを考慮)
    /// startPointとendPointが設定されていない場合は0を返す
    /// </summary>
    public float Length
    {
        get
        {
            if (startPoint == null || endPoint == null)
            {
                return 0f;
            }

            Vector3 diff = endPoint.position - startPoint.position;
            diff.y = 0f; // 水平距離のみを考慮

            return diff.magnitude;
        }
    }

    /// <summary>
    /// この車線上のs位置に対応するワールド座標を返す
    /// </summary>
    /// <param name="_s">車線上の位置</param>
    /// <returns>ワールド座標</returns>
    public Vector3 GetPositionByS(float _s)
    {
        if (startPoint == null || endPoint == null)
        {
            return Vector3.zero;
        }

        Vector3 start = startPoint.position;
        Vector3 end = endPoint.position;
        Vector3 forward = end - start;

        float len = Length;
        if (len <= 1e-6f)
        {
            return start + GetLateralOffsetVector(forward);
        }

        float t = Mathf.Clamp01(_s / len);
        Vector3 centerPos = Vector3.Lerp(start, end, t);

        // 描画線と同じ横オフセットを加算
        return centerPos + GetLateralOffsetVector(forward);
    }

    /// <summary>
    /// この車線上のs位置に対応する前方ベクトルを返す
    /// </summary>
    /// <param name="_s">車線上の位置</param>
    /// <returns>前方ベクトル</returns>
    public Vector3 GetForwardByS(float _s)
    {
        if (startPoint == null || endPoint == null)
        {
            return Vector3.forward;
        }

        Vector3 dir = endPoint.position - startPoint.position;
        dir.y = 0f; // 水平成分のみを考慮
        if (dir.sqrMagnitude <= 1e-6f)
        {
            return Vector3.forward;
        }

        return dir.normalized;
    }

    /// <summary>
    /// この車線から指定した方向に進むときの次の車線を返す
    /// </summary>
    /// <param name="_turnDirection">曲がる方向</param>
    /// <returns>次の車線</returns>
    public Lane GetNextLane(TurnDirection _turnDirection)
    {
        for (int i = 0; i < nextLaneLinks.Count; ++i)
        {
            if (nextLaneLinks[i] != null && nextLaneLinks[i].TurnDirection == _turnDirection)
            {
                return nextLaneLinks[i].NextLane;
            }
        }

        return null;
    }

    /// <summary>
    /// この車線上のs位置に障害物があるかどうかを返す
    /// </summary>
    /// <param name="_s">車線上の位置</param>
    /// <returns>障害物がある場合はtrue、ない場合はfalse</returns>
    public bool HasObstacleAt(float _s)
    {
        for (int i = 0; i < obstacles.Count; ++i)
        {
            if (obstacles[i] == null)
            {
                continue;
            }

            if (obstacles[i].Contains(_s))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// この車線上のs位置より前にある最も近い障害物を返す
    /// </summary>
    /// <param name="_currentS">現在のs位置</param>
    /// <param name="_nearestObstacle">最も近い障害物</param>
    /// <returns>障害物が見つかった場合はtrue、見つからなかった場合はfalse</returns>
    public bool TryGetNearestObstacleForward(float _currentS, out LaneObstacle _nearestObstacle)
    {
        _nearestObstacle = null;
        float bestS = float.MaxValue;

        for (int i = 0; i < obstacles.Count; ++i)
        {
            LaneObstacle obstacle = obstacles[i];
            if (obstacle == null)
            {
                continue;
            }

            if (obstacle.SStart >= _currentS && obstacle.SStart < bestS)
            {
                bestS = obstacle.SStart;
                _nearestObstacle = obstacle;
            }
        }

        return _nearestObstacle != null;
    }

    /// <summary>
    /// この車線上のs位置にある障害物を取得する
    /// </summary>
    /// <param name="_s">車線上の位置</param>
    /// <param name="_obstacle">見つかった障害物</param>
    /// <returns>障害物が見つかった場合はtrue</returns>
    public bool TryGetObstacleAt(float _s, out LaneObstacle _obstacle)
    {
        _obstacle = null;

        for (int i = 0; i < obstacles.Count; ++i)
        {
            LaneObstacle obstacle = obstacles[i];
            if (obstacle == null)
            {
                continue;
            }

            if (obstacle.Contains(_s))
            {
                _obstacle = obstacle;
                return true;
            }
        }

        return false;
    }

    private float GetLateralOffset()
    {
        if (parentWay == null || parentWay.Lanes == null)
        {
            return 0f;
        }

        int laneCount = 0;
        int order = 0;

        // LaneIndex昇順で順番を決める
        for (int i = 0; i < parentWay.Lanes.Count; ++i)
        {
            Lane lane = parentWay.Lanes[i];
            if (lane == null)
            {
                continue;
            }

            laneCount++;

            if (lane != this && lane.LaneIndex < laneIndex)
            {
                order++;
            }
        }

        if (laneCount <= 1)
        {
            return parentWay.LaneGroupOffset;
        }

        float usableWidth = Mathf.Max(0.0f, parentWay.RoadThickness) * Mathf.Clamp01(parentWay.LaneSpreadRatio);
        float laneStep = usableWidth / laneCount;

        // 左車線想定
        float leftOffsetFromCenter = -((order + 0.5f) * laneStep);

        return leftOffsetFromCenter + parentWay.LaneGroupOffset;
    }

    /// <summary>
    /// 障害物をこのLaneに登録する
    /// </summary>
    public void AddObstacle(LaneObstacle _obstacle)
    {
        if (_obstacle == null)
        {
            return;
        }

        if (!obstacles.Contains(_obstacle))
        {
            obstacles.Add(_obstacle);
        }
    }

    /// <summary>
    /// 障害物をこのLaneから解除する
    /// </summary>
    public void RemoveObstacle(LaneObstacle _obstacle)
    {
        if (_obstacle == null)
        {
            return;
        }

        obstacles.Remove(_obstacle);
    }

    /// <summary>
    /// 指定区間に重なる障害物が存在するか
    /// </summary>
    /// <param name="_sStart">開始s位置</param>
    /// <param name="_sEnd">終了s位置</param>
    /// <returns>重なる障害物がある場合はtrue</returns>
    public bool HasObstacleInRange(float _sStart, float _sEnd)
    {
        for (int i = 0; i < obstacles.Count; ++i)
        {
            LaneObstacle obstacle = obstacles[i];
            if (obstacle == null)
            {
                continue;
            }

            bool isOverlapping = _sStart <= obstacle.SEnd && _sEnd >= obstacle.SStart;
            if (isOverlapping)
            {
                return true;
            }
        }

        return false;
    }
}
