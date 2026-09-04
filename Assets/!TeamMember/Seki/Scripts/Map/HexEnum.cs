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
    Invalid = -1,   // 無効
    Grassland,      // 草原　    移動+1
    Desert,         // 砂漠      移動-1
    Rainforest,     // 熱帯雨林　視界-1
    Tundra,         // ツンドラ  移動-1
    Volcanic,       // 火山帯    仮)毎ターンダメージ

    Max
}
/// <summary>
/// タイルの地形 移動コストに関わる
/// </summary>
public enum eTerrain {
    Invalid = -1,   // 無効
    Plain,          // 平原マス
    Hill,           // 丘陵マス
    Forest,         // 森林マス
    Mountain,       // 山岳マス

    Max
}
/// <summary>
/// タイルの属性 現時点では平原、丘陵マスのみに属性付与、進行不可能マスは山岳マスに適応
/// </summary>
public enum eAttribute {
    Invalid = -1,   // 無効
    Event,          // イベントマス
    Crops,          // 作物マス
    Town,           // 街マス
    Outpost,        // 敵の前哨基地マス
    Shop,           // ショップ
    Camp,           // チェックポイント
    CannotMove,     // 進行不可能マス

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
/// <summary>
/// タイルハイライトの種類
/// </summary>
public enum eTileHighlight {
    Invalid = -1,       // 無効
    LineHighlight,      // 枠線
    TileHighlight,      // 内側
    PlayerHighlight,    // プレイヤー
    BattleHighlight,

    Max
}
/// <summary>
/// タイルの移動ステート
/// </summary>
public enum eTileMoveState {
    Invalid = -1,   // 無効
    Normal,         // 通常
    Movable,        // 移動可能
    CharacterIn,    // キャラクターがいる状態
    Reserved,       // 他のキャラの移動予約

    Max
}
/// <summary>
/// 建物の種類
/// </summary>
public enum eBuildingType{
    Invalid = -1,   // 無効
    Building,       // 建物
    Attackable,     // 攻撃可能建物
}
/// <summary>
/// 数量列挙体
/// </summary>
public enum eAmount {
    None,
    Low,
    Medium,
    High
}