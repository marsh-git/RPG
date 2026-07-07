using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableSelectionManager : MonoBehaviour {
    public static ClickableSelectionManager instance { get; private set; }

    // 現在光っているタイルを覚えておくリスト（次にクリックした時に消すため）
    private List<HexTileObject> _highlightedTileObjects = new List<HexTileObject>();

    private void Awake() {
        instance = this;
    }

    /// <summary>
    /// タイルがクリックされた時のメイン処理
    /// </summary>
    public void OnTileClicked(HexTileData centerTile) {
        // 1. まず今光っているタイルをすべて消す
        ClearHighlights();

        // 2. 範囲を計算する（例：以前作った扇形ロジックなどを使用）
        // とりあえず今回は「クリックされたタイル自身と、全方向の隣接マス」を光らせる例
        List<HexTileData> rangeTiles = TileRangeExpansion.GetFanShapedTile(centerTile, eDirectionHex.UpRight);
        // ※↑向きはキャラの向きなどを渡せるようにするとベスト

        // 3. 範囲内のタイルを一つずつ光らせる
        foreach(var tileData in rangeTiles) {
            // HexTileManagerから、データに対応する「見た目(Object)」を探す
            HexTileObject tileObj = HexTileManager.instance.GetTileObject(tileData.ID);

            if(tileObj != null) {
                tileObj.SetHighlight(true);
                _highlightedTileObjects.Add(tileObj); // 消す時のためにリストに入れる
            }
        }
    }

    /// <summary>
    /// すべてのハイライトを消去する
    /// </summary>
    public void ClearHighlights() {
        foreach(var obj in _highlightedTileObjects) {
            obj.SetHighlight(false);
        }
        _highlightedTileObjects.Clear();
    }
}
