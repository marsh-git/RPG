using Mirror.BouncyCastle.Asn1.Pkcs;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形ごとの見た目
/// </summary>
[System.Serializable]
public struct TerrainVisual {
    public GameObject plain;    // 平原
    public GameObject hill;     // 丘陵
    public GameObject forest;   // 森林
    public GameObject mountain; // 山脈

    /// <summary>
    /// 地形に応じたオブジェクトを返す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetPrefab(eTerrain type) {
        switch(type) {
            case eTerrain.Plain:
            return plain;
            case eTerrain.Hill:
            return hill;
            case eTerrain.Forest:
            return forest;
            case eTerrain.Mountain:
            return mountain;
        }
        return null;
    }
}
/// <summary>
/// 属性ごとの見た目
/// </summary>
[System.Serializable]
public struct AttributeVisual {
    public GameObject eventTile; // イベント
    public GameObject town;      // 街
    public GameObject outpost;   // 前哨基地
    public GameObject shop;      // ショップ
    public GameObject camp;

    /// <summary>
    /// 属性に応じたオブジェクトを返す
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject GetPrefab(eAttribute type) {
        switch(type) {
            case eAttribute.Event:
            return eventTile;
            case eAttribute.Town:
            return town;
            case eAttribute.Outpost:
            return outpost;
            case eAttribute.Shop:
            return shop;
            case eAttribute.Camp:
            return camp;
        }
        return null;
    }
}
/// <summary>
/// バイオームごとのデータ
/// </summary>
[System.Serializable]
public struct BiomeVisual {
    public Material terrainMaterial; // 地形マテリアル

    public TerrainVisual terrainLayer;   // 地形階層

    public AttributeVisual attributeLayer; // 属性階層
}