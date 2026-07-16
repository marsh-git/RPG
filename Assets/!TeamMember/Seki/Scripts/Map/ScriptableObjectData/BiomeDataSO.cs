using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBiomeData", menuName = "ScriptableObject/Map/Biome Data")]
public class BiomeDataSO : ScriptableObject {

    [Header("ーーー 全バイオーム一括管理マスター ーーー")]
    public BiomeData grassland;   // 草原
    public BiomeData desert;      // 砂漠
    public BiomeData rainforest;  // 熱帯雨林
    public BiomeData tundra;      // ツンドラ
    public BiomeData volcanic;    // 火山帯

    /// <summary>
    /// バイオームタイプに応じて、そのバイオームデータを返す
    /// </summary>
    public BiomeData GetBiomeVisual(eBiome type) {
        switch(type) {
            case eBiome.Grassland:
            return grassland;
            case eBiome.Desert:
            return desert;
            case eBiome.Rainforest:
            return rainforest;
            case eBiome.Tundra:
            return tundra;
            case eBiome.Volcanic:
            return volcanic;
        }
        return new BiomeData();
    }
}