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
    // マス上のX座標
    public int gridPosX { get; private set; } = -1;
    // マス上のY座標
    public int gridPosY { get; private set; } = -1;
    // タイルの地形
    public eTerrain terrain { get; private set; } = eTerrain.Invalid;
    // タイルの属性
    public eAttribute attribute { get; private set; } = eAttribute.None;
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
    /// 地形の設定
    /// </summary>
    /// <param name="setTerrain"></param>
    public void SetTerrain(eTerrain setTerrain) {
        terrain = setTerrain;
    }
    /// <summary>
    /// 属性の設定
    /// </summary>
    /// <param name="setAttribute"></param>
    public void SetAttribute(eAttribute setAttribute) {
        attribute = setAttribute;
    }
    /// <summary>
    /// バイオームの設定
    /// </summary>
    /// <param name="setBiome"></param>
    public void SetBiome(eBiome setBiome) {
        biome = setBiome;
    }
    /// <summary>
    /// 地形に応じた移動コストの取得
    /// </summary>
    public int GetMovementCost() {
        switch(terrain) {
            case eTerrain.Plain:
            return 1;
            case eTerrain.Hill:
            return 2;
            case eTerrain.Forest:
            return 2;
            case eTerrain.Mountain:
            return 999;
        }
        return -1;
    }
}
