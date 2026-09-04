using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct AmountRangeMap {
    public eAmount amount;
    public IntRange range;
}
[System.Serializable]
public struct TerrainAmountSetting {
    public eTerrain terrain;
    public eAmount amount;
}
[System.Serializable]
public struct BiomeSetting {
    public eBiome biome;
    [Tooltip("各地形の生成量設定")]
    public List<TerrainAmountSetting> terrainAmountList;
}

[CreateAssetMenu(fileName = "NewBiomeData", menuName = "ScriptableObject/Map/BiomeTerrain DataBase")]
public class BiomeTerrainDataSO : ScriptableObject {
    [Header("eAmountに対応する生成数の最小・最大範囲定義")]
    [SerializeField] private List<AmountRangeMap> _amountRangeTable = new List<AmountRangeMap>();

    [Header("バイオームごとの地形生成設定")]
    [SerializeField] private List<BiomeSetting> _biomeSettingList = new List<BiomeSetting>();

    /// <summary>
    /// 指定されたeAmountに対応するIntRangeを取得
    /// </summary>
    public IntRange GetRange(eAmount amount) {
        var target = _amountRangeTable.Find(x => x.amount == amount);
        return target.range;
    }

    /// <summary>
    /// 指定バイオームにおける特定地形の eAmount 設定を取得する。
    /// </summary>
    public eAmount GetTerrainAmount(eBiome biome, eTerrain terrain) {
        var biomeData = _biomeSettingList.Find(x => x.biome == biome);
        if (biomeData.terrainAmountList != null) {
            var terrainData = biomeData.terrainAmountList.Find(x => x.terrain == terrain);
            return terrainData.amount;
        }
        return eAmount.None;
    }

    /// <summary>
    /// シード乱数に基づき、指定されたバイオームと地形に応じた生成個数を決定する。
    /// </summary>
    public int EvaluateTerrainCount(eBiome biome, eTerrain terrain, System.Random rand) {
        eAmount amount = GetTerrainAmount(biome, terrain);
        IntRange range = GetRange(amount);
        return range.SeedRandom(rand);
    }
}
