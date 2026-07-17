using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointAttribute : IAttributeTile {
    private List<int> _checkPlayerIDList = null;

    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.CheckPoint;
    /// <summary>
    /// キャラクターがこのマスを踏んだ瞬間
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnEnterTile(HexTileData tile, CharacterBase character = null) {
        
    }
    /// <summary>
    /// キャラクターがこのマスでターンを終了時処理
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnTurnEndOnTile(HexTileData tile, CharacterBase character = null) {
        throw new System.NotImplementedException();
    }
    /// <summary>
    /// ターン経過によるタイル処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnTickTile(HexTileData tile) {
        throw new System.NotImplementedException();
    }

}
