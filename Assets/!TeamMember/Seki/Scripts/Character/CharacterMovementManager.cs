using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour{
    public static CharacterMovementManager instance { get; private set; } = null;
    // 動かすキャラクター
    private CharacterBase _movementCharacter = null;
    // 移動リスト
    private List<HexTileData> _moveTileList = new List<HexTileData>();
    // 移動可能リスト
    private List<HexTileData> _movableTileList = new List<HexTileData>();

    private void Awake() {
        instance = this;
    }
    /// <summary>
    /// 動かす対象キャラの設定
    /// </summary>
    /// <param name="character"></param>
    public void SetMovementCharacter(CharacterBase character) {
        _movementCharacter = character;
    }
    /// <summary>
    /// 移動可能なタイルの設定
    /// </summary>
    /// <param name="_setMovableList"></param>
    public void SetMovableTileList(List<HexTileData> _setMovableList) {
        _movableTileList = _setMovableList;
    }
    /// <summary>
    /// 移動ルートの決定
    /// </summary>
    /// <param name="targetTile"></param>
    public void DecideMoveRoute(HexTileData targetTile, bool isEnemy = false) {
        // 自身のタイルの取得
        HexTileData startTile = HexTileManager.instance.GetTileData(_movementCharacter.GetTileID());
        // ルート検索
        _moveTileList = HexRouteSearcher.FindPath(startTile, targetTile, isEnemy);
    }
    /// <summary>
    /// キャラクターを動かす処理
    /// </summary>
    /// <returns></returns>
    public async UniTask MoveCharacter() {
        await _movementCharacter.MoveAsync(_moveTileList);
        // 移動タイルの解除
        foreach(var tile in _movableTileList) {
            tile.SetTileState(eTileState.Normal);
        }
    }

}
