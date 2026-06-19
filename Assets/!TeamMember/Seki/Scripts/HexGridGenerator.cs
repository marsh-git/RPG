using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    // 外部からシングルトン（どこからでもアクセスできる仕組み）にして経路探索から呼びやすくする
    public static HexGridGenerator Instance { get; private set; }

    [Header("References")]
    [SerializeField] private HexTile tilePrefab;
    [SerializeField] private HexUnit unitPrefab;

    [Header("Grid Settings")]
    [SerializeField] private int mapWidth = 10;
    [SerializeField] private int mapHeight = 10;
    [SerializeField] private float tileRadius = 1f;

    [Header("Terrain Colors")]
    [SerializeField] private Color plainsColor = new Color(0.3f, 0.7f, 0.3f);
    [SerializeField] private Color desertColor = new Color(0.85f, 0.75f, 0.5f);
    [SerializeField] private Color mountainColor = new Color(0.5f, 0.4f, 0.3f);
    [SerializeField] private Color oceanColor = new Color(0.2f, 0.4f, 0.8f);

    // ★★座標からタイルを高速検索するための辞書★★
    private Dictionary<Vector2Int, HexTile> allTiles = new Dictionary<Vector2Int, HexTile>();

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (tilePrefab == null) return;
        GenerateGrid();

        // マップの真ん中付近（例: 4, 4）にユニットを置いて周囲の広がりをテストする
        SpawnDebugUnit(new Vector2Int(4, 4));
    }

    private void GenerateGrid()
    {
        // mapWidth を「中心からの六角形の半径（マップサイズ）」として扱います
        int mapRadius = mapWidth;

        // 中心 (0,0) から半径 mapRadius の範囲を巡回するループ
        for (int q = -mapRadius; q <= mapRadius; q++)
        {
            // r の範囲を q に応じて制限することで、全体が六角形になります
            int rStart = Mathf.Max(-mapRadius, -q - mapRadius);
            int rEnd = Mathf.Min(mapRadius, -q + mapRadius);

            for (int r = rStart; r <= rEnd; r++)
            {
                // 3D空間への変換公式（既存のものをベースに調整）
                // 中心を (0,0,0) に合わせるため、数式はそのままで綺麗に中央に配置されます
                float x = tileRadius * (Mathf.Sqrt(3f) * q + Mathf.Sqrt(3f) / 2f * r);
                float z = tileRadius * (3f / 2f * r);
                Vector3 spawnPosition = new Vector3(x, 0, z);

                HexTile newTile = Instantiate(tilePrefab, spawnPosition, Quaternion.Euler(0, 30, 0), transform);

                TerrainType randomType = (TerrainType)Random.Range(0, 4);
                Color tileColor = randomType switch
                {
                    TerrainType.Plains => plainsColor,
                    TerrainType.Desert => desertColor,
                    TerrainType.Mountain => mountainColor,
                    TerrainType.Ocean => oceanColor,
                    _ => plainsColor
                };

                // q, r はマイナス値も持つようになります
                newTile.Initialize(q, r, randomType, tileColor);
                allTiles.Add(newTile.axialCoordinate, newTile);
            }
        }
    }
    private void SpawnDebugUnit(Vector2Int targetCoord)
    {
        if (unitPrefab == null) return;

        // 生成した全タイルの中から、指定された座標のタイルを直接探す
        HexTile startTile = GetTileAt(targetCoord);

        if (startTile != null)
        {
            // タイルの「実際のWorld位置」にユニットを生成する
            HexUnit spawnedUnit = Instantiate(unitPrefab, startTile.transform.position, Quaternion.identity);

            // ユニットの内部データと位置を確定
            spawnedUnit.SetPosition(targetCoord, startTile.transform.position);

            Debug.Log($"【ユニット配置完了】 座標 {targetCoord} (実際のWorld位置: {startTile.transform.position}) に配置しました。");
        }
        else
        {
            Debug.LogError($"【エラー】 座標 {targetCoord} のタイルがマップ内に存在しないため、ユニットを配置できませんでした。");
        }
    }
    // ★★外部から座標を指定してタイルを取得する安全なメソッド★★
    public HexTile GetTileAt(Vector2Int coordinate)
    {
        if (allTiles.TryGetValue(coordinate, out HexTile tile))
        {
            return tile;
        }
        return null;
    }
}