using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 六角形タイルのデータクラス
/// </summary>
public class HexTileData {
    // ユニークID
    public int ID { get; private set; } = -1;
    // エリアID
    public int areaID { get; private set; } = -1;
    // マス上にいるキャラクターID
    public bool isPlayerIn { get; private set; } = false;
    // マス上のX座標
    public int gridPosX { get; private set; } = -1;
    // マス上のY座標
    public int gridPosY { get; private set; } = -1;
    // タイルの状態
    public eTileMoveState tileState { get; private set; } = eTileMoveState.Normal;
    // タイルの地形
    public eTerrain terrain { get; private set; } = eTerrain.Invalid;
    // タイル属性
    public IAttributeTile attributeTile { get; private set; } = null;
    // タイルの属性
    public eAttribute Attribute => attributeTile?.AttributeType ?? eAttribute.None;
    // タイルのバイオーム
    public eBiome biome { get; private set; } = eBiome.None;

    /// <summary>
    /// 座標のセットアップ処理
    /// </summary>
    /// <param name="setID"></param>
    /// <param name="setX"></param>
    /// <param name="setY"></param>
    public void Setup(int setID, int setX, int setY) {
        ID = setID;
        gridPosX = setX;
        gridPosY = setY;
    }
    /// <summary>
    /// エリアID設定
    /// </summary>
    /// <param name="setAreaID"></param>
    public void SetAreaID(int setAreaID) {
        areaID = setAreaID;
    }
    /// <summary>
    /// タイル状態の設定
    /// </summary>
    /// <param name="setState"></param>
    public void SetTileState(eTileMoveState setState) {
        tileState = setState;
    }
    /// <summary>
    /// 地形の設定
    /// </summary>
    /// <param name="setTerrain"></param>
    public void SetTerrain(eTerrain setTerrain) {
        terrain = setTerrain;
    }
    /// <summary>
    /// タイル属性の設定
    /// </summary>
    /// <param name="setAttribute"></param>
    public void SetAttributeTile(IAttributeTile setAttribute) {
        attributeTile = setAttribute;
    }
    /// <summary>
    /// タイル属性のクリア
    /// </summary>
    public void ClearAtrribute() {
        attributeTile = AttributeFactory.Create(eAttribute.None);
    }
    /// <summary>
    /// バイオームの設定
    /// </summary>
    /// <param name="setBiome"></param>
    public void SetBiome(eBiome setBiome) {
        biome = setBiome;
    }
    /// <summary>
    /// 対応するタイルオブジェクト取得
    /// </summary>
    /// <returns></returns>
    public HexTileObject GetObject() {
        return HexTileManager.instance.GetTileObject(ID);
    }
    public Vector3 GetTilePos() {
        return (Vector3)(GetObject()?.transform.position);
    }
    /// <summary>
    /// 地形に応じた移動コストの取得
    /// </summary>
    public float GetMovementCost() {
        float moveCost = 0.0f;

        // 地形によるコスト計算
        switch(terrain) {
            case eTerrain.Plain:
            moveCost = 1.0f;
            break;
            case eTerrain.Hill:
            moveCost = 2.0f;
            break;
            case eTerrain.Forest:
            moveCost = 2.0f;
            break;
            case eTerrain.Mountain:
            moveCost = 999.0f;     // 進行不可能
            break;
        }
        // バイオームによる移動補正
        switch(biome) {
            case eBiome.Grassland:
            break;
            case eBiome.Desert:
            moveCost += 0.5f;
            break;
            case eBiome.Rainforest:
            moveCost += 0.5f;
            break;
            case eBiome.Tundra:
            break;
            case eBiome.Volcanic:
            break;
        }
        return moveCost;
    }
}
