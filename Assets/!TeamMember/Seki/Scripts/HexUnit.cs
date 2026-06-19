using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexUnit : MonoBehaviour
{
    [Header("Unit Data")]
    public string unitName = "Settler";
    public Vector2Int axialCoordinate; // 現在いるHex座標
    public int movementPoints = 3;     // 1ターンに移動できる行動力

    [Header("Visual")]
    [SerializeField] private Renderer unitRenderer;
    private Color originalColor;
    private MaterialPropertyBlock propertyBlock;

    private bool isSelected = false;

    [Header("Dice Mechanics")]
    public int currentRollPoints = 0; // サイコロで出た現在の移動力

    void Awake()
    {
        if (unitRenderer == null) unitRenderer = GetComponentInChildren<Renderer>();
        propertyBlock = new MaterialPropertyBlock();

        if (unitRenderer != null)
        {
            originalColor = unitRenderer.material.color;
        }
    }

    // ユニットを特定のHex座標に配置する
    public void SetPosition(Vector2Int newCoordinate, Vector3 worldPosition)
    {
        axialCoordinate = newCoordinate;

        // ユニットのY軸位置を少し浮かせる（地面に埋まらないように調整）
        transform.position = worldPosition + new Vector3(0, 0.5f, 0);
    }

    // 選択状態の切り替え
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        if (unitRenderer == null) return;

        unitRenderer.GetPropertyBlock(propertyBlock);
        if (isSelected)
        {
            // 選択されたら明るい赤色にする（デバッグ用）
            propertyBlock.SetColor("_Color", Color.red);
            // 少し上に浮かせて選択感を出す
            transform.position += new Vector3(0, 0.3f, 0);
        }
        else
        {
            propertyBlock.SetColor("_Color", originalColor);
            // 位置を戻す
            transform.position -= new Vector3(0, 0.3f, 0);
        }
        unitRenderer.SetPropertyBlock(propertyBlock);
    }
    public void MoveAlongPath(List<HexTile> path)
    {
        if (path == null || path.Count == 0) return;

        // 現在走っている移動コルーチンがあれば止める（連打対策）
        StopAllCoroutines();
        StartCoroutine(MoveCoroutine(path));
    }

    private IEnumerator MoveCoroutine(List<HexTile> path)
    {
        SetSelected(false);

        foreach (HexTile nextTile in path)
        {
            Vector3 startPos = transform.position;
            Vector3 endPos = nextTile.transform.position + new Vector3(0, 0.5f, 0);

            float elapsedTime = 0f;
            float duration = 0.2f; // 移動速度（少し速めに調整）

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / duration);
                yield return null;
            }

            transform.position = endPos;

            // ★★重要：1マス進むごとに、ユニットの内部座標をそのマスの座標で【即座に上書き】する★★
            this.axialCoordinate = nextTile.axialCoordinate;
        }

        // 移動力を消費（前回の仕様をここに確実に割り当てる）
        currentRollPoints = 0;

        Debug.Log($"【データ同期完了】 {unitName} の内部座標が {this.axialCoordinate} に更新されました。");
    }
}