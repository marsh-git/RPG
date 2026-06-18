using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // メインカメラ
    [SerializeField]
    private Camera mainCamera;

    // WASD移動速度
    [SerializeField]
    private float moveSpeed = 20f;

    // ドラッグ移動速度
    [SerializeField]
    private float dragSpeed = 0.02f;

    // ズーム速度
    [SerializeField]
    private float zoomSpeed = 3f;

    // ズーム補間速度
    [SerializeField]
    private float zoomSmoothTime = 0.15f;

    // 最小ズーム距離
    [SerializeField]
    private float minDistance = 10f;

    // 最大ズーム距離
    [SerializeField]
    private float maxDistance = 30f;

    // カメラピッチ
    [SerializeField]
    private float pitch = 55f;

    // 初期距離
    [SerializeField]
    private float distance = 20f;

    // 現在距離
    private float currentDistance;

    // 目標距離
    private float targetDistance;

    // SmoothDamp用速度
    private float zoomVelocity;

    // ドラッグ開始時マウス位置
    private Vector3 dragStartMousePosition;

    // ドラッグ開始時カメラ位置
    private Vector3 dragStartCameraPosition;

    private void Awake()
    {
        // カメラ未設定なら自動取得
        if (mainCamera == null)
        {
            // 子オブジェクトから取得
            mainCamera = GetComponentInChildren<Camera>();
        }

        // 初期距離設定
        currentDistance = distance;

        // 目標距離設定
        targetDistance = distance;

        // カメラ角度設定
        mainCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);

        // カメラ位置更新
        UpdateCameraPosition();
    }

    private void Update()
    {
        // WASD移動
        MoveByKeyboard();

        // ドラッグ移動
        MoveByDrag();

        // ホイールズーム
        Zoom();

        // ズーム補間
        UpdateZoom();
    }

    /// <summary>
    /// WASD移動
    /// </summary>
    private void MoveByKeyboard()
    {
        // 横入力取得
        float horizontal =
            Input.GetAxisRaw("Horizontal");

        // 縦入力取得
        float vertical =
            Input.GetAxisRaw("Vertical");

        // 移動方向
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);

        // 正規化
        moveDirection = moveDirection.normalized;

        // 移動
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 右ドラッグ移動
    /// </summary>
    private void MoveByDrag()
    {
        // ドラッグ開始
        if (Input.GetMouseButtonDown(1))
        {
            // マウス位置保存
            dragStartMousePosition = Input.mousePosition;

            // カメラ位置保存
            dragStartCameraPosition = transform.position;
        }

        // ドラッグ中
        if (Input.GetMouseButton(1))
        {
            // マウス差分取得
            Vector3 mouseDelta = Input.mousePosition - dragStartMousePosition;

            // 右方向取得
            Vector3 right = transform.right;

            // Y除外
            right.y = 0f;

            // 正規化
            right.Normalize();

            // 前方向取得
            Vector3 forward = transform.forward;

            // Y除外
            forward.y = 0f;

            // 正規化
            forward.Normalize();

            // 移動量計算
            Vector3 move = (-right * mouseDelta.x - forward * mouseDelta.y) * dragSpeed;

            // 位置更新
            transform.position = dragStartCameraPosition + move;
        }
    }

    /// <summary>
    /// ホイールズーム
    /// </summary>
    private void Zoom()
    {
        // ホイール入力取得
        float scroll =
            Input.mouseScrollDelta.y;

        // 入力なし
        if (Mathf.Approximately(scroll, 0f))
        {
            return;
        }

        // 目標距離変更
        targetDistance -= scroll * zoomSpeed;

        // 制限
        targetDistance = Mathf.Clamp(targetDistance, minDistance, maxDistance);
    }

    /// <summary>
    /// ズーム更新
    /// </summary>
    private void UpdateZoom()
    {
        // スムーズ補間
        currentDistance = Mathf.SmoothDamp(currentDistance, targetDistance, ref zoomVelocity, zoomSmoothTime);

        // カメラ位置更新
        UpdateCameraPosition();
    }

    /// <summary>
    /// カメラ位置更新
    /// </summary>
    private void UpdateCameraPosition()
    {
        // ローカル位置設定
        mainCamera.transform.localPosition =
            Quaternion.Euler(pitch, 0f, 0f) * new Vector3(0f, 0f, -currentDistance);
    }

}
