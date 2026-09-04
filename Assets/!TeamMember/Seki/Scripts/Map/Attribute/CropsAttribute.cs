using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropsAttribute : IAttributeTile {
    // 作物ID
    public int cropsID { get; private set; } = -1;
    // 作物の進捗
    public eCropsProcess process { get; private set; } = eCropsProcess.None;
    // 成長カウンタ
    private int _growCounter = -1;
    // 要求ターン数
    private const int _REQUIRE_GROW_TURN = 3;

    /// <summary>
    /// 自身の属性取得
    /// </summary>
    public eAttribute AttributeType => eAttribute.Crops;
    /// <summary>
    /// 自身の建物タイプ取得
    /// </summary>
    public eBuildingType BuildingType => eBuildingType.Invalid;

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
        
    }
    /// <summary>
    /// ターン経過によるタイル処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnTickTile(HexTileData tile) {
        if(process == eCropsProcess.None || process == eCropsProcess.Harvest) return;
        // カウンタを増やす
        _growCounter++;
        // 一定ターン経過で成長させる
        if(_growCounter >= _REQUIRE_GROW_TURN) {
            _growCounter = 0;
            GrowUp();
        }
    }
    /// <summary>
    /// 準備処理
    /// </summary>
    public void Setup(int setID) {
        cropsID = setID;
        // 生成時は収穫可能にしておく
        process = eCropsProcess.Harvest;
        _growCounter = 0;
    }
    /// <summary>
    /// 作物を植える
    /// </summary>
    /// <param name="setID"></param>
    public void PlantCrops(int setID) {
        cropsID = setID;
        process = eCropsProcess.Seed;
    }
    /// <summary>
    /// 成長処理
    /// </summary>
    public void GrowUp() {
        // カウンタを増やす
        process++;
    }
    /// <summary>
    /// 収穫処理
    /// </summary>
    /// <param name="tile"></param>
    public void HarvestCrops(HexTileData tile) {
        // 収穫処理を行う(プレイヤーのアイテムに作物を渡す)

        // IDを取り除く
        cropsID = -1;
        // 見た目のリセット
        tile.ClearAtrribute();
    }
    /// <summary>
    /// プレイヤーに最初に見つかった時の処理
    /// </summary>
    /// <param name="tile"></param>
    public void OnFoundThisFirst(HexTileData tile) {
        
    }
}