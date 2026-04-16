using UnityEngine;

/// <summary>
/// プレイヤーカメラ
/// </summary>
public class PlayerCamera : MonoBehaviour
{
    #region メンバ変数
    [Tooltip("プレイヤーオブジェクト")]
    [SerializeField] private Transform target;

    [Tooltip("カメラオフセット")]
    [SerializeField] private Vector3 followOffset = new(0f, 5f, -10f);

    [Tooltip("注視点オフセット（プレイヤーローカル座標）")]
    [SerializeField] private Vector3 lookAtOffset = new(0f, 1f, 0f);

    [Tooltip("位置追従のスムーズさ")]
    [SerializeField] private float positionSmoothTime = 0.1f;

    [Tooltip("回転追従のスムーズさ")]
    [SerializeField] private float rotationSmoothTime = 0.1f;

    private Vector3 followVelocity = Vector3.zero; // 位置追従の速度
    #endregion // メンバ変数

    #region Unityイベント
    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("[PlayerCamera] ターゲットが未設定。");
            return;
        }

        // 初期位置
        transform.position = target.TransformPoint(followOffset);

        // 初期回転
        var lookTarget = target.TransformPoint(lookAtOffset);
        transform.rotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // 目標位置（プレイヤーローカル基準）
        var desiredPosition = target.TransformPoint(followOffset);
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            positionSmoothTime);

        // 注視点もプレイヤーローカル基準に統一
        var lookTarget = target.TransformPoint(lookAtOffset);

        // 回転目標は目標位置基準で作るとブレにくい
        var desiredRotation = Quaternion.LookRotation(lookTarget - desiredPosition, Vector3.up);

        // smoothTime ベースの補間率に変換
        var t = ToSmoothLerpFactor(rotationSmoothTime, Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, t);
    }
    #endregion // Unityイベント

    #region privateメソッド
    private static float ToSmoothLerpFactor(float smoothTime, float deltaTime)
    {
        if (smoothTime <= 0f)
        {
            return 1f;
        }

        return 1f - Mathf.Exp(-deltaTime / smoothTime);
    }
    #endregion // privateメソッド
}
