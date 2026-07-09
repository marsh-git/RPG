using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class HexMapGenerator : MonoBehaviour{
    // TODO:ここのタイルに関してはのちにScriptableObjectに紐づけて
    // TODO:バイオームによってタイルの見た目を変えられるようにする
    [Header("TilePrefabs")]
    [SerializeField] private HexTileObject tilePrefabPlain = null;
    [SerializeField] private HexTileObject tilePrefabHill = null;
    [SerializeField] private HexTileObject tilePrefabForest = null;
    [SerializeField] private HexTileObject tilePrefabMountain = null;
    // TODO:ここのキャラクターに関してものちにScriptableObjectに紐づけてすっきりしたい
    [Header("UnitPrefabs")]
    [SerializeField] private PlayerBase playerPrefab = null;
    [SerializeField] private EnemySpawner enemySpawner = null;
    public void CreateDebugMap() {
        int mapRadius = 10;
        int currentTileID = 0;
        int currentAreaID = 0;

        List<HexTileObject> spawnTileList = new List<HexTileObject>();
        // 生成するエリアの中心アキシアル座標を管理するリスト
        List<Vector2Int> areaCentersToCreate = DecideDirArea();

        // エリアのループ
        foreach(Vector2Int areaCenter in areaCentersToCreate) {
            int areaCenterQ = areaCenter.x;
            int areaCenterR = areaCenter.y;

            List<int> registeredTileIDs = new List<int>();

            // エリア内部の小Hex生成ループ
            for(int q = -mapRadius; q <= mapRadius; q++) {
                int rStart = Mathf.Max(-mapRadius, -q - mapRadius);
                int rEnd = Mathf.Min(mapRadius, -q + mapRadius);

                for(int r = rStart; r <= rEnd; r++) {
                    int globalQ = areaCenterQ + q;
                    int globalR = areaCenterR + r;

                    // 3D物理空間への座標変換（Pointy-Toppedの正しい変換式）
                    float x = 2f * (Mathf.Sqrt(3f) * globalQ + Mathf.Sqrt(3f) / 2f * globalR);
                    float z = 2f * (3f / 2f * globalR);
                    Vector3 spawnPosition = new Vector3(x, 0f, z);

                    eTerrain randomTerrain = (eTerrain)Random.Range((int)eTerrain.Plain, (int)eTerrain.Mountain + 1);
                    HexTileObject prefabToSpawn = GetTerrainPrefab(randomTerrain);

                    // Viewの生成
                    HexTileObject newTileObject = Instantiate(prefabToSpawn, Vector3.zero, Quaternion.Euler(0, 30, 0), this.transform);
                    newTileObject.Setup(currentTileID, spawnPosition);
                    newTileObject.name = $"Tile_[ID:{currentTileID}]_Area:{currentAreaID}_G({globalQ},{globalR})";

                    // Model（データ）の生成
                    HexTileData newTileData = new HexTileData();
                    newTileData.Setup(currentTileID, globalQ, globalR);
                    newTileData.SetTerrain(randomTerrain);

                    // Managerへ登録
                    HexTileManager.instance.AddTile(newTileData, newTileObject);

                    if(randomTerrain != eTerrain.Mountain && currentAreaID == 0) {
                        spawnTileList.Add(newTileObject);
                    }
                    registeredTileIDs.Add(currentTileID);
                    currentTileID++;
                }
            }

            // エリアデータの生成とセットアップ
            HexAreaData newAreaData = new HexAreaData();
            eBiome areaBiome = (eBiome)((currentAreaID % (int)eBiome.Max) + 1);
            newAreaData.Setup(currentAreaID, areaCenterQ, areaCenterR, areaBiome, registeredTileIDs);
            HexTileManager.instance.AddArea(newAreaData);
            currentAreaID++;
        }

        Debug.Log($"【マップ生成完了】総エリア数: {currentAreaID} / 総タイル数: {currentTileID}");

        // プレイヤーの生成
        SpawnPlayer(spawnTileList);
        enemySpawner.SpawnEnemy(spawnTileList, 5);
    }
    /// <summary>
    /// 候補タイルリストからランダムな位置にプレイヤーを生成・配置する
    /// </summary>
    private void SpawnPlayer(List<HexTileObject> candidateTiles) {
        // 湧き候補が1つもない場合は処理を中断
        if(candidateTiles == null || candidateTiles.Count == 0 || playerPrefab == null) {
            Debug.Log("プレイヤーの生成に失敗しました（候補タイルがない、またはPrefabが未設定です）");
            return;
        }

        // リストの要素数からランダムタイルを取得
        int randomIndex = UnityEngine.Random.Range(0, candidateTiles.Count);
        HexTileObject targetTileObj = candidateTiles[randomIndex];

        // 選択されたタイルの3D空間上の座標を取得
        Vector3 spawnPos = targetTileObj.transform.position;

        // プレイヤー生成
        PlayerBase player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        player.name = "Player_Debug";
        // プレイヤーのタイルID設定
        player.SetTile(targetTileObj.ID);
        Debug.Log($"【プレイヤー生成成功】タイルID: {targetTileObj.name} の位置に配置しました。");
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
    /// <summary>
    /// 中心エリアから隣接するエリアをランダムに決定する
    /// </summary>
    /// <returns></returns>
    private static List<Vector2Int> DecideDirArea() {
        // 生成するエリアの中心アキシアル座標を管理するリスト
        List<Vector2Int> areaCentersToCreate = new List<Vector2Int>();
        // エリア1は必ず(0, 0)
        areaCentersToCreate.Add(new Vector2Int(0, 0));
        // 半径10のPointy-Topped大Hexが完全密着するための数学的に正しい6方向の相対座標リスト
        Vector2Int[] bigHexOffsets = new Vector2Int[] {
            new Vector2Int(21, -10),  // 東南東 (右下)
            new Vector2Int(10, 11),   // 南 (真下方向)
            new Vector2Int(-11, 21),  // 南西南 (左下)
            new Vector2Int(-21, 10),  // 西北西 (左上)
            new Vector2Int(-10, -11), // 北 (真上方向)
            new Vector2Int(11, -21)   // 北東北 (右上)
        };
        // ランダムに2つの方向インデックスを選択（重複なし）
        List<int> directionIndices = new List<int> { 0, 1, 2, 3, 4, 5 };
        for(int i = 0; i < 2; i++) {
            int randIdx = Random.Range(0, directionIndices.Count);
            int chosenDirIdx = directionIndices[randIdx];
            directionIndices.RemoveAt(randIdx);

            // 確定した隣接エリアの中心座標を追加
            areaCentersToCreate.Add(bigHexOffsets[chosenDirIdx]);
        }
        return areaCentersToCreate;
    }
}