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
        // カウンターを減らす
        _turnSpawnCounter--;
        // カウンターが0になったら敵の生成処理
        if(_turnSpawnCounter <= 0) {
            // TODO : そのうち敵のプール内からの生成

            // 有効なタイル取得
            
        }
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
