using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableSelectionManager : MonoBehaviour {
    public static ClickableSelectionManager instance { get; private set; }
    /// <summary>
    /// ハイライト構造体
    /// </summary>
    private struct HighlightData {
        public HexTileObject hexTileObject;
        public eTileHighlight tileHighlight;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="highlight"></param>
        public HighlightData(HexTileObject obj, eTileHighlight highlight) {
            this.hexTileObject = obj;
            this.tileHighlight = highlight;
        }
    }

    // 現在光っているタイルを覚えておくリスト
    private List<HighlightData> _highlighDataList = new List<HighlightData>();

    private void Awake() {
        instance = this;
    }

    /// <summary>
    /// タイルクリック処理
    /// </summary>
    /// <param name="targetTile"></param>
    /// <param name="isClear"></param>
    /// <param name="setHighlight"></param>
    public void OnTileHighlight(HexTileData targetTile,
        bool isClear = true, 
        eTileHighlight setHighlight = eTileHighlight.TileHighlight) {
        if(targetTile == null) return;
        // 今光っているタイルをすべて消す
        if(isClear) ClearHighlights();
        // データに対応するオブジェクトを探す
        HexTileObject tileObj = HexTileManager.instance.GetTileObject(targetTile.ID);
        if(tileObj != null) {
            tileObj.SetHighlight(true, setHighlight);
            _highlighDataList.Add(new HighlightData(tileObj, setHighlight));
        }
    }
    /// <summary>
    /// 範囲内のタイルを全てハイライト
    /// </summary>
    /// <param name="highlightTileList"></param>
    /// <param name="isClear"></param>
    /// <param name="setHighlight"></param>
    public void HighlightRangeTile(List<HexTileData> highlightTileList,
        bool isClear = true,
        eTileHighlight setHighlight = eTileHighlight.LineHighlight) {
        // 今光っているタイルをすべて消す
        if(isClear) ClearHighlights();
        // 範囲内のタイルを一つずつ光らせる
        foreach(var tileData in highlightTileList) {
            if(tileData == null) continue;
            // データに対応するオブジェクトを探す
            HexTileObject tileObj = HexTileManager.instance.GetTileObject(tileData.ID);
            if(tileObj != null) {
                tileObj.SetHighlight(true, setHighlight);
                _highlighDataList.Add(new HighlightData(tileObj, setHighlight));
            }
        }
    }
    /// <summary>
    /// すべてのハイライトを消去する
    /// </summary>
    public void ClearHighlights() {
        foreach(var data in _highlighDataList) {
            if(data.hexTileObject != null) data.hexTileObject.SetHighlight(false, data.tileHighlight);
        }
        _highlighDataList.Clear();
    }
}
