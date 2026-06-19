using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridController : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private LayerMask tileLayer;
    [SerializeField] private LayerMask unitLayer;

    [Header("Debug Colors")]
    [SerializeField] private Color hoverColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private Color selectColor = new Color(1f, 0.92f, 0.016f);
    [SerializeField] private Color rangeColor = new Color(0.2f, 0.6f, 1f, 0.6f); // ★★移動範囲（青色）★★

    private HexTile currentHoveredTile;
    private HexTile currentSelectedTile;
    private HexUnit currentSelectedUnit;

    // ★★現在ハイライトしている移動範囲のタイル群を記憶★★
    private HashSet<HexTile> currentReachableTiles = new HashSet<HexTile>();

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        // 【デバッグ用】スペースキーを押したらサイコロを振る
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RollDiceForTurn();
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        UpdateTileHover();
    }

    // 1. サイコロを振る処理
    private void RollDiceForTurn()
    {
        int diceResult = Random.Range(1, 7); // 1〜6のランダム
        Debug.Log($"<color=cyan>【サイコロ】 {diceResult} が出ました！ユニットを選択してください。</color>");

        // 本来は現在のターンプレイヤーのユニットに割り当てますが、
        // 今回はシーン内のHexUnitを自動検索してセットします（仮実装）
        HexUnit targetUnit = FindObjectOfType<HexUnit>();
        if (targetUnit != null)
        {
            targetUnit.currentRollPoints = diceResult;

            // もしすでにそのユニットを選択中なら、即座に範囲を再計算する
            if (currentSelectedUnit == targetUnit)
            {
                ShowMovementRange(targetUnit);
            }
        }
    }

    private void HandleLeftClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // ユニットのクリック判定
        if (Physics.Raycast(ray, out RaycastHit unitHit, Mathf.Infinity, unitLayer))
        {
            HexUnit clickedUnit = unitHit.collider.GetComponent<HexUnit>();
            if (clickedUnit != null)
            {
                SelectUnit(clickedUnit);
                return;
            }
        }

        // タイルのクリック判定
        if (Physics.Raycast(ray, out RaycastHit tileHit, Mathf.Infinity, tileLayer))
        {
            HexTile clickedTile = tileHit.collider.GetComponent<HexTile>();
            if (clickedTile != null)
            {
                if (currentSelectedUnit != null)
                {
                    // ★★サイコロの移動範囲内に入っているタイルのみ移動を許可する★★
                    if (currentReachableTiles.Contains(clickedTile))
                    {
                        OrderUnitToMove(currentSelectedUnit, clickedTile);
                    }
                    else
                    {
                        Debug.LogWarning("【移動不可】 そのマスはサイコロの出目（移動範囲）の外です。");
                    }
                }
                else
                {
                    SelectTile(clickedTile);
                }
            }
        }
    }

    private void SelectUnit(HexUnit unit)
    {
        if (currentSelectedUnit != null) currentSelectedUnit.SetSelected(false);
        ClearRangeHighlight(); // 前の範囲表示を消す

        currentSelectedUnit = unit;
        currentSelectedUnit.SetSelected(true);

        Debug.Log($"【ユニット選択】 {unit.unitName} が選ばれました。現在の残り移動力: {unit.currentRollPoints}");

        // ★★ユニットを選択したら、即座に移動可能範囲を表示する★★
        if (unit.currentRollPoints > 0)
        {
            ShowMovementRange(unit);
        }
        else
        {
            Debug.LogWarning("【注意】 まだサイコロを振っていないか、移動力がありません。（Spaceキーでサイコロを振る）");
        }
    }

    // ★★移動範囲を計算して青く光らせる★★
    private void ShowMovementRange(HexUnit unit)
    {
        ClearRangeHighlight();

        HexTile startTile = HexGridGenerator.Instance.GetTileAt(unit.axialCoordinate);
        if (startTile == null) return;

        // パスファインディングから範囲を取得
        currentReachableTiles = HexPathfinding.CalculateMovementRange(startTile, unit.currentRollPoints);

        // 範囲内のタイルを青く染める
        foreach (HexTile tile in currentReachableTiles)
        {
            tile.SetColor(rangeColor);
        }
    }

    // ★★ハイライトを元に戻す★★
    private void ClearRangeHighlight()
    {
        foreach (HexTile tile in currentReachableTiles)
        {
            if (tile != null) tile.ResetColor();
        }
        currentReachableTiles.Clear();
    }

    private void OrderUnitToMove(HexUnit unit, HexTile destTile)
    {
        HexTile startTile = HexGridGenerator.Instance.GetTileAt(unit.axialCoordinate);
        List<HexTile> path = HexPathfinding.FindPath(startTile, destTile);

        if (path != null && path.Count > 0)
        {
            ClearRangeHighlight();
            unit.MoveAlongPath(path);

            // × ここで「unit.axialCoordinate = destTile.axialCoordinate;」などを
            // フライングして書いてしまっていると、コルーチンと衝突してデータが壊れます。
            // 座標の更新は、上記の「ユニットのコルーチン内」の1箇所だけに絞ってください。

            currentSelectedUnit = null;
        }
    }

    // (SelectTile, UpdateTileHover, ClearHover は前回と同じなため省略)
    private void SelectTile(HexTile tile)
    {
        if (currentSelectedTile != null) currentSelectedTile.ResetColor();
        currentSelectedTile = tile;
        currentSelectedTile.SetColor(selectColor);
    }

    private void UpdateTileHover()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, tileLayer))
        {
            HexTile tile = hit.collider.GetComponent<HexTile>();
            if (tile != null)
            {
                if (currentHoveredTile != tile)
                {
                    ClearHover();
                    currentHoveredTile = tile;
                    // 選択中・範囲表示中のタイルでなければホバー色にする
                    if (currentHoveredTile != currentSelectedTile && !currentReachableTiles.Contains(currentHoveredTile))
                    {
                        currentHoveredTile.SetColor(hoverColor);
                    }
                }
                return;
            }
        }
        ClearHover();
    }

    private void ClearHover()
    {
        if (currentHoveredTile != null)
        {
            if (currentHoveredTile != currentSelectedTile && !currentReachableTiles.Contains(currentHoveredTile))
            {
                currentHoveredTile.ResetColor();
            }
            currentHoveredTile = null;
        }
    }
}