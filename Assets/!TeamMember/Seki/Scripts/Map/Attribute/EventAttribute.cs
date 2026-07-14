using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventAttribute : IAttributeTile{
    // イベントID
    public int eventID { get; private set; } = -1;
    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Event;
    /// <summary>
    /// 準備処理
    /// </summary>
    /// <param name="setID"></param>
    public void Setup(int setEventID) {
        eventID = setEventID;
    }
    /// <summary>
    /// キャラクターがこのマスを踏んだ瞬間の処理
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnEnterTile(HexTileData tile, CharacterBase character) {
        if(character.IsEnemy()) return;
        // イベント開始
        EventManager.instance.StartEvent(eventID);
        // 自身の属性をクリア
        tile.SetAttributeTile(null);
    }
    /// <summary>
    /// キャラクターがこのマスでターン終了した時の処理
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="character"></param>
    public void OnTurnEndOnTile(HexTileData tile, CharacterBase character) {
        
    }
    /// <summary>
    /// ターン経過によるタイル処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnTickTile(HexTileData tile) {
        
    }
}
