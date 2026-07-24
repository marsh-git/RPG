using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutpostAttribute : IAttributeTile {
    private int _turnSpawnCounter = -1;
    private int _maxSpawnCounter = -1;

    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Outpost;
    /// <summary>
    /// キャラクターがこのマスを踏んだ瞬間
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnEnterTile(HexTileData tile, CharacterBase character = null) {
        // 自身の属性の変更
        tile.SetAttributeTile(null);
    }
    /// <summary>
    /// キャラクターがこのマスでターンを終了時処理
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnTurnEndOnTile(HexTileData tile, CharacterBase character = null) {

    }
    /// <summary>
    /// ターン経過によるタイル処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnTickTile(HexTileData tile) {

        Debug.Log($"前哨基地ターン処理 : {tile.ID}");

        // カウンターを減らす
        _turnSpawnCounter--;

        // カウンターが0になるまで何もしない
        if (_turnSpawnCounter > 0) return;

        // 次回生成までのカウンターをリセット
        _turnSpawnCounter = _maxSpawnCounter;

        // 敵生成
        EnemySpawner.Instance.SpawnEnemy(tile);
    }
    /// <summary>
    /// 準備処理
    /// </summary>
    /// <param name="setCounter"></param>
    public void Setup(int setCounter) {
        _turnSpawnCounter = setCounter;
        _maxSpawnCounter = setCounter;
    }
    /// <summary>
    /// プレイヤーに見つかった時の処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnFoundThisFirst(HexTileData tile) {

    }
}
