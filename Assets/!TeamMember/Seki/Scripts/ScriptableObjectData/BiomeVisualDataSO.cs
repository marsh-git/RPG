using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBiomeData", menuName = "ScriptableObject/Map/Biome DataBase")]
public class BiomeVisualDataSO : ScriptableObject {

    [Header("ーーー 全バイオーム一括管理マスター ーーー")]
    [SerializeField] private BiomeVisual grassland;   // 草原
    [SerializeField] private BiomeVisual desert;      // 砂漠
    [SerializeField] private BiomeVisual rainforest;  // 熱帯雨林
    [SerializeField] private BiomeVisual tundra;      // ツンドラ
    [SerializeField] private BiomeVisual volcanic;    // 火山帯

    /// <summary>
    /// バイオームタイプに応じて、そのバイオームデータを返す
    /// </summary>
    public BiomeVisual GetBiomeData(eBiome type) {
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
        return new BiomeVisual();
    }
}