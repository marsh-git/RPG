using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HexMapGenerator : MonoBehaviour{
    [Header("TilePrefabs")]
    [SerializeField] private HexTileObject tilePrefabPlain;
    [SerializeField] private HexTileObject tilePrefabHill;
    [SerializeField] private HexTileObject tilePrefabForest;
    [SerializeField] private HexTileObject tilePrefabMountain;

    /// <summary>
    /// デバッグ用のマップ生成 (データとオブジェクトを同時に生成してManagerへ登録)
    /// </summary>
    public void CreateDebugMap() {
        int mapRadius = 10;
        int currentTileID = 0;
        int currentAreaID = 0;

        // 生成したいエリア（大Hex）の座標リストを定義
        // 画像の通り、エリア1(中央下)、エリア2(左上)、エリア3(右上)の3つを配置
        List<Vector2Int> areasToCreate = new List<Vector2Int>() {
            new Vector2Int(0, 0),   // エリア1
            new Vector2Int(-1, 1),  // エリア2 (左上)
            new Vector2Int(0, 1)    // エリア3 (右上)
        };

        // エリアのループ
        foreach(Vector2Int areaCoord in areasToCreate) {
            // このエリアの中心となる、小Hex基準の絶対座標（オフセット）を計算
            int areaCenterQ = areaCoord.x * (3 * mapRadius + 2) + areaCoord.y * (mapRadius + 1);
            int areaCenterR = areaCoord.y * mapRadius;

            // このエリアに所属することになるタイルのIDリスト
            List<int> registeredTileIDs = new List<int>();

            // エリア内部の小Hex生成ループ（中心 0,0 からの相対ロジックのまま）
            for(int q = -mapRadius; q <= mapRadius; q++) {
                int rStart = Mathf.Max(-mapRadius, -q - mapRadius);
                int rEnd = Mathf.Min(mapRadius, -q + mapRadius);

                for(int r = rStart; r <= rEnd; r++) {
                    // 内部の相対座標に、エリアの中心オフセットを足して「絶対座標」にする
                    int globalQ = areaCenterQ + q;
                    int globalR = areaCenterR + r;

                    // 3D空間の物理位置の計算（絶対座標をベースにするので、自動的にズレて配置される）
                    float x = 2f * (Mathf.Sqrt(3f) * globalQ + Mathf.Sqrt(3f) / 2f * globalR);
                    float z = 2f * (3f / 2f * globalR);
                    Vector3 spawnPosition = new Vector3(x, 0f, z);

                    // 地形のランダム決定
                    eTerrain randomTerrain = (eTerrain)Random.Range((int)eTerrain.Plain, (int)eTerrain.Mountain + 1);
                    HexTileObject prefabToSpawn = GetTerrainPrefab(randomTerrain);

                    // Viewの生成
                    HexTileObject newTileObject = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.Euler(0, 30, 0), this.transform);
                    newTileObject.Setup(spawnPosition);
                    newTileObject.name = $"Tile_[ID:{currentTileID}]_Area:{currentAreaID}_G({globalQ},{globalR})";

                    // Model（データ）の生成
                    HexTileData newTileData = new HexTileData();
                    newTileData.Setup(currentTileID, globalQ, globalR); // 絶対座標を登録
                    newTileData.SetTerrain(randomTerrain);

                    // Managerへ登録
                    HexTileManager.instance.AddTile(newTileData, newTileObject);

                    // エリア管理用にIDをキープ
                    registeredTileIDs.Add(currentTileID);
                    currentTileID++;
                }
            }

            // エリアデータの生成とセットアップ
            HexAreaData newAreaData = new HexAreaData();
            // 仮でエリアごとに異なるバイオームを割り振る
            eBiome areaBiome = (eBiome)((currentAreaID % (int)eBiome.Max) + 1);

            newAreaData.Setup(currentAreaID, areaCoord.x, areaCoord.y, areaBiome, registeredTileIDs);

            // マネージャー側のリストにエリアデータを登録
            HexTileManager.instance.AddArea(newAreaData);

            currentAreaID++;
        }

        Debug.Log($"【入れ子マップ生成完了】総エリア数: {currentAreaID} / 総タイル数: {currentTileID}");
    }

    private HexTileObject GetTerrainPrefab(eTerrain terrain) {
        switch(terrain) {
            case eTerrain.Plain:
            return tilePrefabPlain;
            case eTerrain.Hill:
            return tilePrefabHill;
            case eTerrain.Forest:
            return tilePrefabForest;
            case eTerrain.Mountain:
            return tilePrefabMountain;
        }
        return null;
    }
    public static int DecideSeedByLevel() {
        // 難易度選択のみでゲームが開始されるときは、難易度に応じたシード値を決定する。
        return -1;
    }
    public static int DecideSeedByCustom() {
        // カスタムルールでゲームが開始されるときは、カスタム内容に応じたシード値を決定する。
        return -1;
    }
    public static void CreateMap() {
        // シード値に応じたマップ生成を行う。
    }
    /// <summary>
    /// 街マスの取得
    /// </summary>
    private static void CreateTown() {
        // 中心からタイルを決定する

        // 中心タイルから周囲6マスも街マスとする

        // ※そのため、街マスの中心は端マスより1マス内側でなければいけない
    }

}