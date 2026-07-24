using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttributeTile {
    /// <summary>
    /// 自身の属性取得
    /// </summary>
    eAttribute AttributeType { get; }
    /// <summary>
    /// キャラクターがこのマスを踏んだ瞬間
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    void OnEnterTile(HexTileData tile, CharacterBase character = null);
    /// <summary>
    /// キャラクターがこのマスでターンを終了時処理
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    void OnTurnEndOnTile(HexTileData tile, CharacterBase character = null);
    /// <summary>
    /// ターン経過によるタイル処理
    /// </summary>
    /// <param name="tile"></param>
    void OnTickTile(HexTileData tile);
    /// <summary>
    /// プレイヤーに最初に見つかった時の処理
    /// </summary>
    /// <param name="tile"></param>
    void OnFoundThisFirst(HexTileData tile);
}