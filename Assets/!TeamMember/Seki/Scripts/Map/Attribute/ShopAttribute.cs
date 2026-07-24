using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopAttribute : IAttributeTile {
    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Shop;
    /// <summary>
    /// キャラクターがこのマスを踏んだ瞬間
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnEnterTile(HexTileData tile, CharacterBase character = null) {
        // ショップUIを開く
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
        
    }
    /// <summary>
    /// プレイヤーに最初に見つかった時の処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnFoundThisFirst(HexTileData tile) {
        
    }
}
