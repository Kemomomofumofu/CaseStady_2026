using UnityEngine;

/// <summary>
/// Lane上の障害物
/// </summary>
public class LaneObstacle : MonoBehaviour
{
    [Header("所属レーン")]
    [SerializeField] private Lane lane;

    [Header("Lane上の占有区間")]
    [SerializeField] private float sStart = 3.0f;
    [SerializeField] private float sEnd = 5.0f;

    [Header("見た目")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private float visualHeightOffset = 0.5f;
    [SerializeField] private float visualWidth = 1.0f;
    [SerializeField] private float visualLengthScale = 1.0f;

    public Lane Lane => lane;
    public float SStart => sStart;
    public float SEnd => sEnd;

    /// <summary>
    /// 指定したs位置がこの障害物の占有区間内にあるか
    /// </summary>
    public bool Contains(float _s)
    {
        return _s >= sStart && _s <= sEnd;
    }

    /// <summary>
    /// エディタ上で値が変わったとき、見た目を更新する
    /// </summary>
    private void OnValidate()
    {
        SyncVisual();
    }

    /// <summary>
    /// 開始時に見た目を更新する
    /// </summary>
    private void Start()
    {
        SyncVisual();
    }

    /// <summary>
    /// Lane情報から見た目の位置・回転・長さを同期する
    /// </summary>
    public void SyncVisual()
    {
        if (lane == null || visualRoot == null)
        {
            return;
        }

        float centerS = (sStart + sEnd) * 0.5f;
        float length = Mathf.Max(0.1f, sEnd - sStart);

        Vector3 position = lane.GetPositionByS(centerS);
        Vector3 forward = lane.GetForwardByS(centerS);

        visualRoot.position = position + Vector3.up * visualHeightOffset;

        if (forward.sqrMagnitude > 0.0001f)
        {
            visualRoot.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

        visualRoot.localScale = new Vector3(visualWidth, 1.0f, length * visualLengthScale);
    }

    /// <summary>
    /// 障害物の配置情報を設定する
    /// </summary>
    /// <param name="_lane">所属Lane</param>
    /// <param name="_sStart">占有開始s位置</param>
    /// <param name="_sEnd">占有終了s位置</param>
    public void Setup(Lane _lane, float _sStart, float _sEnd)
    {
        lane = _lane;
        sStart = _sStart;
        sEnd = _sEnd;
    }

    /// <summary>
    /// 破棄時にLaneから自分を解除する
    /// </summary>
    private void OnDestroy()
    {
        if (lane != null)
        {
            lane.RemoveObstacle(this);
        }
    }
}