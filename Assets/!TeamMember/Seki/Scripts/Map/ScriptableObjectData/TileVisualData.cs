using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 地形ごとの見た目
/// </summary>
[System.Serializable]
public struct TerrainVisual {
    public eTerrain terrainType;
    public GameObject prefab; // 平原なら空、森林なら木、山脈なら岩など
}
/// <summary>
/// 属性ごとの見た目
/// </summary>
[System.Serializable]
public struct AttributeVisual {
    public eAttribute attributeType;
    public GameObject prefab; // 街、前哨基地、ショップなどの静的プレハブ
}