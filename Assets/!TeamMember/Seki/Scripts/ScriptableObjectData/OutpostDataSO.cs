using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 難易度別HPデータ
/// </summary>
[System.Serializable]
public struct LevelHPData {
    public eGameLevel level;    // ゲーム難易度
    public int HP;              // 難易度によるHP補正値
}

/// <summary>
/// エリア別データ
/// </summary>
[System.Serializable]
public struct AreaData {
    public int areaID;          // エリアID
    public int HP;              // エリアによるHP補正値
    public int maxSpawnNum;     // エリア別の最大スポーン数
}

/// <summary>
/// 前哨地の設定データ
/// </summary>
[System.Serializable]
public struct OutpostData {
    public int baseHP;                           // 基礎HP
    public List<LevelHPData> levelHPList;        // 難易度別HP補正
    public List<AreaData> areaDataList;          // エリア別データ
    public List<int> playerNumHP;                // プレイヤー人数別HP補正
}

/// <summary>
/// 条件から算出された前哨地の最終データ
/// </summary>
public struct OutpostResultData {
    public int HP;              // 最終HP
    public int maxSpawnNum;     // 最大スポーン数
}

[CreateAssetMenu(fileName = "OutpostData",menuName = "ScriptableObject/Map/Outpost Database")]
public class OutpostDataSO : ScriptableObject {
    [Header("敵前哨地のHPデータ")]
    [SerializeField]
    private OutpostData outpostData;


    /// <summary>
    /// 指定された難易度・エリア・プレイヤー人数から前哨地の最終データを算出する。
    /// </summary>
    /// <param name="level"></param>
    /// <param name="areaID"></param>
    /// <param name="playerNum"></param>
    /// <returns>算出された前哨地データ</returns>
    public OutpostResultData CalculateOutpostData(eGameLevel level,int areaID,int playerNum) {
        OutpostResultData result = new OutpostResultData();
        // 基礎HPを設定
        result.HP = outpostData.baseHP;
        // 難易度別HP補正値を加算
        if (TryGetLevelHP(level, out int levelHP)) result.HP += levelHP;
        // エリア別データを加算
        if (TryGetAreaData(areaID, out AreaData areaData)) {
            result.HP += areaData.HP;
            result.maxSpawnNum = areaData.maxSpawnNum;
        }
        // プレイヤー人数別HP補正値を加算
        if (TryGetPlayerNumHP(playerNum, out int playerHP)) result.HP += playerHP;
        // 最終結果を返す
        return result;
    }
    /// <summary>
    /// 指定された難易度のHP補正値を取得
    /// </summary>
    /// <param name="level"></param>
    /// <param name="HP"></param>
    /// <returns></returns>
    private bool TryGetLevelHP(eGameLevel level, out int HP) {
        foreach (LevelHPData data in outpostData.levelHPList) {
            if (data.level == level) {
                HP = data.HP;
                return true;
            }
        }
        HP = 0;
        return false;
    }
    /// <summary>
    /// 指定されたエリアIDのデータを取得
    /// </summary>
    /// <param name="areaID"></param>
    /// <param name="areaData"></param>
    /// <returns></returns>
    private bool TryGetAreaData(int areaID,out AreaData areaData) {
        foreach (AreaData data in outpostData.areaDataList) {
            if (data.areaID == areaID) {
                areaData = data;
                return true;
            }
        }
        areaData = new AreaData();
        return false;
    }
    /// <summary>
    /// 指定されたプレイヤー人数のHP補正値を取得
    /// </summary>
    /// <param name="playerNum"></param>
    /// <param name="hp"></param>
    /// <returns></returns>
    private bool TryGetPlayerNumHP(int playerNum, out int HP) {
        // プレイヤー人数は1から始まるため、インデックスを調整
        int index = playerNum - 1;
        if (index < 0 && index >= outpostData.playerNumHP.Count) {
            HP = 0;
            return false;
        }
        // HP補正値を取得
        HP = outpostData.playerNumHP[index];
        return true;
    }
}