using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileVisualAssignor {
    private static BiomeVisualDataSO _biomeData = null;
    private static CropsVisualDataSO _cropsData = null;

    /// <summary>
    /// データベースの初期設定（マップ生成開始時等に呼ぶ）
    /// </summary>
    public static void Initialize(BiomeVisualDataSO biomeData, CropsVisualDataSO cropsData) {
        _biomeData = biomeData;
        _cropsData = cropsData;
    }
    /// <summary>
    /// タイル
    /// </summary>
    /// <param name="data"></param>
    public static void SetTileObjectView(HexTileData data) {
        if(data == null) return;
        HexTileObject tileObject = data.GetObject();
        if(tileObject == null) return;

        // バイオームデータの取得
        BiomeVisual biomeData = _biomeData.GetBiomeData(data.biome);
        // 地形オブジェクトの取得
        GameObject terrainObject = biomeData.terrainLayer.GetPrefab(data.terrain);
        // 属性オブジェクトの取得
        GameObject attributeObject = null;
        if(data.Attribute == eAttribute.Crops) {
            if(data.attributeTile is CropsAttribute cropsTile)
                attributeObject = _cropsData.GetCropsPrefab(cropsTile.cropsID, cropsTile.process);
        } else {
            attributeObject = biomeData.attributeLayer.GetPrefab(data.Attribute);
        }
        // 見た目の適応
        tileObject.RefreshVisuals(terrainObject, attributeObject, biomeData.terrainMaterial);
    }
}
