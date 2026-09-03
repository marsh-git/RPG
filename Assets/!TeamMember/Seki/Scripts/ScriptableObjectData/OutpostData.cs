using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 前哨地のHPデータ
/// </summary>
public struct OutpostData{
    int baseHP;         // 基礎HP
    int levelHP;        // 難易度別HP
    int areaHP;         // エリア別HP
    int playerNumHP;    // プレイヤーの人数別HP
}
