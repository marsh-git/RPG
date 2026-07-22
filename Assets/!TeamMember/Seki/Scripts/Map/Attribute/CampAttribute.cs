using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampAttribute : IAttributeTile {
    // クールタイマーを計るカウンター
    private int _useCoolTimer = -1;
    // キャンプ使用フラグ
    private bool _isUse = false;

    private const int _COOL_TIMER = 5;

    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Camp;
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
        if(!_isUse) return;

        _useCoolTimer--;
        if(_useCoolTimer <= 0) {
            _useCoolTimer = _COOL_TIMER;
            _isUse = false;
        }
    }
    /// <summary>
    /// 準備処理
    /// </summary>
    public void Setup() {
        _isUse = false;
        _useCoolTimer = _COOL_TIMER;
    }
    /// <summary>
    /// キャンプ使用処理
    /// </summary>
    public void UseCamp() {
        _isUse = true;
    }
}
