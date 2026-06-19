using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TerrainType {
    Plains,
    Desert,
    Mountain,
    Ocean
}

public class HexTile : MonoBehaviour
{
    [Header("Tile Data")]
    public Vector2Int axialCoordinate;
    public TerrainType terrainType;

    [Header("Visual Settings")]
    [SerializeField] private Renderer tileRenderer;

    private Color originalColor;
    private MaterialPropertyBlock propertyBlock;

    public int MovementCost
    {
        get
        {
            // タイル自身の地形タイプ（terrainType）に応じて、厳格にコストを返す
            switch (this.terrainType)
            {
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
    void Awake()
    {
        if (tileRenderer == null) tileRenderer = GetComponent<Renderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    // ★★自動生成用の初期化関数★★
    public void Initialize(int q, int r, TerrainType type, Color defaultColor)
    {
        axialCoordinate = new Vector2Int(q, r);
        terrainType = type;
        gameObject.name = $"Hex_{q}_{r} ({type})";

        // 地形に応じたベースの色を記憶・適用する
        originalColor = defaultColor;
        SetColor(originalColor);
    }

    public void SetColor(Color color)
    {
        if (tileRenderer == null) return;
        tileRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", color);
        tileRenderer.SetPropertyBlock(propertyBlock);
    }

    public void ResetColor()
    {
        SetColor(originalColor);
    }
}