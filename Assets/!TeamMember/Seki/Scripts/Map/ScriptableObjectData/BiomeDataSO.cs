using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewBiomeData", menuName = "Map/Biome Data")]
public class BiomeDataSO : ScriptableObject {
    public eBiome biomeType;
    public Material terrainMaterial;

    public List<TerrainVisual> terrainList = new List<TerrainVisual>();
    public List<AttributeVisual> attributeList = new List<AttributeVisual>();

    public GameObject GetTerrainPrefab(eTerrain type) {
        return terrainList.Find(t => t.terrainType == type).prefab;
    }

    // 属性の基本プレハブ取得
    public GameObject GetAttributePrefab(eAttribute type) {
        return attributeList.Find(a => a.attributeType == type).prefab;
    }
}
