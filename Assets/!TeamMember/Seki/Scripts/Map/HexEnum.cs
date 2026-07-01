using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームの難易度
/// </summary>
public enum eGameLevel {
    Invalid = -1,
    Easy,
    Normal,
    Hard,

    Max
}
/// <summary>
/// エリア内環境の種類
/// </summary>
public enum eBiome {
    None,
    Grassland,      // 草原　    移動+1
    Savanna,        // サバンナ
    Desert,         // 砂漠      移動-1
    Rainforest,     // 熱帯雨林　視界-1
    Tundra,         // ツンドラ  移動-1
    VolcanicZone,   // 火山帯    仮)毎ターンダメージ

    Max
}
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
/// 6方向
/// </summary>
public enum eDirectionHex {
    Invalid = -1,   // 無効
    UpRight,        // 右上
    Right,          // 右
    DownRight,      // 右下
    DownLeft,       // 左下
    Left,           // 左
    UpLeft,         // 左上

    Max
}