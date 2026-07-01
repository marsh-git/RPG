using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType {
    Plains,     // 平地
    Desert,     // 丘陵
    Mountain,   // 山岳
    Ocean,      // 海

    Max
}

public class HexTile : MonoBehaviour {
    [Header("Tile Data")]
    public Vector2Int axialCoordinate;
    public TerrainType terrainType;

    [Header("Visual Settings")]
    [SerializeField] private Renderer tileRenderer;

    private Color originalColor;
    private MaterialPropertyBlock propertyBlock;

    public int MovementCost {
        get {
            // タイル自身の地形タイプ（terrainType）に応じて、厳格にコストを返す
            switch(this.terrainType) {
                case TerrainType.Plains:
                return 1;   // 平原はコスト1
                case TerrainType.Desert:
                return 2;   // 砂漠はコスト2
                case TerrainType.Ocean:
                return 3;   // 海はコスト3（★★ここが1になっていたのが原因です★★）
                case TerrainType.Mountain:
                return 999; // 山岳は侵入不可
                default:
                return 1;
            }
        }
    }
    void Awake() {
        if(tileRenderer == null) tileRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="q"></param>
    /// <param name="r"></param>
    /// <param name="type"></param>
    /// <param name="defaultColor"></param>
    public void Initialize(int q, int r, TerrainType type, Color defaultColor) {
        axialCoordinate = new Vector2Int(q, r);
        terrainType = type;
        gameObject.name = $"Hex_{q}_{r} ({type})";

        // 地形に応じたベースの色を記憶・適用する
        originalColor = defaultColor;
        SetColor(originalColor);
    }
    /// <summary>
    /// 色の設定
    /// </summary>
    /// <param name="color"></param>
    public void SetColor(Color color) {
        if(tileRenderer == null) return;
        tileRenderer.GetPropertyBlock(propertyBlock);
        //propertyBlock.SetColor("_Color", color);
        GetComponent<Renderer>().material.color = color;
        tileRenderer.SetPropertyBlock(propertyBlock);
    }
    /// <summary>
    /// 色のリセット
    /// </summary>
    public void ResetColor() {
        SetColor(originalColor);
    }
}