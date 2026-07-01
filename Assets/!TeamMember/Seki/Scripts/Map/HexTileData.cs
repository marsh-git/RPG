using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイルの地形 移動コストに関わる
/// </summary>
public enum eTerrain {
    Invalid,    // 無効
    Plain,      // 平原マス
    Hill,       // 丘陵マス
    Forest,     // 森林マス
    Mountain,   // 山岳マス

    Max
}
/// <summary>
/// タイルの属性 現時点では平原、丘陵マスのみに属性付与、進行不可能マスは山岳マスに適応
/// </summary>
public enum eAttribute {
    None = -1,  // 無し
    Event,      // イベントマス
    Crops,      // 作物マス
    Town,       // 街マス
    Outpost,    // 敵の前哨基地マス
    CannotMove, // 進行不可能マス

    Max
}

/// <summary>
/// 六角形マスのデータクラス
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
    // マスの地形
    public eTerrain terrain { get; private set; } = eTerrain.Invalid;
    // マスの属性
    public eAttribute attribute { get; private set; } = eAttribute.None;

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
        // TODO : オブジェクトのセットアップ

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
