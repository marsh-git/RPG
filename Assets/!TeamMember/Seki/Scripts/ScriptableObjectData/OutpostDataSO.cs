using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 難易度別HP構造体
/// </summary>
[System.Serializable]
public struct LevelHPData {
    public eGameLevel level;
    public int HP;
}
/// <summary>
/// エリア別HP構造体
/// </summary>
[System.Serializable]
public struct AreaHPData {
    public int areaID;
    public int HP;
}
/// <summary>
/// 前哨地のHPデータ
/// </summary>
[System.Serializable]
public struct OutpostData {
    public int baseHP;                             // 基礎HP
    public List<LevelHPData> levelHPList;          // 難易度別HP
    public List<AreaHPData> areaHPList;            // エリア別HP
    public List<int> playerNumHP;                  // プレイヤーの人数別HP
}

[CreateAssetMenu(fileName = "OutpostData", menuName = "ScriptableObject/Map/Outpost Database")]
public class OutpostDataSO : ScriptableObject {
    [Header("敵前哨地のHPデータ")]
    [SerializeField] private OutpostData outpostData;

    public OutpostData GetOutpostData() => outpostData;
}
