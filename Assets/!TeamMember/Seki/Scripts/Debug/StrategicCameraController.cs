using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategicCameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Camera targetCamera;

    [Header("Movement (Pan)")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float dragSpeed = 1.5f;

    [Header("Zoom & Rotation Limits")]
    [SerializeField] private float minZoomDistance = 5f;   // 最も近づいたときの距離
    [SerializeField] private float maxZoomDistance = 40f;  // 最も遠ざかったときの距離
    [SerializeField] private float zoomSensitivity = 10f;

    // Civ6風：ズーム値に応じたカメラのX軸回転角度（お好みで調整してください）
    [SerializeField] private float minZoomAngle = 30f;    // ズームイン時の寝た角度（地形が見える）
    [SerializeField] private float maxZoomAngle = 70f;    // ズームアウト時の見下ろし角度

    [Header("Edge Scroll")]
    [SerializeField] private bool useEdgeScroll = true;
    [SerializeField] private float edgeThreshold = 15f;    // 画面端から何ピクセルでスクロールするか

    private float currentZoomPercent = 0.5f; // 0 (最接近) ～ 1 (最後方)
    private Vector3 dragOrigin;

    void Start()
    {
        if (targetCamera == null) targetCamera = Camera.main;

        // 初期カメラ位置の計算
        ApplyZoomAndRotation();
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        if (useEdgeScroll) HandleEdgeScroll();

        // 毎フレーム、ズームと角度の連動を適用
        ApplyZoomAndRotation();
    }

    // 1. キー入力およびマウスドラッグによる移動
    private void HandleMovement()
    {
        Vector3 moveDirection = Vector3.zero;

        // キーボード入力 (WASD / 矢印キー)
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        moveDirection += new Vector3(h, 0, v).normalized;

        // マウス右ドラッグによるパン移動
        if (Input.GetMouseButtonDown(1))
        {
            dragOrigin = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            Vector3 diff = Input.mousePosition - dragOrigin;
            // カメラの向きに合わせて移動方向を補正（Y軸回転を考慮）
            Vector3 forward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            Vector3 right = transform.right;

            Vector3 dragMove = (right * -diff.x + forward * -diff.y) * dragSpeed * Time.deltaTime;
            transform.position += dragMove;
            dragOrigin = Input.mousePosition;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    // 2. マウスホイールによるズーム操作
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // スクロール方向に応じてズーム割合（0～1）を増減
            currentZoomPercent -= scroll * zoomSensitivity;
            currentZoomPercent = Mathf.Clamp01(currentZoomPercent);
        }
    }

    // 3. ズーム量に応じた距離と角度の計算（Civ6の肝）
    private void ApplyZoomAndRotation()
    {
        // 線形補間（Lerp）を使って、現在のズーム割合に応じた距離と角度を算出
        float currentDistance = Mathf.Lerp(minZoomDistance, maxZoomDistance, currentZoomPercent);
        float currentAngle = Mathf.Lerp(minZoomAngle, maxZoomAngle, currentZoomPercent);

        // 親（土台）の回転を設定（X軸を見下ろし角度に、Y軸はそのまま）
        transform.rotation = Quaternion.Euler(currentAngle, transform.rotation.eulerAngles.y, 0);

        // 子（カメラ）を、土台の後方（-Z方向）に指定距離だけ離す
        targetCamera.transform.localPosition = new Vector3(0, 0, -currentDistance);
        targetCamera.transform.localRotation = Quaternion.identity; // 親と完全に同じ向きを向かせる
    }

    // 4. 画面端にマウスを置いたときのスクロール
    private void HandleEdgeScroll()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 moveDirection = Vector3.zero;

        // マウスが画面の有効範囲内にいるときのみ処理
        if (mousePos.x >= 0 && mousePos.x <= Screen.width && mousePos.y >= 0 && mousePos.y <= Screen.height)
        {
            if (mousePos.x < edgeThreshold) moveDirection.x = -1;
            else if (mousePos.x > Screen.width - edgeThreshold) moveDirection.x = 1;

            if (mousePos.y < edgeThreshold) moveDirection.z = -1;
            else if (mousePos.y > Screen.height - edgeThreshold) moveDirection.z = 1;

            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
    }
}